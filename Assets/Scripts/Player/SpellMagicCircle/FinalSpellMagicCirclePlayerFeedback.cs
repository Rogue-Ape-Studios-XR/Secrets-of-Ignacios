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
        [SerializeField] private SequenceManager _sequenceManager;
        
        private int _currentCircleIndex = 0;
        private bool _wasWrong = false;
        private bool _finalSpellUnlocked;
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
            _sequenceManager.onReset += StopAllMagicCircles;
            _sequenceManager.onSpellFailedVFX += StopAllMagicCircles;
            SequenceManager.onFinalSpellCompleted += HandleFinalSpellCompletion;
            FinalSpellTrigger.onFinalSpellLock += StopAllMagicCircles;
            FinalSpellTrigger.onFinalSpellUnlocked += HandleFinalSpellUnlock;
            SequenceManager.onFinalSpellValidation += HandleOnFinalSpellValidation;
            SpellManager.onNoSpellMatch += StopAllMagicCircles;
        }

        private void OnDestroy()
        {
            _sequenceManager.onReset -= StopAllMagicCircles;
            _sequenceManager.onSpellFailedVFX -= StopAllMagicCircles;
            FinalSpellTrigger.onFinalSpellLock -= StopAllMagicCircles;
            FinalSpellTrigger.onFinalSpellUnlocked -= HandleFinalSpellUnlock;
            SequenceManager.onFinalSpellCompleted -= HandleFinalSpellCompletion;
            SequenceManager.onFinalSpellValidation -= HandleOnFinalSpellValidation;
            SpellManager.onNoSpellMatch -= StopAllMagicCircles;
        }

        private void HandleOnFinalSpellValidation()
        {
            if (_wasWrong)
            {
                _wasWrong = false;
                return;
            }
            if (!_magicCircleController.activeSelf)
                _magicCircleController.SetActive(true);
            
            if (_currentCircleIndex < _magicCircles.Count)
            {
                Debug.LogWarning("Magic circle count" + _currentCircleIndex);
                _magicCircles[_currentCircleIndex].Play();
                _currentCircleIndex++;
            }
        }

        private void StopAllMagicCircles()
        {
            if (_finalSpellUnlocked)
            {
                _wasWrong = true;
            }
            Debug.LogWarning("stopping circles");
            _currentCircleIndex = 0;
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

        private void HandleFinalSpellUnlock()
        {
            _finalSpellUnlocked = true;
        }

        private void HandleFinalSpellCompletion()
        {
            _wasWrong = true;
            StopAllMagicCircles();
        }
    }
}
