using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Data.Entities
{
    [CreateAssetMenu(fileName = "so-enemyContainer", menuName = "ScriptableObjects/Enemy/so-enemyContainer", order = 1)]
    public class EnemyContainer : ScriptableObject
    {
        public SerializedDictionary<GlobalTypes.EnemyTypes, EnemyData> enemyContainer = new SerializedDictionary<GlobalTypes.EnemyTypes, EnemyData>();
    }
}