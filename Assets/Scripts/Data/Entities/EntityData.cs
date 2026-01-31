using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "so-entityData", menuName = "ScriptableObjects/so-entityData", order = 1)]
public class EntityData : ScriptableObject
{
    public int health;
    public int ColorAngle;

    public void takeDamage(int damage) 
    {
        health -= damage;
    }

    public void heal(int amount)
    {
        health += amount;
    }
}

