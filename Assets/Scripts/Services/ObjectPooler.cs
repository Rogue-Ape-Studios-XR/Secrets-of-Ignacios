using RogueApeStudios.SecretsOfIgnacios.Spells;
using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Services
{
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private List<Spell> _spells;
        [SerializeField] private Transform _poolParent;

        private Dictionary<string, Queue<GameObject>> _objectPools = new();

        private void Awake()
        {
            _serviceLocator.RegisterService(this);

            foreach (var spell in _spells)
                CreatePool(spell._elementType.ToString(), spell._spellPrefab, /*spell._poolSize*/1);
        }

        private void CreatePool(string projectileType, GameObject prefab, int initialSize)
        {
            if (!_objectPools.ContainsKey(projectileType))
            {
                _objectPools[projectileType] = new();

                for (int i = 0; i < initialSize; i++)
                {
                    GameObject projectile = GameObject.Instantiate(prefab, _poolParent);
                    projectile.SetActive(false);
                    _objectPools[projectileType].Enqueue(projectile);
                }
            }
        }

        public GameObject GetProjectile(string objectType, GameObject prefab, Transform transform)
        {
            if (!_objectPools.ContainsKey(objectType))
            {
                Debug.LogError($"No pool exists for this object type: {objectType}");
                return null;
            }

            Queue<GameObject> pool = _objectPools[objectType];

            if (pool.Count <= 0)
            {
                print("adding new projectile to pool");
                GameObject newProjectile = GameObject.Instantiate(prefab, _poolParent);
                return newProjectile;
            }

            GameObject projectile = pool.Dequeue();
            projectile.transform.SetPositionAndRotation(transform.position, transform.rotation);
            projectile.SetActive(true);
            return projectile;
        }

        public void ReturnProjectile(string objectType, GameObject obj)
        {
            if (!_objectPools.ContainsKey(objectType))
            {
                Debug.LogError($"No pool exists for object type: {objectType}");
                return;
            }

            obj.SetActive(false);
            obj.transform.position = _poolParent.position;
            _objectPools[objectType].Enqueue(obj);
        }
    }
}
