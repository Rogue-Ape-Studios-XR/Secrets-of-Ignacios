using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios
{

    public class GrimoireUIController : MonoBehaviour
    {
        private bool _grimioreActive = false;

        [SerializeField]
        private GameObject _grimiore;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SummonGrimiore()
        {
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
            Debug.Log("YES!");
        }

        public void LockGrimiorePos()
        {
            Rigidbody _grimiorebody = _grimiore.GetComponent<Rigidbody>();

            _grimiorebody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
