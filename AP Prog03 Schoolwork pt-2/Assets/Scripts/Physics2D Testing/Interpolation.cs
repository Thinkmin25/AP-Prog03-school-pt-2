using UnityEngine;

public class Interpolation : MonoBehaviour
{
    public Rigidbody2D orbittingChild;
    public float orbitRadius = 2;
    public float timer = 0;
    public Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orbittingChild = transform.GetChild(0).GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        orbittingChild.position = rb.position + new Vector2(Mathf.Cos(timer), Mathf.Sin(timer)) * orbitRadius;
    }
}
