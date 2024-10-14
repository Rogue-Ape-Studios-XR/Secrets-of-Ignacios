using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Earth
{
    internal class Resizable : EarthInteractable
    {
        [SerializeField] private Vector3 _initialScale;
        [SerializeField] private Transform _targetObject;
        [SerializeField] private Vector3[] _premadeSizes;
        [SerializeField] private bool _usesPremadeSizes;
        [SerializeField, Range(0f, 2f)] private float _resizeValue;

        internal override void Awake()
        {
            base.Awake();
            _initialScale = _targetObject.localScale;
           
        }

        internal override void Touched()
        {
            _isTouched = true;
            _targetObject.localScale = _initialScale;
        }

        private void OnValidate()
        {
            ResizeObjectInEditor();
        }

        //Just for testing atm, still trying to figure out how the hell I do this
        private void ResizeObjectInEditor()
        {
            if (_targetObject == null) return;

            Vector3 newScale = _initialScale * _resizeValue;

            if (_usesPremadeSizes && _premadeSizes.Length > 0)
            {
                newScale = FindClosestPremadeSize(newScale);
            }

            _targetObject.localScale = newScale;
        }

        //Doesn't work properly yet
        private Vector3 FindClosestPremadeSize(Vector3 targetScale)
        {
            Vector3 closestSize = _premadeSizes[0];
            float closestDistance = Vector3.Distance(targetScale, closestSize);

            foreach (Vector3 size in _premadeSizes)
            {
                float distance = Vector3.Distance(targetScale, size);
                if (distance < closestDistance)
                {
                    closestSize = size;
                    closestDistance = distance;
                }
            }

            return closestSize;
        }
    }
}
