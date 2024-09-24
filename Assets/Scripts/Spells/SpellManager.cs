using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [SerializeField] private Spell[] _availableSpells;
        [SerializeField] private GestureManager _gestureManager;

        private Spell _currentSpell;
        private bool _canCast = false;

        internal void ResetSpell()
        {
            _currentSpell = null;
            _canCast = false;
        }

        public void ValidateSpell()
        {
            print(_gestureManager.ValidatedGestures.Count);
            foreach (var g in _gestureManager.ValidatedGestures) 
                print(g);

            foreach (var spell in _availableSpells)
                if (spell._gestureSequence.Count == _gestureManager.ValidatedGestures.Count && 
                    spell._gestureSequence == _gestureManager.ValidatedGestures)
                    Debug.Log(spell.name);
        }

    }
}
