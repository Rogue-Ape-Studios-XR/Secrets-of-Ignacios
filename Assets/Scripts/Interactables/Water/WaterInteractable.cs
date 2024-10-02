using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Water
{
	internal abstract class WaterInteractable : Interactables
	{
		[SerializeField] internal bool _isSplashed;

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
            Debug.Log("Some water spell hit");
            switch (spellType)
            {
                case "Debug":
					// Logic for casting a water spell
					Splashed();
                    break;

                default: throw new NotImplementedException();
            }
        }

		internal abstract void Splashed();
	}
}