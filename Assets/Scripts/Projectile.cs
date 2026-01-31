using UnityEngine;

public class Projectile : Entity
{
    [SerializeField] public Collider2D hitBox;

    [Header("Projectile Stats")]
    [SerializeField] public bool isPlayer;
    [SerializeField] public float size = 1f;
    [SerializeField] public int attackDamage = 1;

    protected override bool ColliderIsTrigger => true;

    protected override void Awake()
    {
        base.Awake();

        // Use the collider managed/ensured by Entity as the projectile hitbox.
        if (hitBox == null)
        {
            hitBox = hurtBox != null ? hurtBox : GetComponent<Collider2D>();
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
            
            // Player projectile hit enemy projectile
            if (isPlayer)
            {
                if (otherProjectile.ColorAngle == ColorAngle) {
                    // this projectile grows in size
                    size += otherProjectile.size;   // TODO: choose how size increases
                    return;
                }
            }
            // Enemy projectile hit player projectile
            else
            {
                if (otherProjectile.ColorAngle == ColorAngle) {
                    // this projectile is absorbed by the other projectile
                    isDead = true;
                    return;
                }
            }
            
            // TODO: Decide how to handle projectile damage exchange
            TakeDamage(otherProjectile.attackDamage);
        }

        if (other.GetComponent<Character>() != null) {
            isDead = true;
        }

    }
}
