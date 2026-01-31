using UnityEngine;

public class EntityData : ScriptableObject
{
    [SerializeField] protected int health;
    [SerializeField] protected Color color;

    public getColor()
    {
        return color;
    }
    protected setColor(Color color)
    {
        this.color = color;
    }
    public getHealth()
    {
        return health;
    }
    public setHealth(int health)
    {
        this.health = health;
    }

    protected void takeDamage(int damage) 
    {
        health -= damage;
    }

    protected void heal(int amount)
    {
        health += amount;
    }
}

