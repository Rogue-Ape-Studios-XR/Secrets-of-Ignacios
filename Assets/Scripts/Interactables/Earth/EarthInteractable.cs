using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Earth
{
	internal abstract class EarthInteractable : Interactables
	{
		[SerializeField] internal bool _isGrowSpellActive;
		[SerializeField] internal bool _isShrinkSpellActive;

		internal override void Awake()
		{
			_spellReceiver.OnSpellReceived += HandleSpellReceived;
		}

		internal override void OnDestroy()
		{
			_spellReceiver.OnSpellReceived -= HandleSpellReceived;
		}

		//Basically serves as the oncollisionenter as this is an empty gameobject 
		internal override void HandleSpellReceived(string spellType)
		{
			Debug.Log("Earth spell hit");

			switch (spellType)
			{
				case "Debug":
					Touched();
					//Use the editor for this
					//Select the boolean isgrowspellactive or isshrinkspellactive before it hits!
					break;
				case "Grow":
					_isGrowSpellActive = true;
					Touched();
					break;
				case "Shrink":
					_isShrinkSpellActive = true;
					Touched();
					break;
			}
		}

		internal abstract void Touched();
	}
}