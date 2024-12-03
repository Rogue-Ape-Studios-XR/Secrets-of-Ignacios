using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios
{

    public class GrimoireUIController : MonoBehaviour
    {
        //private bool _grimoireActive = false;

        [SerializeField]
        private GameObject _grimoire;

        [SerializeField]
        private GameObject _braceletbutton;

        [SerializeField]
        private GameObject _player; //camera offset


        [SerializeField]
        private GameObject[] _chapters;

        private bool _wristGaze = false;
        private bool _headGaze = false;

        public void WristHover(bool state)
        {
            _wristGaze = state;
        }

        public void HeadHover(bool state)
        {
            _headGaze = state;
        }

        public void ActivateChapter(int chapterNum)
        {
            foreach (var chapter in _chapters)
            {
                chapter.SetActive(false);
            }

            _chapters[chapterNum].SetActive(true);
        }

        public void Summongrimoire()
        {
            Transform player = _player.transform;
            Vector3 playerPosition = player.position;
            Vector3 playerDirection = player.forward;
            Quaternion playerRotation = player.rotation;

            Vector3 spawnPos = playerPosition + playerDirection * 0.5f;

            _grimoire.transform.eulerAngles = new Vector3(
                0,
                playerRotation.eulerAngles.y + 180,
                0
            );

            _grimoire.transform.position = spawnPos;
            
            /*
            if (!_grimoireActive)
            {
                _grimoireActive = true;
                _grimoire.SetActive(true);
            }
            else if (_grimoireActive) 
            {
                _grimoireActive = false;
                _grimoire.SetActive(false);
            }*/
        }

        private void Update()
        {
            if (_wristGaze && _headGaze)
            {
                _braceletbutton.SetActive(true);
            }
            else
            {
                _braceletbutton.SetActive(false);
            }
        }
    }
}
