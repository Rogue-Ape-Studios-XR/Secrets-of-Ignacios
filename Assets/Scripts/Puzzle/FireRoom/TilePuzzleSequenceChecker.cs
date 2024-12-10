using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.Progression;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.FireRoom
{
    public class TilePuzzleSequenceChecker : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _tileSequence;
        private List<GameObject> _tileCollisions = new();
        [SerializeField] private Spell _spellToUnlock;

        internal void AddAndValidate(GameObject tile)
        {
            AddTileToList(tile);
            if (_tileCollisions.Count == _tileSequence.Count)
            {
                //Fire spell
                UnlockSpell();
                //victory
            }
            else
            {
                CheckSequence();
            }
        }

        private void AddTileToList(GameObject tile)
        {
            _tileCollisions.Add(tile);
        }

        private void CheckSequence()
        {
            if (_tileCollisions[^1] == _tileSequence[_tileCollisions.Count - 1])
            {
                //correct
                Debug.Log("Joepie");
            }
            else
            {
                //incorrect
                Debug.Log("Faal");
                _tileCollisions.Clear();
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
