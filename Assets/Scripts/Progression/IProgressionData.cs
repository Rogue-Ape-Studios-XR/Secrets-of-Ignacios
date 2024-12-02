using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Progression
{
    public interface IProgressionData { }

    public class SpellUnlockData : IProgressionData
    {
        public Spell Spell { get; set; }
    }

    
    //idk implement it
    public class AreaUnlockData : IProgressionData
    {
        public GameObject AreaName { get; set; }
    }
}
