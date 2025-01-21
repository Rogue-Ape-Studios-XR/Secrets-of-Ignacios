using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace RogueApeStudios.SecretsOfIgnacios.FinalSpellPages
{
    public class FinalSpellPageScript : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private VisualEffect _poofEffect;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _triggered;

        public static event Action onPagePickup;
        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _poofEffect.Stop();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        
        // Yeah this is really ugly and stupid but I am not changing the grab code and risking a conflict in the last week
        private void FixedUpdate()
        {
            // 14 is the layer the grab logic sets grabbed objects to
            // so by abusing this we can circumvent an event. The update only runs on 5 pages anyways so who cares
            if (_triggered) return;
            if (gameObject.layer == 14)
            {
                _poofEffect.Play();
                onPagePickup?.Invoke();
                // first just disable mesh renderer so its gone to the player, but won't mess with the vfx
                _meshRenderer.enabled = false;
                _triggered = true;
                HandlePagePickupAsync(_cancellationTokenSource.Token).Forget();   
            }
        }
        
        private async UniTaskVoid HandlePagePickupAsync(CancellationToken token)
        {
            try
            {
                await UniTask.WaitUntil(() => _poofEffect.aliveParticleCount == 0, cancellationToken: token);

                Destroy(gameObject);
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("HandlePagePickupAsync was canceled...");
            }
        }
    }
}
