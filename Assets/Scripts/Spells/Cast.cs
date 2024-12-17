using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Player;
using RogueApeStudios.SecretsOfIgnacios.Player.SpellMagicCircle;
using RogueApeStudios.SecretsOfIgnacios.Services;
using System;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    [RequireComponent(typeof(AudioSource))]
    public class Cast : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpellManager _spellManager;
        [SerializeField] private ObjectPooler _pooler;
        [SerializeField] internal HandData _rightHandData;
        [SerializeField] internal HandData _leftHandData;
        [SerializeField] private HandVfxManager _handVfxManager;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _audioSourceWater;
        [SerializeField] private AudioSource _audioSourceEarth;

        [Header("Casting Mode")]
        [SerializeField] private float _castTimer = 0.75f;

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            SpellManager.onSpellValidation += HandleSpellValidation;

            _cancellationTokenSource = new CancellationTokenSource();
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
                    {
                        _audioSource.Play();
                        await _handVfxManager.ChargeEffect(handData, _castTimer);
                    }

                    ExecuteSpell(handData, config._castType);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Spell casting was canceled");
            }
        }

        private void ExecuteSpell(HandData handData, CastTypes castType)
        {
            switch (castType)
            {
                case CastTypes.SingleFire:
                    SingleCast(handData, false);
                    break;
                case CastTypes.Charged:
                    SingleCast(handData, false);
                    break;
                case CastTypes.Touch:
                    SingleCast(handData, true);
                    break;
                case CastTypes.Automatic:
                    RepeatedCast(handData);
                    break;
                default:
                    throw new NotImplementedException("Cast type not implemented.");
            }
        }

        private void SingleCast(HandData handData, bool isTouch)
        {
            GetSpellProjectile(handData);
            if (isTouch)
            {
                _audioSourceEarth.Play();
            }
            handData._renderer.materials[1].SetColor("_MainColor", handData._defaultColor);
            handData._renderer.material = handData._defaultMaterial;
            handData._canCast = false;

            _handVfxManager.DisableHandVFX(handData);
        }

        private async void RepeatedCast(HandData handData)
        {
            _audioSourceWater.Play();
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

            GameObject obj = _pooler.GetObject(spell._spellPrefab.name, spell._spellPrefab, handData._spellSpawnPoint);

            if (spell._castType is CastTypes.Touch)
            {
                obj.transform.SetParent(handData._spellSpawnPoint, false);
                obj.transform.SetLocalPositionAndRotation(spell._position, new(0, 0, 0, 0));
            }
        }

        private void StopCast(HandData handData)
        {
            if (_spellManager.CurrentSpell is not null && handData._isCasting)
            {
                _audioSourceWater.Stop();
                handData._isCasting = false;
                handData._canCast = false;
                handData._renderer.materials[1].SetColor("_MainColor", handData._defaultColor);
                handData._renderer.material = handData._defaultMaterial;

                _handVfxManager.DisableHandVFX(handData);
            }
        }

        private void HandleSpellValidation()
        {
            _rightHandData._canCast = true;
            _leftHandData._canCast = true;
        }

        public void CastNoHands(Transform transform, string objectType, GameObject spellPrefab)
        {
            GameObject projectile = _pooler.GetObject(objectType, spellPrefab, transform);
            projectile.transform.SetPositionAndRotation(transform.position, transform.localRotation);
        }
    }
}
