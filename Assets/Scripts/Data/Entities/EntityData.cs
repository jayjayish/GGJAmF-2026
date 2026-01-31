using UnityEngine;

[CreateAssetMenu(fileName = "so-entityData", menuName = "ScriptableObjects/so-entityData", order = 1)]
public class EntityData : ScriptableObject
{
    public int health;
    public Color color;

    public Color getColor()
    {
        return color;
    }
    public void setColor(Color newColor)
    {
        color = newColor;
    }
    public int getHealth()
    {
        return health;
    }
    public void setHealth(int health)
    {
        this.health = health;
    }

    public void takeDamage(int damage) 
    {
        health -= damage;
    }

    public void heal(int amount)
    {
        health += amount;
    }
}

