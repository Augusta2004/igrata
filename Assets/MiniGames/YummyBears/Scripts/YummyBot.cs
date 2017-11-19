using UnityEngine;
using System.Collections;

public class YummyBot : MonoBehaviour
{
    public int points = 0;
    public float angle = 0;
    public YummyRandomColor rc;
    private int collisionTimer = 0;

    private void Update()
    {
        if (angle != 0)
        {
            transform.position += new Vector3(angle, 0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        angle *= -1;
        if(collision.gameObject.tag == "Bot")
        {
            GetComponent<SpriteRenderer>().color = rc.GetRandomColor();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        collisionTimer++;
        if(collisionTimer > 10 && !collision.gameObject.name.Contains("Wall"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionTimer = 0;
    }
}
