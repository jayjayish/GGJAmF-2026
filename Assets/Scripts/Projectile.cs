using Data;
using Data.Entities;
using DG.Tweening;
using UnityEngine;
using Utility;

public class Projectile : Entity
{
    [SerializeField] public Collider2D hitBox;
    [SerializeField] protected Rigidbody2D rb;

    [Header("Projectile Stats")]
    public bool isPlayer;
    [SerializeField] public int attackDamage = 1;
    public Vector2 moveDirection;

    protected override bool ColliderIsTrigger => true;

    protected override void Awake()
    {
        base.Awake();

        // Use the collider managed/ensured by Entity as the projectile hitbox.
        if (hitBox == null)
        {
            hitBox = hurtBox != null ? hurtBox : GetComponent<Collider2D>();
        }
        isPlayer = false;
        gameObject.layer = LayerMask.NameToLayer("Enemy Projectiles");

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        transform.position += Time.deltaTime * movementSpeed * new Vector3(moveDirection.x, moveDirection.y, 0f) ;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other object is a projectile.
        var otherProjectile = other != null ? other.GetComponentInParent<Projectile>() : null;
        if (otherProjectile != null)
        {
            // Same team projectile → ignore and do nothing.
            if (otherProjectile.isPlayer == isPlayer)
            {
                Physics2D.IgnoreCollision(hitBox, other, true);
                return;
            }
            
            bool isSameColor = Mathf.Abs(Mathf.DeltaAngle(otherProjectile.ColorAngle, ColorAngle)) < 30;
            // Enemy projectile hit player projectile
            if (isSameColor)
            {
                Debug.Log("enemy proj hit by same color, dies");
                // this projectile is absorbed by the other projectile
                isDead = true;
            } else {
            // TODO: Decide how to handle projectile damage exchange for different colors
                isDead = true;
            }
            return;
        }

        if (other.GetComponent<Player>() != null) {
            isDead = true;
        }
    }
    
    public override void TakeDamage(int amount, int attackColorAngle)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        // Enemy projectil does not take scaled damage
        health = Mathf.Max(0, health - amount);

        if (health <= 0)
        {            
            isDead = true;
        }
    }

    public void SetSpriteThroughDict(Sprite sprite)
    {
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = sprite;
    }
}
