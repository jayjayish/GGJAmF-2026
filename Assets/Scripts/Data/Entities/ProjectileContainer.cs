using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Data.Entities
{
    [CreateAssetMenu(fileName = "so-projectileContainer", menuName = "ScriptableObjects/Projectile/so-projectileContainer", order = 1)]
    public class ProjectileContainer : ScriptableObject
    {
        public SerializedDictionary<GlobalTypes.ProjectileTypes, ProjectileData> projectileContainer = new SerializedDictionary<GlobalTypes.ProjectileTypes, ProjectileData>();
    }
}