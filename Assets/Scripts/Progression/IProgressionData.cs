using RogueApeStudios.SecretsOfIgnacios.Spells;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Progression
{
    public interface IProgressionData { }

    public class SpellUnlockData : IProgressionData
    {
        public Spell Spell { get; set; }
    }

    public class AreaUnlockData : IProgressionData
    {
        public GameObject Area { get; set; }
    }
}
  
