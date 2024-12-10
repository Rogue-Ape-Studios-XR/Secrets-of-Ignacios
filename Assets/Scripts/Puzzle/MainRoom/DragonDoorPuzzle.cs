using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.MainRoom
{
    public class DragonDoorPuzzle : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private List<GameObject> _areasToUnlock;
        [SerializeField] private Spell _spellToUnlock;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Potion"))
            {
                other.gameObject.SetActive(false);
                gameObject.SetActive(false);
                _animator.SetTrigger("DubbleIn");
                
                UnlockAreas();
                //Wind spell
                UnlockSpell();
            }
        }
        
        private void UnlockAreas()
        {
            foreach (var area in _areasToUnlock)
            {
                if (area != null)
                {
                    ProgressionData progressionData = new ProgressionData
                    {
                        Type = ProgressionType.AreaUnlock,
                        Data = new AreaUnlockData { Area = area }
                    };

                    ProgressionManager.TriggerProgressionEvent(progressionData);
                }
            }
        }

        private void UnlockSpell()
        {
           if (_spellToUnlock != null)
            {
                ProgressionData progressionData = new ProgressionData
                {
                    Type = ProgressionType.SpellUnlock,
                    Data = new SpellUnlockData() { Spell = _spellToUnlock }
                };

                ProgressionManager.TriggerProgressionEvent(progressionData);
            }
        }
    }
}
