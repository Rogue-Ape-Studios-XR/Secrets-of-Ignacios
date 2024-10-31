using UnityEngine;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using RogueApeStudios.SecretsOfIgnacios.Gestures;


namespace RogueApeStudios.SecretsOfIgnacios
{
    public class HandVfxManager : MonoBehaviour
    {
        // Script should have a reference to two visual effects. These should enable and swap to the correct vfx on gesture recognized.
        // This VFX is not contained in the spell data as a prefab. Therefore it checks spell type.
        // Element is recognized on gesture 1 

        [SerializeField] private GameObject _heldFirePrefab;
        [SerializeField] private GameObject _heldWaterPrefab;
        [SerializeField] private GameObject _heldAirPrefab;
        [SerializeField] private GameObject _heldEarthLPrefab; //this one is shrink
        [SerializeField] private GameObject _heldEarthRPrefab; //this one is grow

        [SerializeField] private GameObject _prefabContainerL; // a container for the vfx. This is the one moving. This will be disabled by the spell manager
        [SerializeField] private GameObject _prefabContainerR; 
        [SerializeField] private Transform  _palmL; 
        [SerializeField] private Transform  _palmR; 
        [SerializeField] private GameObject _magicCircleTarget; 

        [SerializeField] private SequenceManager _sequenceManager;

        void Start()
        {
            //subscribe!
            SpellManager.OnSpellValidation += HandleSpellRecognized;
            _sequenceManager.OnGestureRecognised += HandleGestureRecognized;
        }


        private void OnDestroy()
        {
            //unsubscribe
            SpellManager.OnSpellValidation -= HandleSpellRecognized;
            _sequenceManager.OnGestureRecognised -= HandleGestureRecognized;

        }

        // Update is called once per frame


        void HandleGestureRecognized(Gesture gesture)
        {
            switch (gesture._rightHandShape)
            {
                case HandShape.Start:
                    break;
                case HandShape.Fire:
                    //spawn fire vfx at circle
                    Instantiate(_heldFirePrefab, _prefabContainerR.transform, false);
                    Instantiate(_heldFirePrefab, _prefabContainerL.transform, false);
                    _prefabContainerR.transform.parent = _magicCircleTarget.transform;
                    _prefabContainerL.transform.parent = _magicCircleTarget.transform;
                    _prefabContainerR.transform.localPosition = Vector3.zero;
                    _prefabContainerL.transform.localPosition = Vector3.zero;
                    _prefabContainerR.SetActive(true);
                    break;
                case HandShape.Water:
                    //spawn water vfx at circle
                    Instantiate(_heldWaterPrefab, _prefabContainerR.transform, false);
                    Instantiate(_heldWaterPrefab, _prefabContainerL.transform, false);
                    _prefabContainerR.transform.parent = _magicCircleTarget.transform;
                    _prefabContainerL.transform.parent = _magicCircleTarget.transform;
                    _prefabContainerR.transform.localPosition = Vector3.zero;
                    _prefabContainerL.transform.localPosition = Vector3.zero;
                    _prefabContainerR.SetActive(true);
                    break;
                case HandShape.Air:
                    //spawn air vfx at circle
                    Instantiate(_heldAirPrefab, _prefabContainerR.transform, false);
                    Instantiate(_heldAirPrefab, _prefabContainerL.transform, false);
                    _prefabContainerR.transform.parent = _magicCircleTarget.transform;
                    _prefabContainerL.transform.parent = _magicCircleTarget.transform;
                    _prefabContainerR.transform.localPosition = Vector3.zero;
                    _prefabContainerL.transform.localPosition = Vector3.zero;
                    _prefabContainerR.SetActive(true);
                    break;
                case HandShape.Earth:
                    //spawn earth vfx at circle
                    Instantiate(_heldEarthRPrefab, _prefabContainerR.transform, false);
                    Instantiate(_heldEarthLPrefab, _prefabContainerL.transform, false);
                    _prefabContainerR.transform.parent = _magicCircleTarget.transform;
                    _prefabContainerL.transform.parent = _magicCircleTarget.transform;
                    _prefabContainerR.transform.localPosition = Vector3.zero;
                    _prefabContainerL.transform.localPosition = Vector3.zero;
                    _prefabContainerR.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        void HandleSpellRecognized(bool recognized)
        {
            //activate the second vfx and move the current vfx to the hands
            _prefabContainerL.SetActive(true);
            _prefabContainerR.transform.parent = _palmR.transform;
            _prefabContainerL.transform.parent = _palmL.transform;
            _prefabContainerR.transform.localPosition = Vector3.zero;
            _prefabContainerL.transform.localPosition = Vector3.zero;


        }

    }

}

