using RogueApeStudios.SecretsOfIgnacios.Puzzle.FireRoom;
using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class TilePuzzleCollisionEvent : MonoBehaviour
    {
        [SerializeField] private TilePuzzleSequenceChecker _tilePuzzleSequenceChecker;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == "Player")
            {
                _tilePuzzleSequenceChecker.AddAndValidate(gameObject);
            }
        }
    }
}
