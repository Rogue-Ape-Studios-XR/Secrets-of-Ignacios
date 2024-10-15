using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.HeadDetection
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] GameObject BlackBox;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                BlackBox.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                BlackBox.SetActive(false);
            }
        }
    }
}
