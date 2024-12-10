using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.MainRoom
{
    public class DragonDoorPuzzle : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private List<GameObject> _areasToUnlock;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Potion"))
            {
                other.gameObject.SetActive(false);
                gameObject.SetActive(false);
                _animator.SetTrigger("DubbleIn");
                
                UnlockAreas();
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
    }
}
