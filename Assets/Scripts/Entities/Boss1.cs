using UI;
using UnityEngine;
using Entities;

public class Boss1 : BasicMob
{
    [SerializeField] private ParticleSystem particleSys;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        movementSpeed = 2;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
        BossHealthBarController.Instance.SetBarAction(true);
        BossHealthBarController.Instance.HealthPercent = Health / data.health;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        particleSys.transform.position = transform.position;
        particleSys.Play();
    }
}
