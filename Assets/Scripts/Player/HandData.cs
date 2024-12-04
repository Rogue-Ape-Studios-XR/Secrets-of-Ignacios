using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Player
{
    public class HandData : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] internal Transform _handTransform;

        [Header("Visual")]
        [SerializeField] internal VisualEffect _chargeEffect;
        [SerializeField] internal VisualEffect _currentEffect;
        [SerializeField] internal GameObject _prefabContainer;
        [SerializeField] internal Transform _prefabContainerTransform;
        [SerializeField] internal Transform _palm;
        [SerializeField] internal LineRenderer _lineRenderer;
        [SerializeField] internal Renderer _renderer;
        [SerializeField] internal Material _material;

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
    }
}
