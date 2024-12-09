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
            {
                if (!spell._duoSpell)
                    CreatePool(spell._primaryConfig._spellPrefab.name,
                        spell._primaryConfig._spellPrefab,
                        spell._primaryConfig._poolSize);
                else
                {
                    CreatePool(spell._primaryConfig._spellPrefab.name,
                        spell._primaryConfig._spellPrefab,
                        spell._primaryConfig._poolSize);

                    CreatePool(spell._secondaryConfig._spellPrefab.name,
                        spell._secondaryConfig._spellPrefab,
                        spell._secondaryConfig._poolSize);
                }
            }

        }

        public void CreatePool(string objectType, GameObject prefab, int initialSize)
        {
            string poolKey = objectType.Replace("(Clone)", "").Trim();

            if (_objectPools.ContainsKey(poolKey))
                return;

            _objectPools[poolKey] = new();

            int currentCount = _objectPools[poolKey].Count;
            int itemsToAdd = initialSize - currentCount;

            for (int i = 0; i < itemsToAdd; i++)
            {
                GameObject projectile = GameObject.Instantiate(prefab, _poolParent);
                projectile.SetActive(false);
                _objectPools[poolKey].Enqueue(projectile);
            }
        }

        public GameObject GetObject(string poolKey, GameObject prefab, Transform transform)
        {
            if (!_objectPools.ContainsKey(poolKey))
            {
                Debug.LogError($"No pool exists for this object type: {poolKey}");
                return null;
            }

            Queue<GameObject> pool = _objectPools[poolKey];

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

        public void ReturnObject(string objectType, GameObject obj)
        {
            string poolKey = objectType.Replace("(Clone)", "").Trim();

            if (!_objectPools.ContainsKey(poolKey))
            {
                Debug.LogError($"No pool exists for object type: {poolKey}");
                return;
            }

            obj.SetActive(false);
            obj.transform.position = _poolParent.position;
            _objectPools[poolKey].Enqueue(obj);
        }
    }
}
