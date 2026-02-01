using UnityEngine;

public class ReticleMouseFollower : MonoSingleton<ReticleMouseFollower>
{
    [SerializeField] private SpriteRenderer spriteRend;
    // Update is called once per frame
    void Update()
    {
        transform.position = InputManager.GetMouseWorldPosition();
    }

    public void SetAlpha(float alpha)
    {
        var spriteRendColor = spriteRend.color;
        spriteRendColor.a = alpha;
        spriteRend.color = spriteRendColor;
    }
}
