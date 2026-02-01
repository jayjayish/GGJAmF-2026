using UnityEngine;

public class Boss1 : EnemyBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        movementSpeed = 2;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
