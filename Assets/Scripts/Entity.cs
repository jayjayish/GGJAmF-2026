using NaughtyAttributes;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    protected EntityData data;

    protected bool isDead;

    protected Collider2D hurtBox;

    // Override in subclasses (e.g. Projectile) as needed.
    protected virtual bool ColliderIsTrigger => false;

    protected virtual void Awake()
    {
        hurtBox = GetComponent<Collider2D>();
        if (hurtBox == null)
        {
            // Ensure there's at least some Collider2D present.
            hurtBox = gameObject.AddComponent<BoxCollider2D>();
        }

        hurtBox.isTrigger = ColliderIsTrigger;
    }

    protected virtual void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
        }

        col.isTrigger = ColliderIsTrigger;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    [Button]
    public void AddColor()
    {
        data.ColorAngle += 10;
    }
    
}
