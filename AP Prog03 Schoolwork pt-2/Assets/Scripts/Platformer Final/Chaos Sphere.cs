using UnityEngine;

public class ChaosSphere : MonoBehaviour
{
    Rigidbody2D rb;
    CircleCollider2D circleCollider;
    public int startingDirection = 1;
    Vector2 previousVelocity = Vector2.zero;
    float velocity = 12f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        rb.linearVelocity = new Vector2(velocity * startingDirection, -velocity);
    }

    // Update is called once per frame
    void Update()
    {
        // Used in math relating to the sphere's velocity; the velocity on impact is low, being that it's running into terrain
        previousVelocity = rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Gets a reflection of the normal angle on impact to bounce the sphere further
        float sphereAngle = Mathf.Atan2(previousVelocity.y, previousVelocity.x) * Mathf.Rad2Deg;
        float terrainAngle = Mathf.Atan2(collision.GetContact(0).normal.y, collision.GetContact(0).normal.x) * Mathf.Rad2Deg;

        float differenceAngle = Mathf.DeltaAngle(sphereAngle, terrainAngle);
        float finalAngle = (sphereAngle + (differenceAngle * 2)) * Mathf.Deg2Rad;

        rb.linearVelocity = new Vector2(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle)) * -velocity;
        if (rb.linearVelocity.magnitude <= 0.01)
        {
            Destroy(gameObject);
        }
    }
}
