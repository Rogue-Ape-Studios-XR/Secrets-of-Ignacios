using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using RogueApeStudios.SecretsOfIgnacios.Services;
using RogueApeStudios.SecretsOfIgnacios.Spells;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Player.SpellMagicCircle
{
    public class HandVfxManager : MonoBehaviour
    {
        //line 71 vfx are instantiated and set for the hands, use this for quick cast

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
        [SerializeField] private ObjectPooler _objectPooler;

        [SerializeField] private HandData _handDataRight;
        [SerializeField] private HandData _handDataLeft;

        private Gesture _lastGesture;
        private GameObject _visualEffectRight;
        private GameObject _visualEffectLeft;
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
            if (gesture._name == "Start")
            {
                DisableBothHandEffects();
                return;
            }

            if (gesture._name == "Quick Cast")
                return;

            if (gesture._visualEffectPrefab == null)
                return;

            SetVFXContainerPositions();

            _lastGesture = gesture;
        }

        private void AssignEffect(GameObject visualEffect, HandData handData)
        {
            if (visualEffect == null)
                return;

            ReturnVisualEffect(_cancellationTokenSource.Token, visualEffect);

            if (visualEffect.TryGetComponent<VisualEffect>(out VisualEffect visual))
                handData._currentEffect = visual;
        }

        private void SetVFXContainerPositions()
        {
            _handDataRight._prefabContainerTransform.parent = _magicCircleTarget;
            _handDataLeft._prefabContainerTransform.parent = _magicCircleTarget;
            _handDataRight._prefabContainerTransform.localPosition = Vector3.zero;
            _handDataLeft._prefabContainerTransform.localPosition = Vector3.zero;
        }

        #endregion

        #region SpellManager

        internal void HandleOnSpellFailed()
        {
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

        internal void SetHandEffects(Spell currentSpell, bool usedQuickCast)
        {
            SetHandMaterials(currentSpell);

            _visualEffectRight = SetupVisualEffect(_handDataRight, _lastGesture);
            _visualEffectLeft = SetupVisualEffect(_handDataLeft, _lastGesture);

            _handDataRight._prefabContainer.SetActive(true);
            _handDataLeft._prefabContainer.SetActive(true);

            _visualEffectRight.transform.SetParent(_handDataRight._prefabContainerTransform);
            _visualEffectLeft.transform.SetParent(_handDataLeft._prefabContainerTransform);

            if (!usedQuickCast)
            {
                MoveVfxToHand(_cancellationTokenSource.Token,
                    _handDataRight._prefabContainerTransform,
                    _handDataRight._palmTransform);
                MoveVfxToHand(_cancellationTokenSource.Token,
                    _handDataLeft._prefabContainerTransform,
                    _handDataLeft._palmTransform);
            }
        }

        private void SetHandMaterials(Spell currentSpell)
        {
            ApplyMaterialAndColor(_handDataRight, currentSpell._primaryConfig);

            if (currentSpell._duoSpell)
                ApplyMaterialAndColor(_handDataLeft, currentSpell._secondaryConfig);
            else
                ApplyMaterialAndColor(_handDataLeft, currentSpell._primaryConfig);
        }

        private void ApplyMaterialAndColor(HandData handData, HandConfig config)
        {
            handData._renderer.material = config._handMaterial;
            handData._renderer.materials[1].SetColor("_MainColor", config._handColor);
        }

        private GameObject SetupVisualEffect(HandData handData, Gesture gesture)
        {
            if (gesture._visualEffectPrefab != null)
            {
                GameObject visualEffect = _objectPooler.GetObject(gesture._visualEffectPrefab.name,
                    gesture._visualEffectPrefab, handData._prefabContainerTransform);

                AssignEffect(visualEffect, handData);

                return visualEffect;
            }

            return null;
        }

        internal void ResetHandColors()
        {
            _handDataRight._renderer.materials[1].SetColor("_MainColor", _handDataRight._defaultColor);
            _handDataLeft._renderer.materials[1].SetColor("_MainColor", _handDataLeft._defaultColor);
        }

        private async void MoveVfxToHand(CancellationToken token, Transform prefabContainer, Transform dest)
        {
            try
            {
                float delay = 0.03f;

                //remove transform so it goes to world
                prefabContainer.parent = null;
                // loop lerp
                for (int i = 0; i < 10; i++)
                {
                    await UniTask.WaitForSeconds(delay, cancellationToken: token);
                    prefabContainer.position = Vector3.Lerp(prefabContainer.position, dest.position, 0.45f);
                }
                // add new transform of hand
                prefabContainer.parent = dest;
                prefabContainer.localPosition = Vector3.zero;

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
            if (hand == _handDataRight)
            {
                _visualEffectRight.transform.parent = null;
                _objectPooler.ReturnObject(_visualEffectRight.name, _visualEffectRight);
            }
            else
            {
                _visualEffectLeft.transform.parent = null;
                _objectPooler.ReturnObject(_visualEffectLeft.name, _visualEffectLeft);
            }

            if (hand._prefabContainer != null)
                hand._prefabContainer.SetActive(false);
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
            if (_visualEffectRight != null && _visualEffectRight.activeSelf)
            {
                _visualEffectRight.transform.parent = null;
                _objectPooler.ReturnObject(_visualEffectRight.name, _visualEffectRight);
                _handDataRight._prefabContainer.SetActive(false);
            }
            if (_visualEffectLeft != null && _visualEffectLeft.activeSelf)
            {
                _visualEffectLeft.transform.parent = null;
                _objectPooler.ReturnObject(_visualEffectLeft.name, _visualEffectLeft);
                _handDataLeft._prefabContainer.SetActive(false);
            }
        }

        private async void ReturnVisualEffect(CancellationToken token, GameObject effect)
        {
            try
            {
                if (effect.TryGetComponent<VisualEffect>(out VisualEffect visual))
                {
                    await UniTask.WaitUntil(() => visual.aliveParticleCount == 0, cancellationToken: token);
                    effect.transform.parent = null;
                    _objectPooler.ReturnObject(effect.name, effect);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Visual Effect wasn't destroyed (likely, the game went out of play mode)");
            }
        }
    }
}

