using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.Puzzle.MainRoom
{
    public class PotionPuzzle : MonoBehaviour
    {
        [SerializeField] private GameObject _containedObject;
        [SerializeField] private int _ingredientAmount = 3;
        [SerializeField] private Renderer _potionWater;
        [SerializeField] private VisualEffect _splashEffect;

        private float _count;

        void Start() 
        { 
            _splashEffect.Stop();
        }

        private void OnTriggerEnter(Collider other)
        {
            _splashEffect.Play();
            if (other.gameObject.CompareTag("Ingredient"))
            {
                _count++;

                Color startColor = _potionWater.material.color;
                Color randomColor = new Color();
                float hue;
                float saturation;
                float brightness;
                Color.RGBToHSV(startColor, out hue, out saturation, out brightness);
                randomColor = Color.HSVToRGB(Random.value, saturation, brightness);

                _potionWater.material.color = randomColor;

                
                
                other.gameObject.SetActive(false);
            }
            else if (!other.gameObject.CompareTag("Ingredient") && !other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Potion"))
            {
                Debug.Log("Wrong!");
                if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    // Successfully found the Rigidbody component
                    Vector3 randomVector = new Vector3(Random.value,1,Random.value);

                    rb.AddForce(randomVector * 10, ForceMode.Impulse);
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
