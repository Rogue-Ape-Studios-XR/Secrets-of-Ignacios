using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Fire
{
    internal class PersistentFire : Flammability
    {
        [SerializeField] private bool _startsOnFire;

        private void Start()
        {
            _burningEffect.Stop();
            _getsDestroyed = false;

            if (_startsOnFire)
            {
                OnFire();
            }
        }

        internal override void OnFire()
        {
            _isOnFire = true;
            _burningEffect.Play();
        }
    }
}
