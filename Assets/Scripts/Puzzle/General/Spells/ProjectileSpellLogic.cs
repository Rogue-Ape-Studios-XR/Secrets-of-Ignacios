using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Services;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.General.Spells
{
    public class ProjectileSpellLogic : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;

        [Header("Visual")]
        [SerializeField] private GameObject _visual;
        [SerializeField] private VisualEffect _impactEffect;
        [SerializeField] private bool _impactShouldRotate = false;
        [SerializeField] private Transform _impactEffectTransform;

        [Header("Projectile Settings")]
        [SerializeField] private ElementType _pool;
        [SerializeField] private float _speed = 1f;

        private ObjectPooler _objectPooler;
        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
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

        private void OnEnable()
        {
            ResetProjectile(true);
            _rb.linearVelocity = transform.forward * _speed;
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async void ReturnAfterDone(CancellationToken token, bool collided)
        {
            try
            {
                ResetProjectile(false);
                _visual.SetActive(false);

                if (collided)
                    await UniTask.WaitUntil(() => _impactEffect.aliveParticleCount == 0, cancellationToken: token);
                else
                    await UniTask.WaitForSeconds(5, cancellationToken: token);

                _objectPooler.ReturnProjectile(_pool.ToString(), gameObject);
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

            _collider.enabled = enabled;
            _rb.isKinematic = !enabled;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_impactEffectTransform is not null && _impactShouldRotate)
            {
                //make the effect rotate to match the normal of the impact
                _impactEffectTransform.LookAt(_impactEffect.transform.position + collision.contacts[0].normal);
                _impactEffectTransform.RotateAround(_impactEffect.transform.position, _impactEffect.transform.right, 90);
            }

            ReturnAfterDone(_cancellationTokenSource.Token, true);
        }
    }
}
