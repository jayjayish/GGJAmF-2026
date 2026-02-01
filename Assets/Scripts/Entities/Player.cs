using System;
using Entities;
using UnityEngine;

public class Player : Character
{
    private static Player _instance;
    public static Player Instance => _instance;
    public PlayerData PlayerEntityDaya => (PlayerData)data;

    [SerializeField] private PlayerColorPicker colorPicker;
    [SerializeField] private Vector2 playerDirection;

    protected override void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("[MonoSingleton<" + typeof(Player).Name + ">]" + " duplicate instance was deleted");
            return;
        }

        _instance = this;
        
        base.Awake();
        // don't want player to be pushed
        rb.bodyType = RigidbodyType2D.Kinematic;
    }


    protected virtual void OnDestroy()
    {
        _instance = null;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        InputManager.AddMoveAction(OnMove);
        InputManager.AddAttackAction(Shoot);
        InputManager.AddLeftDownAction(OnLeft);
        InputManager.AddRightDownAction(OnRight);
        
        ReticleMouseFollower.Instance.gameObject.SetActive(true);
    }

    private void OnRight()
    {
        throw new NotImplementedException();
    }

    private void OnLeft()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    protected override void Update()
    {
        var moveVec = movementSpeed * Time.deltaTime * playerDirection;
        transform.position += new Vector3(moveVec.x, moveVec.y, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Take contact damage from other Entities (e.g. mobs).
        var otherMob = collision.collider != null ? collision.collider.GetComponentInParent<BasicMob>() : null;
        if (otherMob != null && otherMob.getAttackDamage() > 0)
        {
            TakeDamage(otherMob.getAttackDamage());
            Debug.Log("health: " + health);
            return;
        }

        var projectile = collision.collider != null ? collision.collider.GetComponentInParent<Projectile>() : null;
        if (projectile != null && !projectile.isPlayer)
        {
            TakeDamage(projectile.attackDamage);
            Debug.Log("health: " + health);
            return;
        }

    }

    private void OnMove(Vector2 vector)
    {
        playerDirection = vector.normalized;
    }

    private void Shoot()
    {
        var attack = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.TestCircle, transform.position);
        attack.moveDirection = (InputManager.GetMouseWorldPosition() - transform.position).normalized;
    }
}
