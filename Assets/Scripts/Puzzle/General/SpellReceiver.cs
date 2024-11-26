using System;
using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle
{
    public class SpellReceiver : MonoBehaviour
    {
        [SerializeField] private List<string> _spellTags;
        internal event Action<string> OnSpellReceived;

        private void OnTriggerEnter(Collider other)
        {
            if (_spellTags.Contains(other.tag))
            {
                var spell = other.gameObject;
                HandleSpellReceived(spell);
            }
        }

        void HandleSpellReceived(GameObject spell)
        {
            //get the spell name
            SpellData spelldata = spell.GetComponentInChildren<SpellData>();
            Debug.Log(spelldata.spellName);
            OnSpellReceived.Invoke(spelldata.spellName);
        }
    }
}
