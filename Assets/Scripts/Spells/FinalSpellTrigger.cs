using System;
using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.FinalSpellPages;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    
    //Add this to an empty object or just a new trigger collider of that big satan rune in the middle
    public class FinalSpellTrigger : MonoBehaviour
    {
        public static event Action<bool> onFinalSpellUnlockStateChange;

        [SerializeField] private List<string> _tags;
        [SerializeField] private List<Spell> _finalSpells;
        // serializing the bool so you can just enable it for testing
        [SerializeField] private bool _allPagesCollected;

        private void Start()
        {
            FinalSpellPageCounter.onAllPagesCollected += HandleAllPagesCollected;
        }

        private void OnDestroy()
        {
            FinalSpellPageCounter.onAllPagesCollected -= HandleAllPagesCollected;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (_tags.Contains(other.tag) && _allPagesCollected)
            {
                onFinalSpellUnlockStateChange?.Invoke(true);
                foreach (var finalSpell in _finalSpells)
                {
                    if (finalSpell != null)
                    {
                        ProgressionData progressionData = new ProgressionData
                        {
                            Type = ProgressionType.SpellUnlock,
                            Data = new SpellUnlockData() { Spell = finalSpell }
                        };

                        ProgressionManager.TriggerProgressionEvent(progressionData);
                    }
                }
                Debug.Log("Unlocked the final spell");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_tags.Contains(other.tag) && _allPagesCollected)
            {
                onFinalSpellUnlockStateChange?.Invoke(false);
                foreach (var finalSpell in _finalSpells)
                {
                    if (finalSpell != null)
                    {
                        ProgressionData progressionData = new ProgressionData
                        {
                            Type = ProgressionType.SpellLock,
                            Data = new SpellLockData() { Spell = finalSpell }
                        };

                        ProgressionManager.TriggerProgressionEvent(progressionData);
                    }
                }
                Debug.Log("Player exited the area, locking the spell");
            }
        }

        private void HandleAllPagesCollected()
        {
            _allPagesCollected = true;
        }
    }
}
