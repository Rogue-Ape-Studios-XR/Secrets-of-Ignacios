using System;
using UnityEngine;
using UnityEngine.UI;


namespace RogueApeStudios.SecretsOfIgnacios
{
    [RequireComponent(typeof(BoxCollider))]
    public class UIButtonTrigger : Button
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("FingerTip"))
            {
                gameObject.GetComponent<Button>().onClick.Invoke();
            }
        }
    }
}
