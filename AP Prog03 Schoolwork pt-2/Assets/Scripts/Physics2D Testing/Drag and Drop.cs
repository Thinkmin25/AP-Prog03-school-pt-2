using UnityEngine;

public class DragandDrop : MonoBehaviour
{
    public Rigidbody2D rb;
    public bool dragging = false;
    public Vector2 dragOffset= Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GrabOverlap())
            {
                dragging = true;
            }
        }

        if (dragging)
        {
            rb.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }

    bool GrabOverlap()
    {
        rb = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)).attachedRigidbody;
        return rb != null;
    }
}
