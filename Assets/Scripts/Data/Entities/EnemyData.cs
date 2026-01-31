using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "so-enemyData", menuName = "ScriptableObjects/so-enemyData", order = 1)]
public class EnemyData : EntityData
{
    public int attackDamage;
    public float knockBackForce; // determines how far the slime is knocked back when hit
    public int knockBackDuration; // frames the knockback lasts
}

