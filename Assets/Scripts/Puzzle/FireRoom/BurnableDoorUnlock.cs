using RogueApeStudios.SecretsOfIgnacios.Progression;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.FireRoom
{
    public class BurnableDoorUnlock : MonoBehaviour
    {
        //Drag in teleport plane (not the parent object, but 1 of the planes, the one behind the door basically)
        [SerializeField] private GameObject _areaToUnlock;

        private void OnDestroy()
        {
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
            }
        }
    }
}
