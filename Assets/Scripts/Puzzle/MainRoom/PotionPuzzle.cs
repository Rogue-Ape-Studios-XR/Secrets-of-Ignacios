using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
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
        [SerializeField] private VisualEffect _potionCreateEffect;
        [SerializeField] private VisualEffect _ingredientWrongEffect;

        private float _count;

        #region Unitask

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        #endregion

        void Start() 
        { 
            _splashEffect.Stop();
            _potionCreateEffect.Stop();
            _ingredientWrongEffect.Stop();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Potion"))
            {
                _splashEffect.Play();
            }
            if (other.gameObject.CompareTag("Ingredient"))
            {
                _count++;

                Color startColor = _potionWater.material.color;
                Color randomColor = new Color();
                float hue;
                float saturation;
                float brightness;
                Color.RGBToHSV(startColor, out hue, out saturation, out brightness);
                randomColor = Color.HSVToRGB(UnityEngine.Random.value, saturation, brightness);
                ChangePotionColorSteps(_cancellationTokenSource.Token,startColor,randomColor,60);
                
                other.gameObject.SetActive(false);
            }
            else if (!other.gameObject.CompareTag("Ingredient") && !other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Potion"))
            {
                Debug.Log("Wrong!");
                if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    // Successfully found the Rigidbody component
                    Vector3 randomVector = new Vector3(UnityEngine.Random.value,1,UnityEngine.Random.value);
                    _ingredientWrongEffect.Play();
                    rb.AddForce(randomVector * 10, ForceMode.Impulse);
                    Debug.Log(rb);
                }
            }
            PotionCheck(_cancellationTokenSource.Token);
        }

        private async void ChangePotionColorSteps(CancellationToken token, Color startColor, Color endColor, float steps)
        {
            try
            {
                float oldHue;
                float oldSaturation;
                float oldBrightness;
                Color.RGBToHSV(startColor,out oldHue, out oldSaturation, out oldBrightness);

                float newHue;
                float newSaturation;
                float newBrightness;
                Color.RGBToHSV(endColor,out newHue, out newSaturation, out newBrightness);


                for (int i = 0; i < steps; i++)
                {
                    var percentage = (float)i / steps;
                    Color currentColor = Color.HSVToRGB(Mathf.Lerp(oldHue,newHue,percentage),oldSaturation,oldBrightness);
                    Debug.Log(currentColor);
                    await UniTask.WaitForSeconds(0.016f, cancellationToken: token);
                    _potionWater.material.color = currentColor;
                }
                _potionWater.material.color = endColor;


            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Example was Canceled...");
            }
        }
        private async void PotionCheck(CancellationToken token)
        {
            if (_count == _ingredientAmount)
            {
                _count = 0;
                await UniTask.WaitForSeconds(1f, cancellationToken: token);
                _potionCreateEffect.Play();
                await UniTask.WaitForSeconds(.2f, cancellationToken: token);
                _containedObject.SetActive(true);
            }
        }
    }
}
