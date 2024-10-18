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
        
        public bool ShieldFits => _shieldFits;
        //Action so it doesn't need to constantly verify in update
        public event Action onShieldFitChanged;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_shieldTag) && other.TryGetComponent(out Resizable resizable) && other.gameObject == _targetShield)
            {
                bool previousState = _shieldFits;

                switch ((resizable.Shrunk, resizable.Grown))
                {
                    case (true, false):
                        Debug.Log("Shield is shrunk");
                        _shieldFits = _checkForShrunk;
                        break;
                    case (false, true):
                        Debug.Log("Shield is grown");
                        _shieldFits = _checkForGrown;
                        break;
                    case (false, false):
                        Debug.Log("Shield is regular size");
                        _shieldFits = !_checkForShrunk && !_checkForGrown;
                        break;
                    default:
                        _shieldFits = false;
                        break;
                }

                if (previousState != _shieldFits)
                    onShieldFitChanged?.Invoke();
                
                if (_shieldFits)
                    //Just disable for now, could have an extra statement checking if the user still has it grabbed
                    _targetShield.SetActive(false);
            }
        }
    }
}
