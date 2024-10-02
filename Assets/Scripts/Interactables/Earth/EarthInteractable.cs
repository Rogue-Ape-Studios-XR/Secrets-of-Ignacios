namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Earth
{
	internal abstract class EarthInteractable : Interactables
	{
		internal override void Awake()
		{
			_spellReceiver.OnSpellReceived -= HandleSpellReceived;
		}

		internal override void OnDestroy()
		{
			_spellReceiver.OnSpellReceived -= HandleSpellReceived;
		}

		internal override void HandleSpellReceived(string spellType)
		{
		}
	}
}