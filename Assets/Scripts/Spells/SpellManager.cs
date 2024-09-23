using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    internal class SpellManager : MonoBehaviour
    {
        [SerializeField] private Spell[] _availableSpells;
        private Spell _currentSpell;
        private bool _canCast = false;

        internal void ResetSpell()
        {
            _currentSpell = null;
            _canCast = false;
        }

    }
}
