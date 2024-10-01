using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Fire
{
    internal class DestructiveBurning : Flammability
    {
        [Header("Burn Settings")]
        [SerializeField] private float burnTime = 1;

        [Header("Visual Effects")]
        [SerializeField] private VisualEffect _destructionEffect;

        [Header("Spawn Objects")]
        [SerializeField] private bool _spawnObjectOnBurnt = false;
        [SerializeField] private List<GameObject> _containedObjects;

        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _burningEffect.Stop();
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
                _burningEffect.Play();
                await UniTask.WaitForSeconds(burnTime, cancellationToken: token);
                OnBurnt();
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Burning was Canceled");
            }
        }

        internal override void OnFire()
        {
            _isOnFire = true;
            Burning(_cancellationTokenSource.Token);
        }

        private void OnBurnt()
        {
            _burningEffect.Stop();
            _destructionEffect.Play();

            if (_spawnObjectOnBurnt)
                foreach (var item in _containedObjects)
                    item.SetActive(true);

            gameObject.SetActive(false);
        }
    }
}