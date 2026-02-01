using UnityEngine;
using Data;
using Entities;

public class ShooterMob : BasicMob
{
    [SerializeField] private float shootIntervalSeconds = 1.0f;
    [SerializeField] private float shootRange = 25f;
    [SerializeField] private GlobalTypes.ProjectileTypes projectile = GlobalTypes.ProjectileTypes.EnemyBasic;

    private float shootTimer;

    protected override void OnEnable()
    {
        base.OnEnable();
        shootTimer = 0f;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        TryShootAtPlayer();
    }

    private void TryShootAtPlayer()
    {
        if (shootIntervalSeconds <= 0f)
        {
            return;
        }

        var player = Player.Instance;
        if (player == null)
        {
            return;
        }

        var toPlayer = player.transform.position - transform.position;
        toPlayer.z = 0f;

        if (shootRange > 0f && toPlayer.sqrMagnitude > shootRange * shootRange)
        {
            return;
        }

        shootTimer += Time.deltaTime;
        if (shootTimer < shootIntervalSeconds)
        {
            return;
        }

        shootTimer -= shootIntervalSeconds;

        var dir = ((Vector2)toPlayer).normalized;
        if (dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        var proj = ProjectileManager.SpawnProjectile(
            projectile,
            transform.position,
            ColorAngle
        );

        proj.isPlayer = false;
        proj.moveDirection = dir;
    }
}
