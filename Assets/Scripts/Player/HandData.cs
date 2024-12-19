using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Player
{
    [RequireComponent(typeof(AudioSource))]
    public class HandData : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] internal GameObject _prefabContainer;
        [SerializeField] internal LineRenderer _lineRenderer;
        [SerializeField] internal Transform _prefabContainerTransform;
        [SerializeField] internal Transform _palmTransform;
        [SerializeField] internal Transform _spellSpawnPoint;
        [SerializeField] private AudioSource _audioSourceWater;

        [Header("Visual")]
        [SerializeField] internal Renderer _renderer;
        [SerializeField] internal VisualEffect _chargeEffect;

        internal GameObject _currentEffect;
        internal Material _defaultMaterial;
        internal Transform _spellPrefab;
        internal Color _defaultColor;
        internal bool _canCast = false;
        internal bool _isCasting = false;

        private void Start()
        {
            _chargeEffect.Stop();
            _defaultMaterial = _renderer.material;
            _defaultColor = _renderer.materials[1].GetColor("_MainColor");
        }

        internal HandConfig GetHandConfig(SpellManager spellManager, bool isRightHand)
        {
            if (!spellManager.CurrentSpell._duoSpell)
                return spellManager.CurrentSpell._primaryConfig;
            else
            {
                if (isRightHand)
                    return spellManager.CurrentSpell._primaryConfig;
                else
                    return spellManager.CurrentSpell._secondaryConfig;
            }
        }

        internal void PlaySound()
        {
            _audioSourceWater.Play();
        }

        internal void StopSound()
        {
            _audioSourceWater.Stop();
        }

        internal void TogglePrefabContainer(bool active)
        {
            _prefabContainer.SetActive(active);
        }

        public void SetChargeEffect(VisualEffectAsset chargeEffect)
        {
            _chargeEffect.visualEffectAsset = chargeEffect;
        }
    }
}
