using System;
using System.Threading;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Water
{
	internal abstract class WaterInteractable : Interactables
	{
		[SerializeField] internal bool _isSplashed;
        [SerializeField] internal float _spellInterval = 0.5f;

        internal CancellationTokenSource _cancellationTokenSource;

        internal event Action<bool> OnSplashToggle;

        internal override void Awake()
		{
			_spellReceiver.OnSpellReceived += HandleSpellReceived;
            _cancellationTokenSource = new CancellationTokenSource();
        }

		internal override void OnDestroy()
		{
			_spellReceiver.OnSpellReceived -= HandleSpellReceived;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

		internal override void HandleSpellReceived(string spellType)
		{
            Debug.Log("Some water spell hit");
            switch (spellType)
            {
                case "Water":
					// Logic for casting a water spell
					Splashed();
                    OnSplashToggle?.Invoke(_isSplashed);
                    break;

                default: throw new NotImplementedException();
            }
        }

		internal abstract void Splashed();
	}
}