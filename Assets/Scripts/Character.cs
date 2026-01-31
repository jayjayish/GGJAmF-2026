using UnityEngine;

public class Character : Entity
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void checkIsDead()
    {
        if (health <= 0)
        {
            Die();
        }
    }

}
