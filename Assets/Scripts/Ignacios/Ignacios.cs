using RogueApeStudios.SecretsOfIgnacios.FinalSpellPages;
using RogueApeStudios.SecretsOfIgnacios.Gestures;
using UnityEngine;

public class Ignacios : MonoBehaviour
{
    private Rigidbody[] _ragdollRigidbodies;
    [SerializeField] private Animator _animator;

    private void Start()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }

    private void HandleIgnaciosSpawning()
    {
        
    }

    private void DisableRagdoll()
    {
        foreach (Rigidbody rb in _ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
        
        if (_animator != null)
            _animator.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Untagged")
            return;
        
        EnablePermanentRagdoll();
    }

    private void EnablePermanentRagdoll()
    {
        foreach (Rigidbody rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }
    }
}
