using RogueApeStudios.SecretsOfIgnacios.Interactables.Earth;
using RogueApeStudios.SecretsOfIgnacios.Interactables.Water;
using UnityEngine;
using UnityEngine.Events;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class ChainPuzzleFeedback : MonoBehaviour
    {
        [SerializeField] private Resizable _resizable;
        [SerializeField] private Fillable _fillable;
        [SerializeField] private bool _isWaterPuzzle;
        [SerializeField] private ResizeState wantedSize;
        [SerializeField] public UnityEvent onChainHeavy;
        [SerializeField] public UnityEvent onChainUnheavy;

        private bool _correctSize = false;
        private bool _filled = false;
        private bool _chainDown = false;

        private void Start()
        {
            if (_isWaterPuzzle)
            {
                _fillable.onFilled += CheckWater;
            }
            else 
            {     
                _filled = true;
            }
            _resizable.onSizeChanged += CheckSize;
            CheckSize();

        }
        private void OnDestroy()
        {
            if (_isWaterPuzzle)
            {
                _fillable.onFilled -= CheckWater;
            }
            _resizable.onSizeChanged -= CheckSize;
        }
        void CheckSize()
        {
            if(_resizable.CurrentState == wantedSize)
            {
                _correctSize = true;
            }
            else _correctSize = false;
            CheckWeight();
        }
        void CheckWater(bool filled)
        {
            _filled = true;
            CheckWeight();
        }

        void CheckWeight()
        {
            //Debug.Log(_correctSize);
            Debug.Log(_filled);
            if (_correctSize && _filled)
            {
                onChainHeavy.Invoke();
                _chainDown = true;
            }
            else if (_chainDown)
            {
                onChainUnheavy.Invoke();
            }
        }
    }
}
