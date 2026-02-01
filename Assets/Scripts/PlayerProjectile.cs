using Data.Entities;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    [SerializeField] public float sizeScaling = 1.2f;
    public float maxSizeScale = 5f;

    protected override void Awake() {
        base.Awake();

        // only player projectiles have rigidbody
        isPlayer = true;
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

    protected override void OnTriggerEnter2D(Collider2D other)
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
            // Player projectile hit enemy projectile
            if (isSameColor) {
                // this projectile grows in size
                if (transform.localScale.x < maxSizeScale) {
                    transform.localScale *= sizeScaling; // Makes the projectile 20% bigger
                }
            } else {
            // TODO: Decide how to handle projectile damage exchange for different colors
                isDead = true;
            }
            return;
        }
         
        if (other.GetComponent<BasicMob>() != null) {            
            isDead = true;
        }
    }

    public virtual void SetData(ProjectileData projData)
    {
        base.SetData(projData);
        isPlayer = true;
    }
}
