using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Wind
{
	internal class Turnable : WindInteractable
	{
		[SerializeField] private bool _isTurning;
		[SerializeField] private float _turnDuration = 1f;
		[SerializeField] private Transform _targetTransform;
		[SerializeField] private Vector3 _rotationDegrees;
		private CancellationTokenSource _cancellationTokenSource;

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

		private async void Turn(CancellationToken token)
		{
			try
			{
				var startRotation = _targetTransform.rotation;

				//set a custom degrees in the editor so it will take x amount of seconds to reach this rotation
				var targetRotation = Quaternion.Euler(_rotationDegrees);
				var elaspedTime = 0f;

				while (elaspedTime < _turnDuration)
				{
					if (token.IsCancellationRequested) throw new OperationCanceledException();
					elaspedTime += Time.deltaTime;
					var t = Mathf.Clamp01(elaspedTime / _turnDuration);
					_targetTransform.rotation = Quaternion.Slerp(startRotation, _targetTransform.rotation, t);

					await UniTask.Yield(token);
				}

				_targetTransform.rotation = targetRotation;
				_isTurning = false;
				_isBlown = false;
			}
			catch (OperationCanceledException)
			{
				Debug.LogError("turning thingamajing is canceled");
			}
		}

		internal override void Blown()
		{
			Debug.Log("hit the turning thingamajing");
			_isBlown = true;
			_isTurning = true;
			Turn(_cancellationTokenSource.Token);
			Debug.Log("Object is turning");
		}
	}
}