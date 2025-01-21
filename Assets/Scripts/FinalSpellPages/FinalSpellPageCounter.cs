using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.FinalSpellPages
{
    public class FinalSpellPageCounter : MonoBehaviour
    {
        public static event Action onAllPagesCollected;

        [SerializeField] private int _currentPageCounter;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            FinalSpellPageScript.onPagePickup += UpdateCounter;
            _currentPageCounter = 0;
        }

        private void OnDestroy()
        {
            FinalSpellPageScript.onPagePickup -= UpdateCounter;
        }

        private void UpdateCounter()
        {
            _currentPageCounter++;
            if (_currentPageCounter >= 5)
                // just invoke it here and trigger the event on every page pickup
                onAllPagesCollected?.Invoke();
        }
    }
}
