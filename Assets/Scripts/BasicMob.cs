using UnityEngine;

public class BasicMob : Character
{
    private Player player;

    private EnemyData enemyData =>
        data is EnemyData d ? d : null;

    private bool isKnockedBack = false;
    [SerializeField] protected float knockBackForce;
    [SerializeField] protected float knockBackDuration;
    private int knockBackFrames = 0;

    public int getAttackDamage() {
        return enemyData.attackDamage;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        player = Player.Instance;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isKnockedBack = false;
        knockBackFrames = 0;
        knockBackForce = enemyData.knockBackForce;
        knockBackDuration = enemyData.knockBackDuration;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Move();
    }

    // override this for different mobs
    protected virtual void Move() {        
        if (isKnockedBack) {
            BounceBackFromPlayer();
        } else {
            MoveTowardPlayer();
        }
    }

    private void MoveTowardPlayer()
    {
        if (player == null)
        {
            player = Player.Instance;
            return;
        }

        if (movementSpeed <= 0f)
        {
            return;
        }

        var toPlayer = player.transform.position - transform.position;
        toPlayer.z = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f)
        {
            return;
        }

        var dir = toPlayer.normalized;
        transform.position += new Vector3(dir.x, dir.y, 0f) * movementSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var hitPlayer = collision.collider != null ? collision.collider.GetComponentInParent<Player>() : null;
        if (hitPlayer != null)
        {
            isKnockedBack = true;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        var hitProjectile = collider != null ? collider.GetComponentInParent<Projectile>() : null;
        Debug.Log("is hitting projectile: " + hitProjectile);
        if (hitProjectile != null && hitProjectile.isPlayer) {
            TakeDamage(hitProjectile.attackDamage, hitProjectile.ColorAngle);
        }
    }

    private void BounceBackFromPlayer()
    {
        if (knockBackForce <= 0f)
        {
            return;
        }

        
        knockBackFrames++;
        if (knockBackFrames > knockBackDuration) {
            isKnockedBack = false;
            knockBackFrames = 0;
        }

        var away = transform.position - player.transform.position;
        away.z = 0f;

        var dir = away.normalized;
        transform.position += new Vector3(dir.x, dir.y, 0f) * knockBackForce * Time.deltaTime;
    }
}
