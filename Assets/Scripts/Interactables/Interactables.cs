using RogueApeStudios.SecretsOfIgnacios.Puzzle;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables
{
    public abstract class Interactables : MonoBehaviour
    { 
        [SerializeField] internal SpellReceiver _spellReceiver;
        
        internal abstract void Awake();

        internal abstract void OnDestroy();

        internal abstract void HandleSpellReceived(string spellType);
    }
}
