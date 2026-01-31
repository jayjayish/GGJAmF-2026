using UnityEngine;

public class BasicMob : Entity
{
    private Player player;

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
        MoveTowardPlayer();
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
}
