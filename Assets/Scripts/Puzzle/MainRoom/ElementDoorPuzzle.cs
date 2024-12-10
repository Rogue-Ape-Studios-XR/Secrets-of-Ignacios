using RogueApeStudios.SecretsOfIgnacios.Interactables.Fire;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Water;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Wind;
using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.MainRoom
{
    public class ElementDoorPuzzle : MonoBehaviour
    {
        [SerializeField] private List<Interactables.Interactables> _targets;

        [SerializeField] private Fillable _waterTarget;
        [SerializeField] private Blowable _windTarget;
        [SerializeField] private PersistentFire _fireTarget;

        [SerializeField] private Animator _animator;

        [SerializeField] private List<GameObject> _areasToUnlock;
        
        private void Awake()
        {
            _waterTarget.onFilled += TargetCheck;
            _windTarget.onBlown += TargetCheck;
            _fireTarget.OnIgnitionToggle += TargetCheck;
        }

        private void OnDestroy()
        {
            _waterTarget.onFilled -= TargetCheck;
            _windTarget.onBlown -= TargetCheck;
            _fireTarget.OnIgnitionToggle -= TargetCheck;
        }

        private void TargetCheck(bool hit)
        {
            if (_waterTarget._filled && _windTarget._isBlown && _fireTarget._isOnFire)
            {
                Debug.Log("Door opens");
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
