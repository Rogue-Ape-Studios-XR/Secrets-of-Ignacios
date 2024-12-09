using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    [SerializeField] GameObject Inventory;
    [SerializeField] string Tag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            Inventory.SetActive(!Inventory.activeInHierarchy);
            Inventory.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            Inventory.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.3f);
        }
    }
}
