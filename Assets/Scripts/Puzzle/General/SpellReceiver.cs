using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle
{
    public class SpellReceiver : MonoBehaviour
    {
        internal event Action<string> OnSpellReceived;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Spell"))
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
