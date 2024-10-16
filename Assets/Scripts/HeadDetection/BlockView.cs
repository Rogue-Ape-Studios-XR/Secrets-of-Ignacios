using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.HeadDetection
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] GameObject BlackBox;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 3)
            {
                BlackBox.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 3)
            {
                BlackBox.SetActive(false);
            }
        }
    }
}
