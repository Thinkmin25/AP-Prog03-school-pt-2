using UnityEngine;

public class OverlapPoint : MonoBehaviour
{
    public Vector2 scanPos = Vector2.zero;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.OverlapPoint(scanPos))
        {
            spriteRenderer.color = Color.yellow;
        }
        else spriteRenderer.color = Color.magenta;
    }
}
