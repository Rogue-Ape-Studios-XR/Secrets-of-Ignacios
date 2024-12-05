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

        [SerializeField] private HandData _handDataRight;
        [SerializeField] private HandData _handDataLeft;

        private VisualEffect _currentEffectL;
        private VisualEffect _currentEffectR;
        private Gesture _lastGesture;

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        #region SequenceManager

        internal void ChangeColorOnGesture(Gesture gesture)
        {
            _handDataRight._renderer.materials[1].SetColor("_MainColor", gesture._color);
            _handDataLeft._renderer.materials[1].SetColor("_MainColor", gesture._color);
            _handDataRight._renderer.material = _handDataRight._defaultMaterial;
            _handDataLeft._renderer.material = _handDataLeft._defaultMaterial;
        }

        internal void HandleElementRecognized(Gesture gesture)
        {
            GameObject r = null;
            GameObject l = null;

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
                        //    return;

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
                    _handDataRight._currentEffect?.Stop();

                    _handDataRight._currentEffect = visualr;
                }
                if (l.TryGetComponent<VisualEffect>(out VisualEffect visuall))
                {
                    _handDataLeft._currentEffect?.Stop();

                    _handDataLeft._currentEffect = visuall;
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
                    _handDataRight._palmTransform);
                MoveVfxToHand(_cancellationTokenSource.Token,
                    _handDataLeft._prefabContainerTransform,
                    _handDataLeft._palmTransform);
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

        #endregion

        #region SpellManager

        internal void HandleOnSpellFailed()
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

                    _handDataRight._renderer.materials[1].SetColor("_MainColor", _handDataRight._defaultColor);
                    _handDataLeft._renderer.materials[1].SetColor("_MainColor", _handDataLeft._defaultColor);
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
                    _handDataRight._renderer.material = currentSpell._primaryConfig._handMaterial;
                    _handDataRight._renderer.materials[1].SetColor("_MainColor", currentSpell._primaryConfig._handColor);
                    _handDataLeft._renderer.material = currentSpell._secondaryConfig._handMaterial;
                    _handDataLeft._renderer.materials[1].SetColor("_MainColor", currentSpell._secondaryConfig._handColor);
                }
                else
                {
                    _handDataRight._renderer.material = currentSpell._primaryConfig._handMaterial;
                    _handDataRight._renderer.materials[1].SetColor("_MainColor", currentSpell._primaryConfig._handColor);
                    _handDataLeft._renderer.material = currentSpell._primaryConfig._handMaterial;
                    _handDataLeft._renderer.materials[1].SetColor("_MainColor", currentSpell._primaryConfig._handColor);
                }
            }
            else
            {
                _handDataRight._renderer.materials[1].SetColor("_MainColor", _handDataRight._defaultColor);
                _handDataLeft._renderer.materials[1].SetColor("_MainColor", _handDataLeft._defaultColor);
            }
        }

        internal void HandleCastRecognized(bool recognized)
        {
            //activate the second vfx and move the current vfx to the hands
            //vfx in the magic circle, make a separate vfx in the magic circle and enable both right and left container
            _prefabContainerL.SetActive(true);

            MoveVfxToHand(_cancellationTokenSource.Token, _handDataRight._prefabContainerTransform, _handDataRight._palmTransform);
            MoveVfxToHand(_cancellationTokenSource.Token, _handDataLeft._prefabContainerTransform, _handDataLeft._palmTransform);
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
                    _handDataRight._currentEffect.Stop();
                    _handDataLeft._currentEffect.Stop();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("MoveVfxToHand was canceled");
            }
        }

        #endregion

        #region Cast

        internal void DisableHandVFX(HandData hand)
        {
            if (hand._currentEffect != null)
                hand._currentEffect.Stop();
        }

        internal async UniTask ChargeEffect(HandData handData, float delay)
        {
            if (handData._canCast)
            {
                handData._lineRenderer.enabled = true;
                handData._chargeEffect.Play();
                await UniTask.WaitForSeconds(delay, cancellationToken: _cancellationTokenSource.Token);
                handData._lineRenderer.enabled = false;
            }
            else
                handData._lineRenderer.enabled = false;
        }

        #endregion

        private void DisableBothHandEffects()
        {
            _handDataRight._currentEffect.Stop();
            _handDataLeft._currentEffect.Stop();
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
    }
}

