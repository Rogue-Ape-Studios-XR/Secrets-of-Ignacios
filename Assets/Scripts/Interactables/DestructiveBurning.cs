using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Interactables;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.VFX;
using NUnit.Framework;
using System.Collections.Generic;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables
{
    internal class DestructiveBurning : Flammability
    {
        private CancellationTokenSource _cancellationTokenSource;
        [SerializeField] private float burnTime = 1;
        [SerializeField] private VisualEffect _destructionEffect;

        [SerializeField] private List<Collider> _colliders;
        [SerializeField] private List<GameObject> _containedObjects;

        private void Start()
        {
            base._burningEffect.Stop();
            _destructionEffect.Stop();
        }

        internal override void Awake()
        {
            base.Awake();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        internal override void OnDestroy()
        {
            base.OnDestroy();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async void Burning(CancellationToken token)
        {
            try
            {
                // Do stuff before timer starts
                base._burningEffect.Play();
                // WaitForSeconds is used only as an example
                await UniTask.WaitForSeconds(burnTime, cancellationToken: token);
                // Do stuff after timer is finished
                OnBurnt();
            }
            catch (OperationCanceledException)
            {
                //Debug.LogError("Example was Canceled...");
                Debug.Log("This happens on douse");
            }
        }

        internal override void OnFire()
        {
            base._isOnFire = true;
            Debug.Log("heb ik tekst?");
            Burning(_cancellationTokenSource.Token);
        }

        private void OnBurnt()
        {
            Debug.Log("burn!");

            base._burningEffect.Stop();
            _destructionEffect.Play();
            _destructionEffect.gameObject.transform.parent = null;
            _burningEffect.gameObject.transform.parent = null;
            foreach (var collider in _colliders)
            {
                collider.enabled = false;
            }
            foreach (var item in _containedObjects)
            {
                GameObject obj = Instantiate(item, transform);
                obj.transform.parent = transform.parent;
            }
            gameObject.SetActive(false);
        }
    }
}
