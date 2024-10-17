using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Wind
{
	internal abstract class WindInteractable : Interactables
	{
		[SerializeField] internal bool _isBlown;

		internal event Action<bool> onBlown;
		
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
			Debug.Log("Some wind spell hit");
			switch (spellType)
			{
				case "Debug":
					Blown();
					onBlown?.Invoke(_isBlown);
					break;
				case "Wind":
					// Implement the spinning of the fan. Boxes and things are not required, as the wind spell will have force
					Blown();
					onBlown?.Invoke(_isBlown);
					break;
				default: throw new NotImplementedException();
			}
		}

		internal abstract void Blown();
	}
}