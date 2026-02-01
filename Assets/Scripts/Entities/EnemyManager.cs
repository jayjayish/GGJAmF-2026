using System;
using System.Collections.Generic;
using Colors;
using Data;
using Data.Entities;
using Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Entities
{
    public static class EnemyManager
    {
        private static Dictionary<GlobalTypes.EnemyTypes, ObjectPool<BasicMob>> _dictionaryPool = new ();
        private static ObjectPool<BasicMob> _objectPool;
        private static EnemyContainer _cachedEnemyData;


        public static void Initialize(Action onSuccess = null)
        {
            // Cache the scene settings async
            Addressables.LoadAssetAsync<EnemyContainer>(GlobalAddresses.EnemysAddr).Completed += asyncHandle =>
            {
                if (asyncHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _cachedEnemyData = asyncHandle.Result;
                    onSuccess?.Invoke();
                }
                else
                {
                    Debug.LogError("EnemyContainer couldn't be loaded");
                }
            };
        }

        public static BasicMob SpawnEnemy(GlobalTypes.EnemyTypes type, Vector2 position, int colorAngle)
        {
            if (!_dictionaryPool.ContainsKey(type))
            {
                WarmPool(type);
            }

            BasicMob spawnEnemy = _dictionaryPool[type].Get();
            spawnEnemy.transform.position = position;
            spawnEnemy.ColorAngle = colorAngle;

            var enemyData = GetEnemyData(type);
            spawnEnemy.SetData(enemyData);
            
            return spawnEnemy;
        }

        private static void WarmPool(GlobalTypes.EnemyTypes type)
        {
            if (!_cachedEnemyData.enemyContainer.TryGetValue(type, out EnemyData projData))
            {
                Debug.LogError($"Enemy {type.ToString()} couldn't be loaded");
            }
            var poolParent = new GameObject($"{type.ToString()}_Pool");
            var newPool = new ObjectPool<BasicMob>(() => OnCreateProj(projData, poolParent.transform), OnGetProj, OnReleaseProj);
            _dictionaryPool.Add(type, newPool);
        }

        private static void OnReleaseProj(BasicMob proj)
        {
            proj.transform.position = Vector3.zero;
            proj.transform.rotation = Quaternion.identity;
            proj.transform.localScale = Vector3.one;
            proj.gameObject.SetActive(false);
        }

        private static void OnGetProj(BasicMob proj)
        {
            proj.gameObject.SetActive(true);
        }

        private static BasicMob OnCreateProj(EnemyData data, Transform parent)
        {
            var obj = Object.Instantiate(data.projPrefab,  parent);
            return  obj.GetComponent<BasicMob>();
        }

        public static EnemyData GetEnemyData(GlobalTypes.EnemyTypes type)
        {
            return _cachedEnemyData.enemyContainer[type];
        }
    }
}
