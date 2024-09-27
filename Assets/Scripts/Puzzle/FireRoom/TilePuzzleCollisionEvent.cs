using RogueApeStudios.SecretsOfIgnacios.Puzzle.FireRoom;
using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.FireRoom
{
    public class TilePuzzleCollisionEvent : MonoBehaviour
    {
        [SerializeField] private TilePuzzleSequenceChecker _tilePuzzleSequenceChecker;

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                _tilePuzzleSequenceChecker.AddAndValidate(gameObject);
            }
        }
    }
}
