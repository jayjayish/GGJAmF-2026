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
    [SerializeField] public bool isPlayer;
    [SerializeField] public float sizeScaling = 1.2f;
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
        
        // only player projectiles have rigidbody
        if (isPlayer) {
            gameObject.layer = LayerMask.NameToLayer("Player Projectiles");

            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        else {
            gameObject.layer = LayerMask.NameToLayer("Enemy Projectiles");
        }

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

    private void OnTriggerEnter2D(Collider2D other)
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
            
            bool isSameColor = Mathf.Abs(Mathf.DeltaAngle(otherProjectile.ColorAngle, ColorAngle)) < 15;
            // Player projectile hit enemy projectile
            if (isPlayer)
            {
                if (isSameColor) {
                    // this projectile grows in size
                    transform.localScale *= sizeScaling; // Makes the projectile 20% bigger
                    return;
                }
            }
            // Enemy projectile hit player projectile
            else
            {
                Debug.Log("enemy projectile");
                if (isSameColor)
                {
                    Debug.Log("enemy proj hit by same color, dies");
                    // this projectile is absorbed by the other projectile
                    Die();
                    return;
                }
            }
            
            // TODO: Decide how to handle projectile damage exchange
            TakeDamage(otherProjectile.attackDamage, otherProjectile.ColorAngle);
        }

        if (isPlayer) {            
            if (other.GetComponent<BasicMob>() != null) {
                Die();
            }
        }
        else {
            if (other.GetComponent<Player>() != null) {
                Die();
            }
        }
    }

    public void SetSpriteThroughDict(Sprite sprite)
    {
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = sprite;
    }
}
