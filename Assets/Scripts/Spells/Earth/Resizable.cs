using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Services;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Spells.Earth
{
    public class Resizable : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Collider _collider;

        [Header("Visual")]
        [SerializeField] private VisualEffect _impactEffect;

        [Header("Spell Settings")]
        [SerializeField] private float _deactivationTime = 3f;

        private ObjectPooler _objectPooler;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _collided = false;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        void Start()
        {
            _objectPooler = ServiceLocator.GetService<ObjectPooler>();
        }

        private void OnEnable()
        {
            ResetSpell(true);
            ReturnAfterTime(_cancellationTokenSource.Token);
            transform.position = new(0, 0, 0.76f);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private async void ReturnAfterDone(CancellationToken token)
        {
            try
            {
                ResetSpell(false);

                await UniTask.WaitUntil(() => _impactEffect.aliveParticleCount == 0, cancellationToken: token);

                _objectPooler.ReturnObject(gameObject.name, gameObject);
            }
            catch (OperationCanceledException)
            {
                Debug.LogError($"Return of {gameObject.name} to pool was canceled");
            }
        }

        private void ResetSpell(bool enabled)
        {
            if (enabled)
            {
                _impactEffect.Reinit();
                _impactEffect.Stop();
            }
            else
                _impactEffect.Play();

            _collided = false;
            _collider.enabled = enabled;
        }

        private async void ReturnAfterTime(CancellationToken token)
        {
            await UniTask.WaitForSeconds(_deactivationTime, cancellationToken: token);

            if (!_collided && gameObject.activeSelf)
            {
                ResetSpell(true);
                _objectPooler.ReturnObject(gameObject.name, gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _collided = true;
            ReturnAfterDone(_cancellationTokenSource.Token);
        }
    }
}
