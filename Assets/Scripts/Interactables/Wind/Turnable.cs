using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Wind
{
	internal class Turnable : WindInteractable
	{
		[SerializeField] private float _turnDuration = 1f;
		[SerializeField] private Transform _targetTransform;
		[SerializeField] private Vector3 _rotationDegrees;
		private float _elapsedTime;
		private Quaternion _startRotation;
		private Quaternion _targetRotation;

		// No need to override the awake and ondestroy, as theres no Unitask, so they will simply do the base implementation
		private void FixedUpdate()
		{
			if (_isBlown) Turn();
		}

		internal override void Blown()
		{
			// If there are any issues, you can wrap this in an if (!_isBlown).
			// This is not done so if the player casts multiple spells, it won't stop with the logic of only 1
			Debug.Log("hit the turning thingamajig");
			_isBlown = true;

			_startRotation = _targetTransform.rotation;
			_targetRotation = _startRotation * Quaternion.Euler(_rotationDegrees);
			_elapsedTime = 0f;

			Debug.Log("object is turning");
		}

		private void Turn()
		{
			_elapsedTime += Time.deltaTime;
			var t = Mathf.Clamp01(_elapsedTime / _turnDuration);

			_targetTransform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, t);

			if (t >= 1f) _isBlown = false;
		}
	}
}