using RogueApeStudios.SecretsOfIgnacios.Puzzle;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Water
{
    internal abstract class Washable : WaterInteractable 
    {
        [SerializeField] private Renderer _noise; //when used: _noise.material.[name of shader graph]
        private float _grime;

        private void Wash()
        {

        }
    }
}
