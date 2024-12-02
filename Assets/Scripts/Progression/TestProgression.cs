using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Progression
{
    public class TestProgression : MonoBehaviour
    {
        [Header("Progression Type")] [SerializeField]
        private ProgressionType _progressionType;

        [Header("Spell Unlock Data")] [SerializeField]
        private Spell _spell;

        [Header("Area Unlock Data")] [SerializeField]
        private GameObject _area; 

        [Header("Trigger Event")] [SerializeField]
        private bool triggerEvent;

        private void OnValidate()
        {
            if (triggerEvent)
            {
                TriggerProgressionEvent();
                triggerEvent = false;
            }
        }

        private void TriggerProgressionEvent()
        {
            ProgressionData data = new ProgressionData { Type = _progressionType };

            switch (_progressionType)
            {
                case ProgressionType.SpellUnlock:
                    if (_spell != null)
                    {
                        data.Data = new SpellUnlockData { Spell = _spell };
                    }
                    else
                    {
                        Debug.LogError("No spell set for SpellUnlock progression.");
                    }
                    break;
            }

            ProgressionManager.TriggerProgressionEvent(data);
        }
    }
}