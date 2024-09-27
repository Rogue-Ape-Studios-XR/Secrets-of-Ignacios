using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.FireRoom
{
    public class TilePuzzleSequenceChecker : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _tileSequence;
        private List<GameObject> _tileCollisions = new();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        internal void AddAndValidate(GameObject tile)
        {
            AddTileToList(tile);
            CheckSequence();
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
            }
        }
    }
}
