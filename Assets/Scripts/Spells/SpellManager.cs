using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SequenceManager _gestureManager;
        [SerializeField] private Renderer _rendererRight;
        [SerializeField] private Renderer _rendererLeft;
        [SerializeField] private VisualEffect _chargeEffectRight;
        [SerializeField] private VisualEffect _chargeEffectLeft;

        [Header("Hand Objects")]
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Renderer _rightHandMaterial;
        [SerializeField] private Renderer _leftHandMaterial;

        [Header("Spells")]
        [SerializeField] private Spell[] _availableSpells;

        [Header("Casting Mode")]
        [SerializeField] private bool _timerAim = false;
        [SerializeField] private float _castTimer = 1;

        private Spell _currentSpell;
        private Spell _lastSpell;
        private CancellationTokenSource _cancellationTokenSource;
        private Color _defaultColor;
        private bool _canCastRightHand = false;
        private bool _canCastLeftHand = false;

        private bool _isCastingLeft = false;
        private bool _isCastingRight = false;

        public static event Action<bool> OnSpellValidation;

        internal bool CanCastRightHand => _canCastRightHand;
        internal bool CanCastLeftHand => _canCastLeftHand;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _defaultColor = _rightHandMaterial.materials[1].GetColor("_MainColor");
            _chargeEffectLeft.Stop();
            _chargeEffectRight.Stop();
        }

        private void OnEnable()
        {
            _gestureManager.OnSequenceCreated += ValidateSequence;
            _gestureManager.OnReset += HandleReset;
            _gestureManager.OnQuickCast += HandleOnQuickCast;
        }

        private void OnDestroy()
        {
            _gestureManager.OnSequenceCreated -= ValidateSequence;
            _gestureManager.OnReset -= HandleReset;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void SetSpell(Spell spell)
        {
            _currentSpell = spell;
            _lastSpell = spell;
            _canCastRightHand = true;
            _canCastLeftHand = true;
        }

        public void StopRightCast()
        {
            if (_currentSpell != null && _currentSpell._castType == CastTypes.Automatic && _isCastingRight)
            {
                _canCastRightHand = false;
                _rightHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                _isCastingRight = false;
            }
        }

        public void StopLeftCast()
        {
            if (_currentSpell != null && _currentSpell._castType == CastTypes.Automatic && _isCastingLeft) { 
                _canCastLeftHand = false;
                _leftHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                _isCastingLeft = false;
            }
        }



        public void ValidateSequence()
        {
            foreach (var spell in _availableSpells)
                if (spell._gestureSequence.Count == _gestureManager.ValidatedGestures.Count &&
                    spell._gestureSequence.SequenceEqual(_gestureManager.ValidatedGestures))
                {
                    SetSpell(spell);
                    break;
                }
                else
                {
                    _currentSpell = null;
                    _canCastRightHand = false;
                    _canCastLeftHand = false;
                    SpellWrongIndication(_cancellationTokenSource.Token);
                }

            OnSpellValidation?.Invoke(_canCastRightHand);
        }

        private async void SpellWrongIndication(CancellationToken token)
        {
            try
            {
                float delay = 0.5f;
                int loopAmount = 2;

                for (int i = 0; i < loopAmount; i++)
                {
                    await UniTask.WaitForSeconds(delay, cancellationToken: token);

                    _rightHandMaterial.materials[1].SetColor("_MainColor", Color.red);
                    _leftHandMaterial.materials[1].SetColor("_MainColor", Color.red);

                    await UniTask.WaitForSeconds(delay, cancellationToken: token);

                    _rightHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                    _leftHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("SpellWrongIndication was canceled");
            }
        }


        private async void RightRepeatedCast(Transform hand, CancellationToken token)
        {
            while (_canCastRightHand)
            //for (int i = 0; i < 25; i++)
            {
                Instantiate(_currentSpell._spellPrefab, hand.position, hand.rotation);
                await UniTask.WaitForSeconds(0.02f, cancellationToken: token);
                _isCastingRight = true;
            }
        }


        private async void LeftRepeatedCast(Transform hand, CancellationToken token)
        {
            while (_canCastLeftHand)
            //for (int i = 0; i < 25; i++)
            {
                Instantiate(_currentSpell._spellPrefab, hand.position, hand.rotation);
                await UniTask.WaitForSeconds(0.03f, cancellationToken: token);
                _isCastingLeft = true;
            }
        }

        private void Cast(ref bool handCanCast, ref Renderer handMaterial, Transform hand, bool isRightHand)
        {

            switch (_currentSpell._castType)
            {
                case CastTypes.SingleFire:
                    var spell = Instantiate(_currentSpell._spellPrefab, hand.position, hand.rotation);
                    handCanCast = false;
                    handMaterial.materials[1].SetColor("_MainColor", _defaultColor);
                    break;
                case CastTypes.Automatic:
                    if(isRightHand)
                        RightRepeatedCast(hand, _cancellationTokenSource.Token);
                    else
                        LeftRepeatedCast(hand, _cancellationTokenSource.Token);

                    break;
                default: throw new NotImplementedException("Cast type not implemented (how did we even get here)");
                
            }




            if (!_canCastLeftHand)
                HandleReset();
        }

        public async void CastRightHandSpell()
        {
            try
            {
                if (_timerAim && _canCastRightHand && _currentSpell._castType == CastTypes.SingleFire)
                {
                    _rendererRight.enabled = true;
                    _chargeEffectRight.Play();
                    await UniTask.WaitForSeconds(_castTimer, cancellationToken: _cancellationTokenSource.Token);
                    _rendererRight.enabled = false;
                }
                else
                    _rendererRight.enabled = false;

                if (_canCastRightHand)
                    Cast(ref _canCastRightHand, ref _rightHandMaterial, _rightHand, true);

            }
            catch (OperationCanceledException)
            {
                Debug.LogError("CastRightHandSpell was Canceled");
            }

        }

        public async void CastLeftHandSpell()
        {
            try
            {
                if (_timerAim && _canCastLeftHand && _currentSpell._castType == CastTypes.SingleFire)
                {
                    _rendererLeft.enabled = true;
                    _chargeEffectLeft.Play();
                    await UniTask.WaitForSeconds(_castTimer, cancellationToken: _cancellationTokenSource.Token);
                    _rendererLeft.enabled = false;
                }
                else
                    _rendererLeft.enabled = false;

                if (_canCastLeftHand)
                    Cast(ref _canCastLeftHand, ref _leftHandMaterial, _leftHand, false);
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("CastLeftHandSpell was Canceled");
            }
        }

        internal void HandleReset()
        {
            _currentSpell = null;
            _canCastRightHand = false;
            _canCastLeftHand = false;

            _rightHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
            _leftHandMaterial.materials[1].SetColor("_MainColor", _defaultColor);
        }

        private void HandleOnQuickCast()
        {
            _currentSpell = _lastSpell;
            _rightHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._handColor);
            _leftHandMaterial.materials[1].SetColor("_MainColor", _currentSpell._handColor);
            _canCastLeftHand = true;
            _canCastRightHand = true;
        }
    }
}

