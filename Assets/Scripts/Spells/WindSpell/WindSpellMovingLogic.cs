using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Services;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Spells.WindSpell
{
    public class WindSpellMovingLogic : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Rigidbody _rb;

        [SerializeField] private Collider _collider;

        [Header("Visual")][SerializeField] private GameObject _visual;
        [SerializeField] private VisualEffect _impactEffect;
        [SerializeField] private bool _impactShouldRotate = false;
        [SerializeField] private Transform _impactEffectTransform;

        [Header("Projectile Settings")]
        [SerializeField]
        private ElementType _pool;

        [SerializeField] private float _speed = 1f;

        [Header("Ignore Layers")]
        [SerializeField]
        private LayerMask _ignoreLayers;


        private ObjectPooler _objectPooler;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _collided = false;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            ResetProjectile(true);
            _rb.linearVelocity = transform.forward * _speed;
            ReturnAfterTime(5, _cancellationTokenSource.Token);
        }

        private void Start()
        {
            _objectPooler = ServiceLocator.GetService<ObjectPooler>();
        }

        /*
		 Spell should:
		-Have a certain velocity (gravity is set in rigidbody)
		-Play VFX upon impact
		-Disable self upon impact
		 */

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async void ReturnAfterDone(CancellationToken token)
        {
            try
            {
                ResetProjectile(false);
                _visual.SetActive(false);

                await UniTask.WaitUntil(() => _impactEffect.aliveParticleCount == 0, cancellationToken: token);

                _objectPooler.ReturnObject(_pool.ToString(), gameObject);
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Return of water projectile to pool was canceled");
            }
        }

        private void ResetProjectile(bool enabled)
        {
            if (enabled)
            {
                _impactEffect.Reinit();
                _impactEffect.Stop();
                _visual.SetActive(enabled);
            }
            else
            {
                _visual.SetActive(!enabled);
                _impactEffect.Play();
            }

            _collided = false;
            _collider.enabled = enabled;
            _rb.isKinematic = !enabled;
        }

        private async void ReturnAfterTime(float time, CancellationToken token)
        {
            await UniTask.WaitForSeconds(time, cancellationToken: token);

            if (!_collided && gameObject.activeSelf)
                _objectPooler.ReturnObject(gameObject.name, gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((_ignoreLayers & (1 << other.gameObject.layer)) != 0) return;

            if (!other.TryGetComponent(out Rigidbody rb))
            {
                _collided = true;

                if (_impactEffectTransform != null && _impactShouldRotate)
                {
                    Vector3 closestPoint = other.ClosestPoint(transform.position);
                    Vector3 impactNormal = (transform.position - closestPoint).normalized;

                    _impactEffectTransform.LookAt(_impactEffectTransform.position + impactNormal);
                    _impactEffectTransform.RotateAround(_impactEffectTransform.position, _impactEffectTransform.right, 90);
                }

                ReturnAfterDone(_cancellationTokenSource.Token);
            }
        }

    }
}