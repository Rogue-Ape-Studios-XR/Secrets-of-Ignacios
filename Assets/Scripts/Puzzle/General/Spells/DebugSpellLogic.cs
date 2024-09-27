using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class DebugSpellLogic : MonoBehaviour
    {

        [SerializeField] private float _speed = 1f;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private VisualEffect _impactEffect;
        [SerializeField] private VisualEffect _spellEffect;
        [SerializeField] private Collider _collider;


        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
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



        /*
         Spell should:
        -Have a certain velocity (gravity is set in rigidbody)
        -Play VFX upon impact
        -Disable self upon impact
         */

        private void OnEnable()
        {
            _rb.AddForce(gameObject.transform.forward*_speed);
        }
        // Update is called once per frame
        void FixedUpdate()
        {
            //move the projectile
            //rb.linearVelocity = gameObject.transform.forward * speed;
        }

        private void OnCollisionEnter(Collision collision)
        {

            _impactEffect.enabled = true;
            _spellEffect.Stop();
            _rb.isKinematic = true;
            _collider.enabled = false;
            DestroyAfterDone(_cancellationTokenSource.Token); 
            //gameObject.SetActive(false);
        }

    }
}
