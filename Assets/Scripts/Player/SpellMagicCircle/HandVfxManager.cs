using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

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
        [SerializeField] private Transform _palmL;
        [SerializeField] private Transform _palmR;
        [SerializeField] private Transform _magicCircleTarget;

        [SerializeField] private SequenceManager _sequenceManager;
        [SerializeField] private Cast _castscript;

        private HandData _handDataRight;
        private HandData _handDataLeft;

        private VisualEffect _currentEffectL;
        private VisualEffect _currentEffectR;
        private Gesture _lastGesture;

        private Color _defaultColor;

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            SpellManager.onSpellValidation += HandleCastRecognized;
            SpellManager.onNoSpellMatch += SpellManagerOnOnSpellFailed;
            //_sequenceManager.onElementValidated += HandleElementRecognized;
            //_castscript.onSpellCastComplete += DisableHandVFX;
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            SpellManager.onSpellValidation -= HandleCastRecognized;
            SpellManager.onNoSpellMatch -= SpellManagerOnOnSpellFailed;
            //_sequenceManager.onElementValidated -= HandleElementRecognized;
            //_castscript.onSpellCastComplete -= DisableHandVFX;
        }

        private void Start()
        {
            _defaultColor = _handDataRight._renderer.materials[1].GetColor("_MainColor");
        }

        private void SpellManagerOnOnSpellFailed()
        {
            DisableBothHandEffects();
            SpellWrongIndication(_cancellationTokenSource.Token);
        }

        internal async void SpellWrongIndication(CancellationToken token)
        {
            try
            {
                float delay = 0.5f;
                int loopAmount = 2;

                for (int i = 0; i < loopAmount; i++)
                {
                    await UniTask.WaitForSeconds(delay, cancellationToken: token);

                    _handDataRight._renderer.materials[1].SetColor("_MainColor", Color.red);
                    _handDataLeft._renderer.materials[1].SetColor("_MainColor", Color.red);

                    await UniTask.WaitForSeconds(delay, cancellationToken: token);

                    _handDataRight._renderer.materials[1].SetColor("_MainColor", _defaultColor);
                    _handDataLeft._renderer.materials[1].SetColor("_MainColor", _defaultColor);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("SpellWrongIndication was canceled");
            }
        }

        internal void SetHandEffects(bool isDefaultColor, Spell currentSpell)
        {
            if (isDefaultColor)
            {
                if (currentSpell._duoSpell)
                {
                    _handDataRight._material = currentSpell._primaryConfig._handMaterial;
                    _handDataRight._renderer.materials[1].SetColor("_MainColor", currentSpell._primaryConfig._handColor);
                    _handDataLeft._material = currentSpell._secondaryConfig._handMaterial;
                    _handDataLeft._renderer.materials[1].SetColor("_MainColor", currentSpell._secondaryConfig._handColor);
                }
                else
                {
                    _handDataRight._material = currentSpell._primaryConfig._handMaterial;
                    _handDataRight._renderer.materials[1].SetColor("_MainColor", currentSpell._primaryConfig._handColor);
                    _handDataLeft._material = currentSpell._primaryConfig._handMaterial;
                    _handDataLeft._renderer.materials[1].SetColor("_MainColor", currentSpell._primaryConfig._handColor);
                }
            }
            else
            {
                _handDataRight._renderer.materials[1].SetColor("_MainColor", _defaultColor);
                _handDataLeft._renderer.materials[1].SetColor("_MainColor", _defaultColor);
            }
        }

        internal void HandleElementRecognized(Gesture gesture)
        {
            GameObject r = null;
            GameObject l = null;
            Debug.Log(gesture._rightHandShape);
            switch (gesture._rightHandShape)
            {
                case HandShape.Start:
                    DisableBothHandEffects();
                    break;
                case HandShape.QuickCast:
                    // effect should be at hands and play there instantly
                    // This doesn't work since quick cast is aparrently a seperate thing.
                    break;
                default:
                    if (gesture._visualEffectPrefab != null)
                    {
                        //if (!_spellManager.IsSpellUnlockedForGesture(gesture))
                        //{
                        //    Debug.Log($"'{gesture.name}' is not unlocked");
                        //    return;
                        //}
                        r = Instantiate(gesture._visualEffectPrefab, _prefabContainerR.transform, false);
                        l = Instantiate(gesture._visualEffectPrefab, _prefabContainerL.transform, false);
                        _lastGesture = gesture;
                        SetVFXContainerPositions();
                    }
                    break;
            }

            if (r != null && l != null)
            {
                //This sets a timer on the visual effect to destroy when it is done playing.
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

        public void HandleQuickCast() //will subscribe to quick cast event
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

                MoveVfxToHand(_cancellationTokenSource.Token,
                    _handDataRight._prefabContainerTransform,
                    _handDataRight._palm);
                MoveVfxToHand(_cancellationTokenSource.Token,
                    _handDataLeft._prefabContainerTransform,
                    _handDataLeft._palm);
            }
        }

        private void DisableHandVFX(HandData hand)
        {
            if (hand._currentEffect != null)
                hand._currentEffect.Stop();
        }

        private void DisableBothHandEffects()
        {
            if (_handDataRight != null)
                _handDataRight._currentEffect.Stop();
            if (_handDataLeft != null)
                _handDataLeft._currentEffect.Stop();
        }

        private async void MoveVfxToHand(CancellationToken token, Transform effect, Transform dest)
        {
            try
            {
                float delay = 0.03f;

                //remove transform so it goes to world
                effect.parent = null;
                // loop lerp
                for (int i = 0; i < 10; i++)
                {
                    await UniTask.WaitForSeconds(delay, cancellationToken: token);
                    effect.position = Vector3.Lerp(effect.position, dest.position, 0.45f);
                }
                // add new transform of hand
                effect.parent = dest;
                effect.localPosition = Vector3.zero;

                //if last gesture was earth, stop the vfx (so the touch spell works)
                if (_lastGesture != null && _lastGesture._rightHandShape == HandShape.Earth)
                {
                    _currentEffectL.Stop();
                    _currentEffectR.Stop();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("MoveVfxToHand was canceled");
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
            _handDataRight._prefabContainerTransform.parent = _magicCircleTarget;
            _handDataLeft._prefabContainerTransform.parent = _magicCircleTarget;
            _handDataRight._prefabContainerTransform.localPosition = Vector3.zero;
            _handDataLeft._prefabContainerTransform.localPosition = Vector3.zero;
            _handDataRight._prefabContainer.SetActive(true);
        }

        internal void HandleCastRecognized(bool recognized)
        {
            //activate the second vfx and move the current vfx to the hands
            //vfx in the magic circle, make a separate vfx in the magic circle and enable both right and left container
            _prefabContainerL.SetActive(true);

            MoveVfxToHand(_cancellationTokenSource.Token, _handDataRight._prefabContainerTransform, _palmR);
            MoveVfxToHand(_cancellationTokenSource.Token, _handDataLeft._prefabContainerTransform, _palmL);
        }
    }
}

