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

            foreach (var shieldChecker in _shieldCheckers)
            {
                if (!shieldChecker.ShieldFits)
                {
                    _allShieldsFit = false;
                    break;
                }
            }

            if (_allShieldsFit)
            {
                Debug.Log("All shields fit");
            }
            else
            {
                Debug.Log("Not all shields fit");
            }
        }
    }
}
