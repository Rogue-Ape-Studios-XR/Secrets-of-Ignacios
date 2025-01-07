using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.MainRoom
{
    public class PotionPuzzle : MonoBehaviour
    {
        [SerializeField] private GameObject _containedObject;
        [SerializeField] private int _ingredientAmount = 3;
        private float _count;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Ingredient"))
            {
                _count++;
                other.gameObject.SetActive(false);
            }
            else if (!other.gameObject.CompareTag("Ingredient") && !other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Potion"))
            {
                Debug.Log("Wrong!");
                if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    // Successfully found the Rigidbody component
                    rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
                    Debug.Log(rb);
                }
            }
            PotionCheck();
        }

        private void PotionCheck()
        {
            if (_count == _ingredientAmount)
            {
                _containedObject.SetActive(true);
            }
        }
    }
}
