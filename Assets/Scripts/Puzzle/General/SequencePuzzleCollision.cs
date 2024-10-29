using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.General
{
    public class SequencePuzzleCollision : MonoBehaviour
    {
        [SerializeField] private SequenceChecker _sequenceChecker;
        [SerializeField] private List<string> _tags;

        private void OnTriggerEnter(Collider other)
        {
            if (_tags.Contains(other.tag))
            {
                _sequenceChecker.AddChoiceToList(gameObject);
                _sequenceChecker.CheckSequence();
                print(other.name);
            }
        }
    }
}
