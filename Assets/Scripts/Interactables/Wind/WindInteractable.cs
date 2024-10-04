using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Wind
{
	internal abstract class WindInteractable : Interactables
	{
		[SerializeField] internal bool isBlown;

		internal override void Awake()
		{
			_spellReceiver.OnSpellReceived -= HandleSpellReceived;
		}

		internal override void OnDestroy()
		{
			_spellReceiver.OnSpellReceived -= HandleSpellReceived;
		}
		
		internal event Action<bool> onBlown; 

		internal override void HandleSpellReceived(string spellType)
		{
			Debug.Log("Some spell hit");
			switch (spellType)
			{
				case "Debug":
					// Logic for casting a fire spell
					break;
				case "Wind":
					// Implement the spinning of the fan. Boxes and things are not required, as the wind spell will have force
					break;
				default: throw new NotImplementedException();
			}
		}
		
		internal abstract void HandleBlown();
	}
}