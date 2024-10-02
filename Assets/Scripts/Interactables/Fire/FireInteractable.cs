using System;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Fire
{
	internal abstract class FireInteractable : Interactables
	{
		[SerializeField] internal bool _getsDestroyed;
		[SerializeField] internal bool _isOnFire;

		[SerializeField] internal VisualEffect _burningEffect;

		internal override void Awake()
		{
			_spellReceiver.OnSpellReceived += HandleSpellReceived;
		}

		internal override void OnDestroy()
		{
			_spellReceiver.OnSpellReceived -= HandleSpellReceived;
		}

		internal event Action<bool> OnIgnitionToggle;

		internal override void HandleSpellReceived(string spellType)
		{
			Debug.Log("Some spell hit");
			switch (spellType)
			{
				case "Debug":
					// Logic for casting a fire spell
					OnFire();
					OnIgnitionToggle?.Invoke(_isOnFire);
					break;
				/*case SpellType.Water:
				    // Logic for casting a water spell
				    Debug.Log("Casting a water spell!");
				    break;*/
				default: throw new NotImplementedException();
			}
		}

		internal abstract void OnFire();
	}
}