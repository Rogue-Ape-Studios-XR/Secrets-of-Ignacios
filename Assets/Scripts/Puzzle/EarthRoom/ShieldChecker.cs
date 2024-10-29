using System;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Earth;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.EarthRoom
{
    public class ShieldChecker : MonoBehaviour
    {
        [SerializeField] private string _shieldTag = "Shield";
        [SerializeField] private bool _checkForShrunk;
        [SerializeField] private bool _checkForGrown;
        [SerializeField] private bool _shieldFits;
        //Each shield is different so you should add the correct shield here
        [SerializeField] private GameObject _targetShield;
        [SerializeField] private Resizable _targetResizable;

        public bool ShieldFits => _shieldFits;
        //Action so it doesn't need to constantly verify in update

        public event Action onShieldFitChanged;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_shieldTag) && other.gameObject == _targetShield)
            {
                bool previousState = _shieldFits;
                _shieldFits = IsShieldInRequiredState();

                if (previousState != _shieldFits)
                {
                    onShieldFitChanged?.Invoke();
                }

                if (_shieldFits)
                    //Just disable for now, could have an extra statement checking if the user still has it grabbed
                {
                    _targetShield.SetActive(false);
                    Debug.Log("Shield fits and is disabled");
                }
                else
                {
                    //Placeholder debug log as requested
                    Debug.Log("Shield doesn't fit");
                }
            }
        }

        private bool IsShieldInRequiredState()
        {
            switch (_targetResizable.CurrentState)
            {
                case ResizeState.Shrunk:
                    return _checkForShrunk;
                case ResizeState.Grown:
                    return _checkForGrown;
                case ResizeState.Default:
                    return !_checkForShrunk && !_checkForGrown;
                default:
                    return false;
            }
        }
    }
}
