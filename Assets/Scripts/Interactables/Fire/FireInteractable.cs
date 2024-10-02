using RogueApeStudios.SecretsOfIgnacios.Puzzle;
using System;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Fire
{
    internal abstract class Flammability : Interactables 
    {
        [SerializeField] internal bool _getsDestroyed;
        [SerializeField] internal bool _isOnFire;
        
        [SerializeField] internal VisualEffect _burningEffect;

        internal event Action<bool> OnIgnitionToggle;

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
