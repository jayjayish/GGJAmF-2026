using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Data.Entities
{
    [CreateAssetMenu(fileName = "so-projectileSettings", menuName = "ScriptableObjects/Projectile/so-projectileSettings", order = 1)]
    public class ProjectileData : EntityData
    {
        public GlobalTypes.ProjectileTypes projType;
        public GameObject projPrefab;
        public SerializedDictionary<GlobalTypes.Color, Sprite> spriteColorVariants = new SerializedDictionary<GlobalTypes.Color, Sprite>();
    }
}
