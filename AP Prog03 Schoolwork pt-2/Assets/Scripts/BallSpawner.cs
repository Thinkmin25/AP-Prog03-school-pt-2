using System.Collections;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public int ballCount = 400;
    public float spawnTimer = 0.01f;
    public bool randomColor = true;

    IEnumerator Start()
    {
        for (int i = 0; i < ballCount; i++)
        {
            GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity, transform);
            ball.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle.normalized, ForceMode2D.Impulse);

            if (randomColor)
            {
                ball.GetComponent<SpriteRenderer>().material.color = Random.ColorHSV();
            }

            yield return new WaitForSeconds(spawnTimer);
        }
    }

}
