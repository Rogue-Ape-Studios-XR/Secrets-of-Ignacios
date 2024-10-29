using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Fire
{
	internal class PersistentFire : FireInteractable
	{
		[SerializeField] private bool _startsOnFire;

		private void Start()
		{
			_burningEffect.Stop();
			//_dousingEffect.Stop();
			_getsDestroyed = false;

			if (_startsOnFire) OnFire();
		}

		internal override void OnFire()
		{
			_isOnFire = true;
			_burningEffect.Play();
		}

        internal override void OnDouse()
        {
            _isOnFire = false;
			_burningEffect.Stop();
			//_dousingEffect.Play();
        }
    }
}