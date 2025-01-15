using System.Collections.Generic;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Player.SpellMagicCircle
{
    public class FinalSpellMagicCirclePlayerFeedback : MonoBehaviour
    {
        [SerializeField] private List<VisualEffect> _magicCircles;
        [SerializeField] private float _yOffset = 0.5f;
        [SerializeField] private GameObject _magicCircleController;
        
        private int _currentCircleIndex = 0;
        private Camera _mainCamera;

        private void Start()
        {
            //Sorry but like, no matter what I did, the damn circles went into the floor
            _mainCamera = Camera.main;
            foreach (var magicCircle in _magicCircles)
            {
                magicCircle.Stop();
            }
            PositionMagicCircles();

            SequenceManager.onFinalSpellValidation += HandleOnFinalSpellValidation;
            SpellManager.onNoSpellMatch += StopAllMagicCircles;
        }

        private void OnDestroy()
        {
            SequenceManager.onFinalSpellValidation -= HandleOnFinalSpellValidation;
            SpellManager.onNoSpellMatch -= StopAllMagicCircles;
        }

        private void HandleOnFinalSpellValidation()
        {
            if (!_magicCircleController.activeSelf)
                _magicCircleController.SetActive(true);
            
            if (_currentCircleIndex < _magicCircles.Count)
            {
                _magicCircles[_currentCircleIndex].Play();
                _currentCircleIndex++;
            }
        }

        private void StopAllMagicCircles()
        {
            _currentCircleIndex = 0;
            foreach (var magicCircle in _magicCircles)
            {
                magicCircle.Stop();
            }
            _magicCircleController.SetActive(false);
        }

        private void PositionMagicCircles()
        {
            if (_mainCamera != null)
            {
                for (int i = 0; i < _magicCircles.Count; i++)
                {
                    var magicCirclePosition = _mainCamera.transform.position;
                    magicCirclePosition.y += _yOffset;

                    _magicCircles[i].transform.position = magicCirclePosition;
                }
            }
        }
    }
}
