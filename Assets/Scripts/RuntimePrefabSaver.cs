#if UNITY_EDITOR
using Mono.Cecil;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class RuntimePrefabSaver : MonoBehaviour
    {
        // Reference to the GameObject you want to save as a prefab
        [SerializeField] private GameObject[] _objectsToSave;

        // Optional: Specify a custom folder and name for the prefab
        [SerializeField] private string _prefabName = "NewPrefab";

        private string _prefabFolderPath = "Assets/Prefabs/";
        // Input Action for saving the prefab
        private InputAction saveAction;

        private void OnEnable()
        {
            // Set up the Input Action for Spacebar
            saveAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");

            // Subscribe to the performed event
            saveAction.performed += ctx => SaveObjectAsPrefab();

            // Enable the Input Action
            saveAction.Enable();
        }

        private void OnDisable()
        {
            // Disable the Input Action
            saveAction.Disable();
        }

        // This method duplicates the object and saves it as a prefab
        public void SaveObjectAsPrefab()
        {
            if (_objectsToSave == null)
            {
                Debug.LogError("No object assigned to save.");
                return;
            }

            foreach (var _objectToSave in _objectsToSave)
            {
                // Duplicate the object
                GameObject copy = Instantiate(_objectToSave);
                copy.transform.SetPositionAndRotation(_objectToSave.transform.position, _objectToSave.transform.rotation);
                copy.transform.localScale = _objectToSave.transform.localScale;

                // Ensure the folder exists
                string folderPath = _prefabFolderPath;
                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

#if UNITY_EDITOR
                // Generate a unique file path for the prefab
                string localPath = _prefabFolderPath + _prefabName + ".prefab";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath); // Make sure path is unique

                // Save the prefab
                PrefabUtility.SaveAsPrefabAsset(copy, localPath);

                // Destroy the instantiated copy in the scene
                DestroyImmediate(copy);

                // Refresh the AssetDatabase to make sure it's updated
                AssetDatabase.Refresh();

                Debug.Log("Prefab saved at: " + localPath);
#else
        Debug.LogError("Prefab saving is only supported in the Unity Editor.");
#endif
            }
        }
    }
}
