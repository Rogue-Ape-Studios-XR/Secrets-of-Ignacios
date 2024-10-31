using UnityEngine;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.VFX;
using NUnit.Framework.Internal;


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


        private VisualEffect _currentEffectL; 
        private VisualEffect _currentEffectR;
        private Gesture _lastGesture;


        //unitask things

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            //unsubscribe
            SpellManager.OnSpellValidation -= HandleCastRecognized;
            _sequenceManager.OnGestureRecognised -= HandleElementRecognized;
        }

        void Start()
        {
            //subscribe!
            SpellManager.OnSpellValidation += HandleCastRecognized;
            _sequenceManager.OnGestureRecognised += HandleElementRecognized;
        }

        void HandleElementRecognized(Gesture gesture)
        {
            GameObject r = null;
            GameObject l = null;
            switch (gesture._rightHandShape)
            {
                case HandShape.Start:
                    //disable all hand effects
                    if(_currentEffectL != null)
                    {
                        _currentEffectL.Stop();
                    }
                    if(_currentEffectR != null)
                    {
                        _currentEffectR.Stop();
                    }

                    break;
                case HandShape.Fire:
                    //spawn fire vfx at circle
                    r = Instantiate(_heldFirePrefab, _prefabContainerR.transform, false);
                    l = Instantiate(_heldFirePrefab, _prefabContainerL.transform, false);
                    _lastGesture = gesture;
                    SetVFXContainerPositions();
                    break;

                /* because the other gestures aren't properly done yet, i'm commenting this out
                case HandShape.Water:
                    //spawn water vfx at circle
                    r = Instantiate(_heldWaterPrefab, _prefabContainerR.transform, false);
                    l = Instantiate(_heldWaterPrefab, _prefabContainerL.transform, false);
                    _lastGesture = gesture;
                    SetVFXContainerPositions();
                    break;
                case HandShape.Air:
                    //spawn air vfx at circle
                    r = Instantiate(_heldAirPrefab, _prefabContainerR.transform, false);
                    l = Instantiate(_heldAirPrefab, _prefabContainerL.transform, false);
                    _lastGesture = gesture;
                    SetVFXContainerPositions();
                    break;
                case HandShape.Earth:
                    //spawn earth vfx at circle
                    r = Instantiate(_heldEarthRPrefab, _prefabContainerR.transform, false);
                    l = Instantiate(_heldEarthLPrefab, _prefabContainerL.transform, false);
                    _lastGesture = gesture;
                    SetVFXContainerPositions();
                    break;*/
                case HandShape.QuickCast:
                    // effect should be at hands and play there instantly
                    // kind of requires refactoring where the effect is on the gesture

                    break;
                default:
                    break;
            }
            if (r != null && l != null) { //await the effect's destruction
                DestroyVisualEffectWhenDone(_cancellationTokenSource.Token, r);
                DestroyVisualEffectWhenDone(_cancellationTokenSource.Token, l);

                //assign current effect
                if (r.TryGetComponent<VisualEffect>(out VisualEffect visualr))
                {
                    if (_currentEffectR != null)
                    {
                        _currentEffectR.Stop();
                    }
                    _currentEffectR = visualr;
                }
                if (l.TryGetComponent<VisualEffect>(out VisualEffect visuall))
                {
                    if (_currentEffectL != null)
                    {
                        _currentEffectL.Stop();
                    }
                    _currentEffectL = visuall;
                }
            }
        }

        private async void MoveVfxToHand(CancellationToken token, GameObject effect, Transform dest)
        {
            //remove transform so it goes to world
            effect.transform.parent = null;
            // loop lerp
            for (int i = 0; i < 10; i++)
            {
                await UniTask.WaitForSeconds(0.03f);
                effect.transform.position = Vector3.Lerp(effect.transform.position, dest.position, 0.45f);
            }
            // add new transform of hand
            effect.transform.parent = dest;
            effect.transform.localPosition = Vector3.zero;
        }

        private async void DestroyVisualEffectWhenDone(CancellationToken token, GameObject effect)
        {
            try
            {
                if (effect.TryGetComponent<VisualEffect>(out VisualEffect visual))
                {
                    await UniTask.WaitUntil(() => visual.aliveParticleCount == 0, cancellationToken: token);
                    // Do stuff after done
                    // 
                    Destroy(effect);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Example was Canceled...");
            }
        }

        private void SetVFXContainerPositions()
        {
            _prefabContainerR.transform.parent = _magicCircleTarget.transform;
            _prefabContainerL.transform.parent = _magicCircleTarget.transform;
            _prefabContainerR.transform.localPosition = Vector3.zero;
            _prefabContainerL.transform.localPosition = Vector3.zero;
            _prefabContainerR.SetActive(true);
        }

        void HandleCastRecognized(bool recognized)
        {
            //activate the second vfx and move the current vfx to the hands
            _prefabContainerL.SetActive(true);

            MoveVfxToHand(_cancellationTokenSource.Token, _prefabContainerR, _palmR);
            MoveVfxToHand(_cancellationTokenSource.Token, _prefabContainerL, _palmL);

        }

    }

}

