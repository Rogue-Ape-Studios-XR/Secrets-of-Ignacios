using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Services;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells.WindSpell
{
    public class WindForceSpell : MonoBehaviour
    {
        [Header("Wind Force Settings")] 
        [SerializeField] private float _force = 10f;
        [SerializeField] private float _affectRadius = 10f;
        //This is how wide the cone will be, 180 would be fully around you
        [SerializeField] private float _angle = 45f;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private ElementType _pool;
        
        [Header("Visual")] 
        //Currently disabled, will implement this later
        // [SerializeField] private VisualEffect _windEffect;
        private ObjectPooler _objectPooler;
        private CancellationTokenSource _cancellationTokenSource;
        
        private void Start()
        {
            _objectPooler = ServiceLocator.GetService<ObjectPooler>();
        }

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            CastWindForce();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            //Debug draw, could keep it if you want to
            Gizmos.color = Color.green;
            var forward = transform.forward * _affectRadius;

            var leftBoundary = Quaternion.Euler(0, -_angle, 0) * forward;
            var rightBoundary = Quaternion.Euler(0, _angle, 0) * forward;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

            const int segmentCount = 20;
            for (var i = 0; i < segmentCount; i++)
            {
                var currentAngle = -_angle + 2 * _angle * i / (segmentCount - 1);
                var direction = Quaternion.Euler(0, currentAngle, 0) * forward;
                var nextDirection = Quaternion.Euler(0, currentAngle + 2 * _angle / segmentCount, 0) * forward;
                Gizmos.DrawLine(transform.position + direction, transform.position + nextDirection);
            }
        }
        
        private void CastWindForce()
        {
            ApplyWindForce(_cancellationTokenSource.Token).Forget();
        }

        private async UniTask ApplyWindForce(CancellationToken cancellationToken)
        {
            var elapsedTime = 0f;
            // _windEffect?.Play();

            while (elapsedTime < _duration)
            {
                if (cancellationToken.IsCancellationRequested) return;

                var colliders = Physics.OverlapSphere(transform.position, _affectRadius);
                foreach (var hit in colliders)
                {
                    if (hit.gameObject == gameObject) continue;

                    if (hit.TryGetComponent(out Rigidbody rb))
                    {
                        var direction = (hit.transform.position - transform.position).normalized;
                        if (Vector3.Angle(transform.forward, direction) <= _angle)
                        {
                            var distance = Vector3.Distance(transform.position, hit.transform.position);
                            var forceMagnitude = _force * (1 - distance / _affectRadius);
                            rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                        }
                    }
                }

                elapsedTime += Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate(cancellationToken);
            }

            // _windEffect?.Stop();
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _objectPooler.ReturnProjectile(_pool.ToString(), gameObject);
        }
    }
}
