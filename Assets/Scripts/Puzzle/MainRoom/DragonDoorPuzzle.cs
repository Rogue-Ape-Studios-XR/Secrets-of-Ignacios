using RogueApeStudios.SecretsOfIgnacios.Progression;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.MainRoom
{
    public class DragonDoorPuzzle : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _areaToUnlock;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Potion"))
            {
                other.gameObject.SetActive(false);
                gameObject.SetActive(false);
                _animator.SetTrigger("DubbleIn");
                
                
            if (_areaToUnlock != null)
            {
                ProgressionData progressionData = new ProgressionData
                {
                    Type = ProgressionType.AreaUnlock,
                    Data = new AreaUnlockData { Area = _areaToUnlock }
                };

                ProgressionManager.TriggerProgressionEvent(progressionData);
            }
            else
            {
                Debug.LogError("No area assigned to unlock");
            }            }
        }
    }
}
