using UnityEngine;

public class BasicMob : Character
{
    private Player player;

    private float knockBackForce =>
        data is EnemyData enemyData ? enemyData.knockBackForce : 0f;

    private bool isKnockedBack = false;
    private int knockBackFrames = 0;

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

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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

        if (data.movementSpeed <= 0f)
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
        transform.position += new Vector3(dir.x, dir.y, 0f) * data.movementSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("knocked back");
        var hitPlayer = collision.collider != null ? collision.collider.GetComponentInParent<Player>() : null;
        if (hitPlayer == null)
        {
            return;
        }
        isKnockedBack = true;
    }

    private void BounceBackFromPlayer()
    {
        if (knockBackForce <= 0f)
        {
            return;
        }

        
        knockBackFrames++;
        if (knockBackFrames > 10) {
            isKnockedBack = false;
            knockBackFrames = 0;
        }

        var away = transform.position - player.transform.position;
        Debug.Log(away);
        away.z = 0f;

        var dir = away.normalized;
        transform.position += new Vector3(dir.x, dir.y, 0f) * knockBackForce;
    }
}
