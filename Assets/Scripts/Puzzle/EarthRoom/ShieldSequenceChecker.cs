using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.EarthRoom
{
    public class ShieldSequenceChecker : MonoBehaviour
    {
        [SerializeField] private List<ShieldChecker> _shieldCheckers;
        [SerializeField] private bool _allShieldsFit = true;
        [SerializeField] private int _correctShieldAmount = 0;
        [SerializeField] private UnityEvent _completePuzzle;

        private void OnEnable()
        {
            ShieldChecker.onShieldFitChanged += CheckAllShields;
            ShieldChecker.onShieldFitIncrementCounter += IncrementShieldCounter;
            //Do an initial check
            CheckAllShields();
        }

        private void OnDisable()
        {
            ShieldChecker.onShieldFitChanged -= CheckAllShields;
            ShieldChecker.onShieldFitIncrementCounter -= IncrementShieldCounter;
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
                _completePuzzle?.Invoke();
            }
            else
                Debug.Log("Not all shields fit");
        }

        private void IncrementShieldCounter()
        {
            _correctShieldAmount++;
            if (_correctShieldAmount >= 5)
            {
                _completePuzzle?.Invoke();
            }
        }
    }
}
