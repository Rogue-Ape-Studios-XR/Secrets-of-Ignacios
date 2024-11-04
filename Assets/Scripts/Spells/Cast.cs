using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Services;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.Hands;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    public class Cast : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpellManager _spellManager;
        [SerializeField] private ObjectPooler _pooler;
        [SerializeField] internal HandData _rightHandData;
        [SerializeField] internal HandData _leftHandData;

        [Header("Casting Mode")]
        [SerializeField] private bool _timerAim = true;
        [SerializeField] private float _castTimer = 0.75f;

        internal event Action<Handedness> onSpellCastComplete;
        
        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            SpellManager.OnSpellValidation += HandleSpellValidation;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _rightHandData._chargeEffect.Stop();
            _leftHandData._chargeEffect.Stop();
        }

        private void OnDestroy()
        {
            SpellManager.OnSpellValidation += HandleSpellValidation;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public void CastRightSpell() => CastSpell(_rightHandData);
        public void CastLeftSpell() => CastSpell(_leftHandData);
        public void StopRightCast() => StopCast(_rightHandData);
        public void StopLeftCast() => StopCast(_leftHandData);

        private async void CastSpell(HandData handData)
        {
            try
            {
                if (handData._canCast)
                {
                    if (_spellManager.CurrentSpell is not null && _spellManager.CurrentSpell._castType is CastTypes.SingleFire)
                        await ChargeEffect(handData);

                    ExecuteSpell(handData);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Spell casting was canceled");
            }
        }

        private async UniTask ChargeEffect(HandData handData)
        {
            if (_timerAim && handData._canCast)
            {
                handData._lineRenderer.enabled = true;
                handData._chargeEffect.Play();
                await UniTask.WaitForSeconds(_castTimer, cancellationToken: _cancellationTokenSource.Token);
                handData._lineRenderer.enabled = false;
            }
            else
                handData._lineRenderer.enabled = false;
        }

        private void ExecuteSpell(HandData handData)
        {
            switch (_spellManager.CurrentSpell._castType)
            {
                case CastTypes.SingleFire:
                    SingleCast(handData);
                    break;
                case CastTypes.Automatic:
                    RepeatedCast(handData);
                    break;
                default:
                    throw new NotImplementedException("Cast type not implemented.");
            }
        }

        private void SingleCast(HandData handData)
        {
            _pooler.GetProjectile(_spellManager.CurrentSpell._elementType.ToString(),
                    _spellManager.CurrentSpell._spellPrefab,
                    handData._handTransform);
            handData._renderer.materials[1].SetColor("_MainColor", _spellManager.DefaultColor);
            handData._canCast = false;
            
            RemoveVFXFromHand(handData);
        }

        private async void RepeatedCast(HandData handData)
        {
            handData._isCasting = true;
            while (handData._canCast)
            {
                _pooler.GetProjectile(_spellManager.CurrentSpell._elementType.ToString(),
                    _spellManager.CurrentSpell._spellPrefab,
                    handData._handTransform);

                await UniTask.WaitForSeconds(0.05f, cancellationToken: _cancellationTokenSource.Token);
            }
        }

        private void StopCast(HandData handData)
        {
            if (_spellManager.CurrentSpell != null &&
                _spellManager.CurrentSpell._castType == CastTypes.Automatic &&
                handData._isCasting)
            {
                handData._isCasting = false;
                handData._canCast = false;
                handData._renderer.materials[1].SetColor("_MainColor", _spellManager.DefaultColor);
                
                RemoveVFXFromHand(handData);
            }
        }

        private void RemoveVFXFromHand(HandData handData)
        {
            if (handData == null) return;
            
            Handedness handIdentifier = handData == _leftHandData ? Handedness.Left : Handedness.Right;

            Debug.Log("invoking");
            onSpellCastComplete?.Invoke(handIdentifier);
        }
        
        
        private void HandleSpellValidation(bool canCast)
        {
            _rightHandData._canCast = canCast;
            _leftHandData._canCast = canCast;
        }

        public void CastNoHands(Transform transform, string objectType, GameObject spellPrefab)
        {
            GameObject projectile = _pooler.GetProjectile(objectType, spellPrefab, transform);
            projectile.transform.SetPositionAndRotation(transform.position, transform.localRotation);
        }

        [Serializable]
        internal class HandData
        {
            [Header("References")]
            [SerializeField] internal Transform _handTransform;

            [Header("Visual")]
            [SerializeField] internal VisualEffect _chargeEffect;
            [SerializeField] internal LineRenderer _lineRenderer;
            [SerializeField] internal Renderer _renderer;

            internal bool _canCast = false;
            internal bool _isCasting = false;
        }
    }
}
