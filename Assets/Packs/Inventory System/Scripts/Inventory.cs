using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<string> _tags;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var tag in _tags)
        {
            if (other.CompareTag(tag))
            {
                other.transform.parent = transform;
                if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.linearDamping = 1000;
                    rb.angularDamping = 1000;
                    rb.useGravity = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var tag in _tags)
        {
            if (other.CompareTag(tag))
            {
                other.transform.parent = null;
                if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.linearDamping = 0;
                    rb.angularDamping = 0.05f;
                    rb.useGravity = true;
                }
            }
        }
        
    }
}
