using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [SerializeField] private SequenceManager _gestureManager;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Renderer _rightHandMaterial;
        [SerializeField] private Renderer _leftHandMaterial;
        [SerializeField] private Spell[] _availableSpells;

        private Spell _currentSpell;
        private Spell _lastSpell;
        private CancellationTokenSource _cancellationTokenSource;
        private Color _defaultColor;
        private bool _canCastRightHand = false;
        private bool _canCastLeftHand = false;

        public static event Action<bool> OnSpellValidation;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _defaultColor = _rightHandMaterial.materials[1].GetColor("_MainColor");
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

        public void ValidateSequence()
        {
            foreach (var spell in _availableSpells)
                if (spell._gestureSequence.Count == _gestureManager.ValidatedGestures.Count &&
                    spell._gestureSequence.SequenceEqual(_gestureManager.ValidatedGestures))
                {
                    SetSpell(spell);
                    Debug.Log("HEEEEEEEEEEEEEEEEEEY" + spell.name);
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

        public void CastRightHandSpell()
        {
            if (_canCastRightHand)
            {
                var rightHandSpell = Instantiate(_currentSpell._spellPrefab, _rightHand.position, _rightHand.rotation);
                _canCastRightHand = false;

                if (!_canCastLeftHand)
                    HandleReset();
            }
        }

        public void CastLeftHandSpell()
        {
            if (_canCastLeftHand)
            {
                var leftHandSpell = Instantiate(_currentSpell._spellPrefab, _leftHand.position, _leftHand.rotation);
                _canCastLeftHand = false;

                if (!_canCastRightHand)
                    HandleReset();
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

