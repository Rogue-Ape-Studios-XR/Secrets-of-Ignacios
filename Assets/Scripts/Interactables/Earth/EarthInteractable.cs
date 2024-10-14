using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Earth
{
	internal abstract class EarthInteractable : Interactables
	{
		[SerializeField] internal bool _isTouched;
		internal event Action<bool> onTouched;
		
		internal override void Awake()
		{
			_spellReceiver.OnSpellReceived += HandleSpellReceived;
		}

		internal override void OnDestroy()
		{
			_spellReceiver.OnSpellReceived -= HandleSpellReceived;
		}

		internal override void HandleSpellReceived(string spellType)
		{
			Debug.Log("Earth spell hit");

			switch (spellType)
			{
				case "Debug":
					Touched();
					onTouched?.Invoke(_isTouched);
					break;	
				case "Earth":
					Touched();
					onTouched?.Invoke(_isTouched);
					break;
			}
		}

		internal abstract void Touched();
	}
}