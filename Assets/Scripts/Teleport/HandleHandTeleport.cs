using Cysharp.Threading.Tasks;
using RogueApeStudios.SecretsOfIgnacios.Player.Movement;
using System;
using System.Threading;
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
        [SerializeField] private GameObject _interactorObject;
        [SerializeField] private AlternateMovement? _alternateMovement;
        [SerializeField] private ForwardMovement? _forwardMovement;

        [SerializeField] private float _deactivationDelay = 0.75f;

        private bool _isTeleportActive = false;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _interactorObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public void DeactivateAfterDelay() => DeactivationAfterDelay(_cancellationTokenSource.Token, _deactivationDelay);

        public void ActivateTeleport()
        {
            if (!_isTeleportActive)
            {
                _interactorObject.SetActive(true);
                _isTeleportActive = true;
                ToggleMovement(_cancellationTokenSource.Token, false);
            }
        }

        public void DeactivateTeleport()
        {
            if (_isTeleportActive)
            {
                _interactorObject.SetActive(false);
                _isTeleportActive = false;
            }
        }

        public void TriggerTeleport()
        {
            if (_isTeleportActive && _teleportInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                TeleportRequest request = new()
                {
                    destinationPosition = hit.point,
                };
                _teleportationProvider.QueueTeleportRequest(request);
            }

            ToggleMovement(_cancellationTokenSource.Token, true);
        }

        private async void DeactivationAfterDelay(CancellationToken token, float delay)
        {
            try
            {
                await UniTask.WaitForSeconds(delay, cancellationToken: token);
                DeactivateTeleport();
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("DeactivateAfterDelay was canceled");
            }
        }

        private async void ToggleMovement(CancellationToken token, bool active)
        {
            try
            {
                await UniTask.WaitForSeconds(0.1f, cancellationToken: token);
                if (_forwardMovement is not null)
                    _forwardMovement.enabled = active;
                else if (_alternateMovement is not null)
                    _alternateMovement.enabled = active;
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("ToggleMovement was canceled");
            }
        }
    }
}