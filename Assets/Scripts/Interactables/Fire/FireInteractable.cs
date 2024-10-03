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
		[SerializeField] internal VisualEffect _dousingEffect;

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
			Debug.Log("Some fire spell hit");
			switch (spellType)
			{
				case "Debug":
					// Logic for casting a fire spell
					OnFire();
					OnIgnitionToggle?.Invoke(_isOnFire);
					break;
				case "Water":
					// Logic for casting a water spell
					OnDouse();
                    OnIgnitionToggle?.Invoke(!_isOnFire);
                    break;
				default: throw new NotImplementedException();
			}
		}

		internal abstract void OnFire();

		internal abstract void OnDouse();
	}
}