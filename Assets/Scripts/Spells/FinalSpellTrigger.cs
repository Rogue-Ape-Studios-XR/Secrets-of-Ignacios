using System;
using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    
    //Add this to an empty object or just a new trigger collider of that big satan rune in the middle
    public class FinalSpellTrigger : MonoBehaviour
    {
        public static event Action<bool> onFinalSpellUnlockStateChange;

        [SerializeField] private List<string> _tags;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (_tags.Contains(other.tag))
            {
                onFinalSpellUnlockStateChange?.Invoke(true);
                Debug.Log("Unlocked the final spell");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_tags.Contains(other.tag))
            {
                onFinalSpellUnlockStateChange?.Invoke(false);
                Debug.Log("Player exited the area, locking the spell");
            }
        }
    }
}
