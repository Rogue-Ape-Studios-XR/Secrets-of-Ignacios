using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Services;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells.WindSpell
{
    public class RangedWindSpellLogic : MonoBehaviour
    {
        [Header("Wind Force Settings")]
        [SerializeField] private float _force = 10f;
        [SerializeField] private float _affectRadius = 10f;
        [SerializeField] private float _angle = 45f;
        [SerializeField] private ElementType _pool;

        [Header("Ignore Layers")]
        [SerializeField] private LayerMask _ignoreLayers; // Layers to ignore

        [Header("Visual")]
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
            while (!cancellationToken.IsCancellationRequested)
            {
                var colliders = Physics.OverlapSphere(transform.position, _affectRadius);
                foreach (var hit in colliders)
                {
                    if (hit.gameObject == gameObject || IsIgnoredLayer(hit.gameObject)) continue;

                    if (hit.TryGetComponent(out Rigidbody rb))
                    {
                        var direction = (hit.transform.position - transform.position).normalized;

                        float angleBetween = Vector3.Angle(transform.forward, direction);

                        if (angleBetween <= _angle / 2)
                        {
                            var distance = Vector3.Distance(transform.position, hit.transform.position);

                            var forceMagnitude = Mathf.Max(0f, _force * (1 - distance / _affectRadius));

                            rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                        }
                    }
                }

                await UniTask.WaitForFixedUpdate(cancellationToken);
            }

            ReturnToPool();
        }
       

        private bool IsIgnoredLayer(GameObject obj)
        {
            return (_ignoreLayers & (1 << obj.layer)) != 0;
        }

        private void ReturnToPool()
        {
            if (gameObject.activeSelf)
                _objectPooler.ReturnObject(gameObject.name, gameObject);
        }
    }
}
