using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.General
{
    public class SequenceChecker : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _sequence;
        [SerializeField] private UnityEvent OnPuzzleCompleteUnityEvent;

        private List<GameObject> _playerSequence = new();

        internal event Action OnPuzzleComplete;

        internal void AddChoiceToList(GameObject choice)
        {
            _playerSequence.Add(choice);
        }

        internal void CheckSequence()
        {
            if (_playerSequence.Count == _sequence.Count && _playerSequence.SequenceEqual(_sequence))
            {
                OnPuzzleComplete?.Invoke();
                OnPuzzleCompleteUnityEvent?.Invoke();
            }
            else if (_playerSequence[^1] == _sequence[_playerSequence.Count - 1])
                Debug.Log("Correct"); //indication it was correct
            else
            {
                Debug.Log("Wrong, player's sequence was cleared"); //indication it was wrong
                _playerSequence.Clear();
            }
        }
    }
}
