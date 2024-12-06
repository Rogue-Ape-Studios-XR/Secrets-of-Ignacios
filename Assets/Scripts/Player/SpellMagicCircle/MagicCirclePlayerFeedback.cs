using RogueApeStudios.SecretsOfIgnacios.Gestures;
using RogueApeStudios.SecretsOfIgnacios.Services;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Player.SpellMagicCircle
{
    public class MagicCirclePlayerFeedback : MonoBehaviour
    {

        /*
         This script should subscribe to the "handsign added" event, and do the following depending on the gesture

            -On start gesture, set the should be alive of the vfx to true and play() the magic circle
            -On an element added, change the colour to the element's colour - make it a gradient if possible
            -On end cast sign, set should be alive to false
         */
        [SerializeField] private ObjectPooler _objectPooler;

        [SerializeField] private VisualEffect _magicCircle;
        [SerializeField] private GameObject _propertyControls;
        [SerializeField] private LineRenderer _colorControls;
        [SerializeField] private SequenceManager _sequenceManager;

        private GameObject _pooledObject;

        private void Start()
        {
            _magicCircle.Stop();

            //sub to the gesture recognized and spell recognized events
            SpellManager.onSpellValidation += ResetMagicCircle;
            SpellManager.onNoSpellMatch += ResetMagicCircle;
            _sequenceManager.onGestureRecognised += HandleGestureRecognized;
        }
        private void OnDestroy()
        {
            //unsubscribe
            SpellManager.onSpellValidation -= ResetMagicCircle;
            SpellManager.onNoSpellMatch -= ResetMagicCircle;
            _sequenceManager.onGestureRecognised -= HandleGestureRecognized;
        }

        void HandleGestureRecognized(List<Gesture> gestures)
        {
            if (gestures.Count > 0)
            {
                switch (gestures[^1]._rightHandShape)
                {
                    case HandShape.Start:
                        if (_pooledObject != null && _pooledObject.activeSelf)
                        {
                            _pooledObject.transform.parent = null;
                            _objectPooler.ReturnObject(_pooledObject.name, _pooledObject);
                        }

                        _propertyControls.SetActive(true);
                        _magicCircle.Play();
                        break;
                    default:
                        if (gestures[^1]._visualEffectPrefab != null)
                        {
                            _objectPooler.CreatePool(gestures[^1]._visualEffectPrefab.name, gestures[^1]._visualEffectPrefab, 3);
                            _pooledObject = _objectPooler.GetObject(gestures[^1]._visualEffectPrefab.name,
                                gestures[^1]._visualEffectPrefab,
                                transform);
                            _pooledObject.transform.parent = transform;
                        }
                        break;
                }
                _colorControls.startColor = gestures[^1]._color;
            }
        }

        private void ResetMagicCircle()
        {
            _propertyControls.SetActive(false);
            if (_pooledObject != null)
            {
                _pooledObject.transform.parent = null;
                _objectPooler.ReturnObject(_pooledObject.name, _pooledObject);
            }
        }
    }
}
