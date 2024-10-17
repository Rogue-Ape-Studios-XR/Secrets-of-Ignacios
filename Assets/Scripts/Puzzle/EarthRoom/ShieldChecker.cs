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
                        _shieldFits = true;
                        break;
                    case (false, true):
                        Debug.Log("Shield is grown");
                        _shieldFits = true;
                        break;
                    case (false, false):
                        Debug.Log("Shield is regular size");
                        _shieldFits = true;
                        break;
                }

                if (previousState != _shieldFits)
                {
                    onShieldFitChanged?.Invoke();
                }
            }
        }
    }
}
