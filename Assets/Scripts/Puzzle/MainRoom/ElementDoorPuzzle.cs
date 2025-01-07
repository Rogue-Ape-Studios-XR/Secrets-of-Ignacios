using RogueApeStudios.SecretsOfIgnacios.Interactables.Fire;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Water;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Wind;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
        [SerializeField] private Spell _spellToUnlock;

        [SerializeField] private VisualEffect _fireVisualEffect;
        [SerializeField] private VisualEffect _waterVisualEffect;
        [SerializeField] private VisualEffect _windVisualEffect;

        private bool _opened = false;

        private void Awake()
        {
            _waterTarget.onFilled += TargetCheck;
            _waterTarget.onFilled += (bool fodder) => { _waterVisualEffect.Play(); };
            _windTarget.onBlown += TargetCheck;
            _windTarget.onBlown += (bool fodder) => { _windVisualEffect.Play(); }; ;
            _fireTarget.OnIgnitionToggle += TargetCheck;
            _fireTarget.OnIgnitionToggle += (bool fodder) => { _fireVisualEffect.Play(); }; ;

            _fireVisualEffect.Stop();
            _waterVisualEffect.Stop();
            _windVisualEffect.Stop();
        }

        private void OnDestroy()
        {
            _waterTarget.onFilled -= TargetCheck;
            _windTarget.onBlown -= TargetCheck;
            _fireTarget.OnIgnitionToggle -= TargetCheck;
        }

        private void TargetCheck(bool hit)
        {
            if (_waterTarget._filled && _windTarget._isBlown && _fireTarget._isOnFire && !_opened)
            {
                _opened = true;
                Debug.Log("Door opens");
                _animator.SetTrigger("DubbleIn");
                UnlockAreas();
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
