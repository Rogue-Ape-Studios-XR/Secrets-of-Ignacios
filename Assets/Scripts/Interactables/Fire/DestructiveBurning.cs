using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Fire
{
	internal class DestructiveBurning : FireInteractable
	{
		[Header("Burn Settings")] [SerializeField]
		private float _burnTime = 1;

		[Header("Visual Effects")] [SerializeField]
		private VisualEffect _destructionEffect;

		[Header("Spawn Objects")] [SerializeField]
		private bool _spawnObjectOnBurnt;

		[SerializeField] private List<GameObject> _containedObjects;

		private CancellationTokenSource _cancellationTokenSource;

		internal override void Awake()
		{
			base.Awake();
			_cancellationTokenSource = new();
		}

		private void Start()
		{
			_burningEffect.Stop();
			//_dousingEffect.Stop();
			_destructionEffect.Stop();
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
				await UniTask.WaitForSeconds(_burnTime, cancellationToken: token);
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

        internal override void OnDouse()
        {
            _isOnFire = false;
			_cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new();
            _burningEffect.Stop();
            //_dousingEffect.Play();
        }
    }
}