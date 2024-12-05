using RogueApeStudios.SecretsOfIgnacios.Gestures;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Player
{
    public class MagicCirclePlayerFeedback : MonoBehaviour
    {

        /*
         This script should subscribe to the "handsign added" event, and do the following depending on the gesture

            -On start gesture, set the should be alive of the vfx to true and play() the magic circle
            -On an element added, change the colour to the element's colour - make it a gradient if possible
            -On end cast sign, set should be alive to false
         */

        [SerializeField] private VisualEffect _magicCircle;
        [SerializeField] private GameObject _boolControls;
        [SerializeField] private LineRenderer _colorControls;
        [SerializeField] private SequenceManager _sequenceManager;

        private void Start()
        {
            _magicCircle.Stop();

            //sub to the gesture recognized and spell recognized events
            SpellManager.onSpellValidation += HandleSpellRecognized;
            SpellManager.onNoSpellMatch += HandleSpellFailed;
            _sequenceManager.onGestureRecognised += HandleGestureRecognized;
        }
        private void OnDestroy()
        {
            //unsubscribe
            SpellManager.onSpellValidation -= HandleSpellRecognized;
            SpellManager.onNoSpellMatch -= HandleSpellFailed;
            _sequenceManager.onGestureRecognised -= HandleGestureRecognized;
        }

        void HandleGestureRecognized(List<Gesture> gesture)
        {
            if (gesture.Count > 0)
            {
                switch (gesture[^1]._rightHandShape)
                {
                    case HandShape.Start:
                        _boolControls.SetActive(true);
                        _magicCircle.Play();
                        break;
                    case HandShape.Fire:
                        break;
                    case HandShape.Water:
                        break;
                    case HandShape.Earth:
                        break;
                    case HandShape.Air:
                        break;
                    default:
                        break;
                }
                _colorControls.startColor = gesture[^1]._color;
            }
        }

        void HandleSpellRecognized(bool recognized)
        {
            _boolControls.SetActive(false);
        }

        private void HandleSpellFailed()
        {
            _boolControls.SetActive(false);
        }

    }
}
