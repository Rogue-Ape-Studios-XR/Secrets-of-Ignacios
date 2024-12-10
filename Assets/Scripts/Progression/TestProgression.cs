using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;
using System.Collections.Generic;

namespace RogueApeStudios.SecretsOfIgnacios.Progression
{
    public class TestProgression : MonoBehaviour
    {
        [Header("Progression Type")]
        [SerializeField] private ProgressionType _progressionType;

        [Header("Spell Unlock Data")]
        [SerializeField] private List<Spell> _spells;

        [Header("Area Unlock Data")]
        [SerializeField] private List<GameObject> _areas;

        [Header("Trigger Event")]
        [SerializeField] private bool triggerEvent;

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
            switch (_progressionType)
            {
                case ProgressionType.SpellUnlock:
                    if (_spells != null && _spells.Count > 0)
                    {
                        foreach (var spell in _spells)
                        {
                            if (spell != null)
                            {
                                ProgressionData data = new ProgressionData
                                {
                                    Type = ProgressionType.SpellUnlock,
                                    Data = new SpellUnlockData { Spell = spell }
                                };
                                ProgressionManager.TriggerProgressionEvent(data);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("No spells assigned for SpellUnlock progression.");
                    }
                    break;

                case ProgressionType.AreaUnlock:
                    if (_areas != null && _areas.Count > 0)
                    {
                        foreach (var area in _areas)
                        {
                            if (area != null)
                            {
                                ProgressionData data = new ProgressionData
                                {
                                    Type = ProgressionType.AreaUnlock,
                                    Data = new AreaUnlockData { Area = area }
                                };
                                ProgressionManager.TriggerProgressionEvent(data);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("No areas assigned for AreaUnlock progression.");
                    }
                    break;
            }
        }
    }
}
