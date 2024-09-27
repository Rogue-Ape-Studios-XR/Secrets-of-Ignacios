using System.Linq;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [SerializeField] private Spell[] _availableSpells;
        [SerializeField] private SequenceManager _gestureManager;

        private Spell _currentSpell;
        private bool _canCast = false;

        private void OnEnable()
        {
            _gestureManager.OnSequenceCreated += ValidateSequence;
            _gestureManager.OnReset += HandleReset;
        }

        private void OnDestroy()
        {
            _gestureManager.OnSequenceCreated -= ValidateSequence;
            _gestureManager.OnReset -= HandleReset;
        }

        private void SetSpell(Spell spell)
        {
            _currentSpell = spell;
        }

        public void ValidateSequence()
        {
            foreach (var spell in _availableSpells)
                if (spell._gestureSequence.Count == _gestureManager.ValidatedGestures.Count &&
                    spell._gestureSequence.SequenceEqual(_gestureManager.ValidatedGestures))
                {
                    SetSpell(spell);
                    Debug.Log("HEEEEEEEEEEEEEEEEEEY" + spell.name);
                }
        }

        internal void HandleReset()
        {
            _currentSpell = null;
            _canCast = false;
        }
    }
}
