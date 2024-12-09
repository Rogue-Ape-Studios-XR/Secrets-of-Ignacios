using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Progression.AreaProgression
{
    public class AreaProgressionManager : MonoBehaviour
    {
        private void OnEnable()
        {
            ProgressionManager.OnProgressionEvent += HandleProgressionEvent;
        }

        private void OnDisable()
        {
            ProgressionManager.OnProgressionEvent -= HandleProgressionEvent;
        }

        private void HandleProgressionEvent(ProgressionData data)
        {
            if (data.Type == ProgressionType.AreaUnlock && data.Data is AreaUnlockData areaData)
            {
                if (areaData.Area != null)
                {
                    Debug.Log($"area {areaData.Area.name} has been unlocked");
                    areaData.Area.SetActive(true);
                }
            }
        }
    }
}
