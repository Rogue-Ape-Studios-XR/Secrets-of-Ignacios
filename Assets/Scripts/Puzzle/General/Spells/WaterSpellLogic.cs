using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class WaterSpellLogic : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }


        [SerializeField] private float _speed = 1f;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private GameObject _waterVisual;
        [SerializeField] private VisualEffect _impactEffect;
        //[SerializeField] private VisualEffect _spellEffect;
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


        // Update is called once per frame
        void FixedUpdate()
        {
            //move the projectile
            //rb.linearVelocity = gameObject.transform.forward * speed;
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


            /* official unity debug code 
            // Print how many points are colliding with this transform
            Debug.Log("Points colliding: " + collision.contacts.Length);

            // Print the normal of the first point in the collision.
            Debug.Log("Normal of the first point: " + collision.contacts[0].normal);

            // Draw a different colored ray for every normal in the collision
            foreach (var item in collision.contacts)
            {
                Debug.DrawRay(item.point, item.normal * 100, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 1000f);
            }
            */

            //make the effect rotate to match the normal of the impact
            _impactEffect.transform.LookAt(_impactEffect.transform.position + collision.contacts[0].normal);
            _impactEffect.transform.RotateAround(_impactEffect.transform.position, _impactEffect.transform.right, 90);
            //_impactEffect.transform.RotateAround(transform.position, transform.up, -90);

            _impactEffect.Play();
            //_spellEffect.Stop();
            _rb.isKinematic = true;
            _collider.enabled = false;
            _waterVisual.SetActive(false);
            DestroyAfterDone(_cancellationTokenSource.Token);
        }
    }
}
