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
            SpellManager.onSpellValidation += HandleSpellValidation;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _rightHandData._chargeEffect.Stop();
            _leftHandData._chargeEffect.Stop();
        }

        private void OnDestroy()
        {
            SpellManager.onSpellValidation -= HandleSpellValidation;

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
                    HandConfig config = handData.GetHandConfig(_spellManager, handData == _rightHandData);

                    if (config._castType == CastTypes.Charged)
                        await ChargeEffect(handData);

                    ExecuteSpell(handData, config._castType);
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

        private void ExecuteSpell(HandData handData, CastTypes castType)
        {
            switch (castType)
            {
                case CastTypes.SingleFire:
                case CastTypes.Charged:
                case CastTypes.Touch:
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
            GetSpellProjectile(handData);
            handData._renderer.materials[1].SetColor("_MainColor", _spellManager.DefaultColor);
            handData._canCast = false;
            
            RemoveVFXFromHand(handData);
        }

        private async void RepeatedCast(HandData handData)
        {
            handData._isCasting = true;
            while (handData._canCast)
            {
                GetSpellProjectile(handData);
                await UniTask.WaitForSeconds(0.05f, cancellationToken: _cancellationTokenSource.Token);
            }
        }

        private void GetSpellProjectile(HandData handData)
        {
            HandConfig spell = handData.GetHandConfig(_spellManager, handData == _rightHandData);

            GameObject obj = _pooler.GetObject(spell._spellPrefab.name, spell._spellPrefab, handData._handTransform);

            if (spell._castType == CastTypes.Touch)
            {
                obj.transform.SetParent(handData._handTransform, false);
                obj.transform.SetLocalPositionAndRotation(spell._position, new(0, 0, 0, 0));
            }
        }

        private void StopCast(HandData handData)
        {
            if (_spellManager.CurrentSpell is not null && handData._isCasting)
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
            GameObject projectile = _pooler.GetObject(objectType, spellPrefab, transform);
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

            internal Transform _spellPrefab;
            internal bool _canCast = false;
            internal bool _isCasting = false;

            internal HandConfig GetHandConfig(SpellManager spellManager, bool isRightHand)
            {
                if (!spellManager.CurrentSpell._duoSpell)
                    return spellManager.CurrentSpell._primaryConfig;
                else
                {
                    if (isRightHand)
                        return spellManager.CurrentSpell._primaryConfig;
                    else
                        return spellManager.CurrentSpell._secondaryConfig;
                }
            }
        }
    }
}
