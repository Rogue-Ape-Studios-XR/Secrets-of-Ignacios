using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables
{
    internal class PersistentFire : Flammability
    {
        [SerializeField] private bool _startsOnFire;

        private void Start()
        {
            base._burningEffect.Stop();
            base._getsDestroyed = false;

            if (_startsOnFire)
            {
                OnFire();
            }
        }

        internal override void OnFire()
        {
            base._isOnFire = true;
            base._burningEffect.Play();
        }
    }
}
