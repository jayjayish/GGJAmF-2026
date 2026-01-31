using UnityEngine;

public class EntityData : ScriptableObject
{
    [SerializeField] public int health;
    [SerializeField] public Color color;

    public Color getColor()
    {
        return color;
    }
    public void setColor(Color color)
    {
        this.color = color;
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

