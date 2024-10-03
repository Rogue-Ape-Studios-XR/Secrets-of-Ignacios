using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios
{

    public class GrimoireUIController : MonoBehaviour
    {
        private bool _grimioreActive = false;

        [SerializeField]
        private GameObject _grimiore;

        public void SummonGrimiore()
        {
            Debug.Log(_grimioreActive + "pre");
            if (!_grimioreActive)
            {
                _grimioreActive = true;
                _grimiore.SetActive(true);
            }
            else if (_grimioreActive) 
            {
                _grimioreActive = false;
                _grimiore.SetActive(false);
            }
            Debug.Log(_grimioreActive + "after");
        }
    }
}
