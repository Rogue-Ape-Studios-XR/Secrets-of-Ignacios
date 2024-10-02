namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Wind
{
	internal abstract class WindInteractable : Interactables
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