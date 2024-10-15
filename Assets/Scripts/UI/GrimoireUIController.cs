using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios
{

    public class GrimoireUIController : MonoBehaviour
    {
        private bool _grimoireActive = false;

        [SerializeField]
        private GameObject _grimoire;


        [SerializeField]
        private GameObject[] _chapters;

        public void ActivateChapter(int chapterNum)
        {
            Debug.Log(chapterNum);
            foreach (var chapter in _chapters)
            {
                chapter.SetActive(false);
            }

            _chapters[chapterNum].SetActive(true);
        }

        public void Summongrimoire()
        {
            if (!_grimoireActive)
            {
                _grimoireActive = true;
                _grimoire.SetActive(true);
            }
            else if (_grimoireActive) 
            {
                _grimoireActive = false;
                _grimoire.SetActive(false);
            }
        }
    }
}
