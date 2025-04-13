using RogueApeStudios.SecretsOfIgnacios.FinalSpellPages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RogueApeStudios.SecretsOfIgnacios
{

    public class GrimoireUIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _grimoire;

        [SerializeField]
        private GameObject _braceletbutton;

        [SerializeField]
        private GameObject _player; //camera offset

        [SerializeField]
        private GameObject[] _chapters;

        [SerializeField]
        private GameObject[] _finalSpellPages;

        [SerializeField]
        private TextMeshProUGUI _finalSpellCounter;

        [SerializeField]
        private GameObject _finalSpellTab;
        
        [SerializeField]
        private float grimoireTeleportDistance = 100f;
        
        [SerializeField]
        private float summonCooldown = 0.5f;
        
        [SerializeField]
        private int startPage = 0;

        private bool _hasOpenedOnce = false;

        private float _lastSummonTime;

        private bool _wristGaze = false;
        private bool _headGaze = false;

        private int _currentPageCounter;

        private void Start()
        {
            FinalSpellPageScript.onPagePickup += UpdateCounter;
            _currentPageCounter = -1;
        }

        private void OnDestroy()
        {
            FinalSpellPageScript.onPagePickup -= UpdateCounter;
        }

        private void Update()
        {
            if (_headGaze)
            {
                _braceletbutton.SetActive(true);
            }
            else
            {
                _braceletbutton.SetActive(false);
            }
        }

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
            if (Time.time - _lastSummonTime < summonCooldown) return;

            _lastSummonTime = Time.time;

            Transform player = _player.transform;
            Vector3 playerPosition = player.position;
            Vector3 playerDirection = player.forward;
            Quaternion playerRotation = player.rotation;

            float distance = Vector3.Distance(playerPosition, _grimoire.transform.position);

            if (_grimoire.activeSelf)
            {
                if (distance >= grimoireTeleportDistance)
                {
                    Vector3 spawnPos = playerPosition + playerDirection * 0.5f;

                    _grimoire.transform.eulerAngles = new Vector3(
                        0,
                        playerRotation.eulerAngles.y + 180,
                        0
                    );

                    _grimoire.transform.position = spawnPos;
                }
                else
                {
                    _grimoire.SetActive(false);
                }
            }
            else
            {
                Vector3 spawnPos = playerPosition + playerDirection * 0.5f;

                _grimoire.transform.eulerAngles = new Vector3(
                    0,
                    playerRotation.eulerAngles.y + 180,
                    0
                );

                _grimoire.transform.position = spawnPos;
                _grimoire.SetActive(true);
            }
        }
     
        private void UpdateCounter()
        {
            _currentPageCounter++;
            if ( _currentPageCounter == 0)
            {
                _finalSpellTab.SetActive(true);
            }
            _finalSpellPages[_currentPageCounter].SetActive(true);
            _finalSpellCounter.SetText((_currentPageCounter + 1).ToString());
        }
    }
}
