using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.FireRoom
{
    public class TilePuzzleFeedback : MonoBehaviour
    {
        [SerializeField] private List<VisualEffect> _correctChoiceEffects;
        // Script will activate and deactivate vfx based on events
        void Start()
        {
            //deactivate all vfx in the list
            DeactivateEffects();
        }
        public void ActivateEffect(int index)
        {
            _correctChoiceEffects[index].Play();
        }
        public void DeactivateEffects() 
        {
            foreach (var effect in _correctChoiceEffects)
            {
                effect.Stop();
            }
        }
    }
}
