using System;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class GodSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _ignacios;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private bool _testBool;
        private void Start()
        {
            SequenceManager.onFinalSpellCompleted += HandleIgnaciosSpawning;
        }

        private void OnDestroy()
        {
            SequenceManager.onFinalSpellCompleted += HandleIgnaciosSpawning;
        }

        private void Update()
        {
            if (_testBool)
            {
                HandleIgnaciosSpawning();
                _testBool = false;
            }
        }

        private void HandleIgnaciosSpawning()
        {
            _ignacios.SetActive(true);
            _collider.enabled = true;
        }
    }
}
