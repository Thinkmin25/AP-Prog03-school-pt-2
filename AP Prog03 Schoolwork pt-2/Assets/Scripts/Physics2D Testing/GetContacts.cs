using UnityEngine;

public class GetContacts : MonoBehaviour
{
    public SpriteRenderer sr;
    public Color shapeColor;
    public float colorFizzle = 0;
    public Rigidbody2D rb;
    ContactPoint2D[] awesome = new ContactPoint2D[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        colorFizzle += rb.GetContacts(awesome);
        if (colorFizzle > 0)
        {
            colorFizzle -= Time.deltaTime;
        }
        shapeColor = new Color(0, 40 * colorFizzle, 0);
        sr.color = shapeColor;
    }
}
