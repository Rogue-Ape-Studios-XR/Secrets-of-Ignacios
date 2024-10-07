using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Wind
{
	internal class Blowable : WindInteractable
	{
		// Duration something stays in the blown state
		[SerializeField] private float _blownDuration = 1f;

		// Bool to decide if something stays blown or not.
		[SerializeField] private bool _temporaryBlown;

		private float _elapsedTime;
		// Event to indicate an object is hit with the wind spell, thus blown :)
		internal event Action<bool> onObjectBlown; 

		private void FixedUpdate()
		{
			if (_isBlown && _temporaryBlown)
			{
				_elapsedTime += Time.fixedDeltaTime;
				if (_elapsedTime >= _blownDuration) EndBlown();
			}
		}

		internal override void Blown()
		{
			_isBlown = true;
			onObjectBlown?.Invoke(_isBlown);
			_elapsedTime = 0f;
		}

		private void EndBlown()
		{
			_isBlown = false;
			onObjectBlown?.Invoke(_isBlown);
		}
	}
}