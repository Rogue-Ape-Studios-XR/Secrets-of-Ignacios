using RogueApeStudios.SecretsOfIgnacios.Puzzle;
using System;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables
{
    internal abstract class Flammability : MonoBehaviour
    {
        [SerializeField] internal bool _getsDestroyed;
        [SerializeField] internal bool _isOnFire;
        [SerializeField] internal SpellReceiver _spellReceiver;

        [SerializeField] internal VisualEffect _burningEffect;

        internal event Action<bool> OnIgnitionToggle;

        internal virtual void Awake()
        {
            _spellReceiver.OnSpellReceived += HandleSpellReceived;
        }

        internal virtual void OnDestroy()
        {
            _spellReceiver.OnSpellReceived -= HandleSpellReceived;
        }

        private void HandleSpellReceived(string spellType)
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
