using System;
using UnityEngine;
using UnityEngine.UI;


namespace RogueApeStudios.SecretsOfIgnacios
{
    [RequireComponent(typeof(BoxCollider))]
    public class UIButtonTrigger : MonoBehaviour
    {
        [SerializeField] private GrimoireUIController _bookcontroller;
        [SerializeField] private int _selectedChapter;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("FingerTip"))
            {
                if (gameObject.CompareTag("UIButtonTab"))
                {
                    _bookcontroller.ActivateChapter(_selectedChapter);
                }
                else if (gameObject.CompareTag("UIButtonBracelet"))
                {
                    _bookcontroller.Summongrimoire();
                }
            }
        }
    }
}
