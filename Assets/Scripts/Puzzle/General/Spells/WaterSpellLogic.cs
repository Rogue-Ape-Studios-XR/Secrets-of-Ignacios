using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class WaterSpellLogic : MonoBehaviour
    {
        [SerializeField] private float _speed = 1f;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private GameObject _waterVisual;
        [SerializeField] private VisualEffect _impactEffect;
        [SerializeField] private Collider _collider;


        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /*
         Spell should:
        -Have a certain velocity (gravity is set in rigidbody)
        -Play VFX upon impact
        -Disable self upon impact
         */

        private void OnEnable()
        {
            _rb.linearVelocity = transform.forward * _speed;
            _impactEffect.Stop();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async void DestroyAfterDone(CancellationToken token)
        {
            try
            {
                await UniTask.WaitForSeconds(5, cancellationToken: token);
                Destroy(gameObject);
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Destruction was Canceled...");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //make the effect rotate to match the normal of the impact
            _impactEffect.transform.LookAt(_impactEffect.transform.position + collision.contacts[0].normal);
            _impactEffect.transform.RotateAround(_impactEffect.transform.position, _impactEffect.transform.right, 90);
            _impactEffect.Play();
            _rb.isKinematic = true;
            _collider.enabled = false;
            _waterVisual.SetActive(false);
            DestroyAfterDone(_cancellationTokenSource.Token);
        }
    }
}
