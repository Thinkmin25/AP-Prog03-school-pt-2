using System.Collections.Generic;
using UnityEngine;

public class OverlapCollider : MonoBehaviour
{
    public SpriteRenderer sr;
    public Color shapeColor;
    public Collider2D col;
    public List <Collider2D> collider2Ds = new List<Collider2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        shapeColor = new Color(0, 0, 40 * Physics2D.OverlapCollider(col, collider2Ds));
        sr.color = shapeColor;
    }
}
