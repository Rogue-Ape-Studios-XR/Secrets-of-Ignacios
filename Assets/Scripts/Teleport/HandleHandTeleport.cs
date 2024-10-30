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
        [SerializeField] private AlternateMovement _alternateMovement;

        private bool _isTeleportActive = false;
        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _interactorObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public void ActivateTeleport()
        {
            if (!_isTeleportActive)
            {
                _interactorObject.SetActive(true);
                _isTeleportActive = true;
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
            _alternateMovement.enabled = false;

            if (_isTeleportActive && _teleportInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                TeleportRequest request = new()
                {
                    destinationPosition = hit.point,
                };
                _teleportationProvider.QueueTeleportRequest(request);
            }
            EnableMovement(_cancellationTokenSource.Token);

        }

        private async void EnableMovement(CancellationToken token)
        {
            try
            {
                await UniTask.WaitForSeconds(0.1f, cancellationToken: token);
                _alternateMovement.enabled = true;
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("EnableMovement was canceled");
            }
        }
    }
}