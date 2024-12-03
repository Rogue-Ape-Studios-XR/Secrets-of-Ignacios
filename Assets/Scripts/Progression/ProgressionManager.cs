using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Progression
{
    public class ProgressionManager : MonoBehaviour
    {
        public static event Action<ProgressionData> OnProgressionEvent;

        public static void TriggerProgressionEvent(ProgressionData data)
        {
            Debug.Log($"ProgressionManager: Event triggered - {data.Type}");
            OnProgressionEvent?.Invoke(data);
        }
    }
}
