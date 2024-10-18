using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.EarthRoom
{
    public class ShieldSequenceChecker : MonoBehaviour
    {
        [SerializeField] private List<ShieldChecker> _shieldCheckers;
        [SerializeField] private bool _allShieldsFit;

        private void OnEnable()
        {
            foreach (var shieldChecker in _shieldCheckers)
            {
                shieldChecker.onShieldFitChanged += CheckAllShields;
            }
            //Do an initial check
            CheckAllShields();
        }

        private void OnDisable()
        {
            foreach (var shieldChecker in _shieldCheckers)
            {
                shieldChecker.onShieldFitChanged -= CheckAllShields;
            }
        }

        private void CheckAllShields()
        {
            //It's true by default when checked initially (will be set false in the foreach, so no need to worry about early puzzle clears)
            _allShieldsFit = true;

            foreach (var shieldChecker in _shieldCheckers)
            {
                if (!shieldChecker.ShieldFits)
                {
                    _allShieldsFit = false;
                    break;
                }
            }

            if (_allShieldsFit)
                Debug.Log("All shields fit");
            else
                Debug.Log("Not all shields fit");
        }
    }
}
