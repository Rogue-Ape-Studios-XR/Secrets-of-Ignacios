using UnityEngine;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;
using UnityEngine.VFX;
using NUnit.Framework.Internal;
using UnityEngine.XR.Hands;

namespace RogueApeStudios.SecretsOfIgnacios.Player.SpellMagicCircle
{
    public class HandVfxManager : MonoBehaviour
    {
        // Script should have a reference to two visual effects. These should enable and swap to the correct vfx on gesture recognized.
        // This VFX is not contained in the spell data as a prefab. Therefore it checks spell type.
        // Element is recognized on gesture 1 
        // Refactor: Element prefab should be on Gesture. This allows for expandability. Earth can be overridden.


        [SerializeField] private GameObject _prefabContainerL; // a container for the vfx. This is the one moving. This can be turned off with the public function
        [SerializeField] private GameObject _prefabContainerR; 
        [SerializeField] private Transform  _palmL; 
        [SerializeField] private Transform  _palmR; 
        [SerializeField] private GameObject _magicCircleTarget; 

        [SerializeField] private SequenceManager _sequenceManager;
        [SerializeField] private Cast _castscript;


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

            _sequenceManager.OnElementValidated -= HandleElementRecognized;
            
            _castscript.onSpellCastComplete -= DisableHandVFX;
            //unsubscribe from cast script's cast finished event (event on cast not implemented)
        }

        void Start()
        {
            //subscribe!
            SpellManager.OnSpellValidation += HandleCastRecognized;
            _sequenceManager.OnElementValidated += HandleElementRecognized;
            _castscript.onSpellCastComplete += DisableHandVFX;
            //subscribe to cast script's cast finished event (event on cast not implemented)
        }

        void HandleElementRecognized(Gesture gesture)
        {
            GameObject r = null;
            GameObject l = null;
            Debug.Log(gesture._rightHandShape);
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
                case HandShape.QuickCast:
                    // effect should be at hands and play there instantly
                    // This doesn't work since quick cast is aparrently a seperate thing.
                    break;
                default:
                    if (gesture._visualEffectPrefab != null)
                    {
                        r = Instantiate(gesture._visualEffectPrefab, _prefabContainerR.transform, false);
                        l = Instantiate(gesture._visualEffectPrefab, _prefabContainerL.transform, false);
                        _lastGesture = gesture;
                        SetVFXContainerPositions();
                    }
                    break;
            }
            if (r != null && l != null) { //This sets a timer on the visual effect to destroy when it is done playing.
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

        private void HandleQuickCast() //will subscribe to quick cast event
        {
            if (_lastGesture != null && _lastGesture._visualEffectPrefab != null)
            {
                GameObject rr = Instantiate(_lastGesture._visualEffectPrefab, _prefabContainerR.transform, false);
                GameObject ll = Instantiate(_lastGesture._visualEffectPrefab, _prefabContainerL.transform, false);
                SetVFXContainerPositions();

                DestroyVisualEffectWhenDone(_cancellationTokenSource.Token, rr);
                DestroyVisualEffectWhenDone(_cancellationTokenSource.Token, ll);

                //assign current effect
                if (rr.TryGetComponent<VisualEffect>(out VisualEffect visualr))
                {
                    if (_currentEffectR != null)
                    {
                        _currentEffectR.Stop();
                    }
                    _currentEffectR = visualr;
                }
                if (ll.TryGetComponent<VisualEffect>(out VisualEffect visuall))
                {
                    if (_currentEffectL != null)
                    {
                        _currentEffectL.Stop();
                    }
                    _currentEffectL = visuall;
                }

                MoveVfxToHand(_cancellationTokenSource.Token, _prefabContainerR, _palmR);
                MoveVfxToHand(_cancellationTokenSource.Token, _prefabContainerL, _palmL);
            }
        }

        private void DisableHandVFX(Handedness hand)
        {
            if (hand == Handedness.Left && _currentEffectL != null)
            {
                _currentEffectL.Stop();
            }
            else if (hand == Handedness.Right && _currentEffectR != null)
            {
                _currentEffectR.Stop();
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

            //if last gesture was earth, stop the vfx (so the touch spell works)
            if (_lastGesture != null && _lastGesture._rightHandShape == HandShape.Earth)
            {
                _currentEffectL.Stop();
                _currentEffectR.Stop();
            }
        }

        private async void DestroyVisualEffectWhenDone(CancellationToken token, GameObject effect)
        {
            try
            {
                if (effect.TryGetComponent<VisualEffect>(out VisualEffect visual))
                {
                    // Destroy effect after it is done playing.
                    await UniTask.WaitUntil(() => visual.aliveParticleCount == 0, cancellationToken: token);
                    Destroy(effect);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Visual Effect wasn't destroyed (likely, the game went out of play mode)");
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

