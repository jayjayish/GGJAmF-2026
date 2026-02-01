using System;
using System.Collections.Generic;
using Data;
using Data.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Entities
{
    public static class ProjectileManager
    {
        private static Dictionary<GlobalTypes.ProjectileTypes, ObjectPool<Projectile>> _dictionaryPool = new ();
        private static ObjectPool<Projectile> _objectPool;
        private static ProjectileContainer _cachedProjectileData;


        public static void Initialize(Action onSuccess = null)
        {
            // Cache the scene settings async
            Addressables.LoadAssetAsync<ProjectileContainer>(GlobalAddresses.ProjectilesAddr).Completed += asyncHandle =>
            {
                if (asyncHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _cachedProjectileData = asyncHandle.Result;
                    onSuccess?.Invoke();
                }
                else
                {
                    UnityEngine.Debug.LogError("ProjectileContainer couldn't be loaded");
                }
            };
        }

        public static Projectile SpawnProjectile(GlobalTypes.ProjectileTypes type, Vector2 position, int colorAngle)
        {
            if (!_dictionaryPool.ContainsKey(type))
            {
                WarmPool(type);
            }

            Projectile proj = _dictionaryPool[type].Get();
            proj.transform.position = position;
            proj.ColorAngle = colorAngle;

            return proj;
        }

        private static void WarmPool(GlobalTypes.ProjectileTypes type)
        {
            if (!_cachedProjectileData.projectileContainer.TryGetValue(type, out ProjectileData projData))
            {
                Debug.LogError($"Projectile {type.ToString()} couldn't be loaded");
            }
            
            var newPool = new ObjectPool<Projectile>(() => OnCreateProj(projData), OnGetProj, OnReleaseProj);
            _dictionaryPool.Add(type, newPool);
        }

        private static void OnReleaseProj(Projectile proj)
        {
            proj.transform.position = Vector3.zero;
            proj.transform.rotation = Quaternion.identity;
            proj.transform.localScale = Vector3.one;
            proj.gameObject.SetActive(false);
        }

        private static void OnGetProj(Projectile proj)
        {
            proj.gameObject.SetActive(true);
        }

        private static Projectile OnCreateProj(ProjectileData data)
        {
            var obj = Object.Instantiate(data.projPrefab);
            return  obj.GetComponent<Projectile>();
        }
    }
}