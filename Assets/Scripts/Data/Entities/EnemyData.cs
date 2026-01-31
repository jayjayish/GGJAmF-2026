using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "so-enemyData", menuName = "ScriptableObjects/so-enemyData", order = 1)]
public class EnemyData : EntityData
{
    public int speed;
    public int attackDamage;
    public int knockBackForce; // determines how far the slime is knocked back when hit
}

