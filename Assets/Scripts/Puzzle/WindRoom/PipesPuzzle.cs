using RogueApeStudios.SecretsOfIgnacios.Puzzle.General;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.WindRoom
{
    public class PipesPuzzle : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SequenceChecker _sequenceChecker;
        [SerializeField] private string _animationTrigger;

        private void OnEnable()
        {
            _sequenceChecker.OnPuzzleComplete += HandleOnPuzzleComplete;
        }

        private void OnDestroy()
        {
            _sequenceChecker.OnPuzzleComplete -= HandleOnPuzzleComplete;
        }

        private void HandleOnPuzzleComplete()
        {
            _animator.SetTrigger(_animationTrigger);
        }
    }
}
