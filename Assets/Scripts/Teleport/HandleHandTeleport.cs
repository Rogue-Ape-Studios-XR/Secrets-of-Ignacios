using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace RogueApeStudios.SecretsOfIgnacios.Teleport
{
    public class HandleHandTeleport : MonoBehaviour
    {
        [SerializeField] private XRRayInteractor _teleportInteractor;

        [SerializeField] private TeleportationProvider _teleportationProvider; 
        // This is the "Teleport" gameobject under the "Locomotion" gameobject in the XROrigin

        private bool _isTeleportActive = false;

        private void Start()
        {
            _teleportInteractor.gameObject.SetActive(false);
        }

        public void ActivateTeleport()
        {
            if (!_isTeleportActive)
            {
                _teleportInteractor.gameObject.SetActive(true);
                _isTeleportActive = true;
            }
        }

        public void DeactivateTeleport()
        {
            if (_isTeleportActive)
            {
                _teleportInteractor.gameObject.SetActive(false);
                _isTeleportActive = false;
            }
        }

        public void TriggerTeleport()
        {
            if (_isTeleportActive && _teleportInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                TeleportRequest request = new TeleportRequest()
                {
                    destinationPosition = hit.point,
                };
                _teleportationProvider.QueueTeleportRequest(request);
            }
        }
    }
}