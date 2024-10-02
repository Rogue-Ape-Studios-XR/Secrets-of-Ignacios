namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Water
{
	internal abstract class WaterInteractable : Interactables
	{
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
		}
	}
}