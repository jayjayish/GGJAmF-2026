using UnityEngine;

namespace Data.Entities
{
    [CreateAssetMenu(fileName = "so-projectileSettings", menuName = "ScriptableObjects/Projectile/so-projectileSettings", order = 1)]
    public class ProjectileData : EntityData
    {
        public GlobalTypes.ProjectileTypes projType;
        public GameObject projPrefab;
    }
}