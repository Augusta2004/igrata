using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSkiObject : MonoBehaviour {


    void Update ()
    {
        int level = GameObject.Find("Bear").GetComponent<BearSkiController>().level;
        bool moveForward = GameObject.Find("Bear").GetComponent<BearSkiController>().moveForward;

        float move = -0.05f - 0.01f * level;
        if(moveForward)
        {
             move += -0.01f * level;
        }

        this.transform.position += new Vector3(0, move, 0);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("SkiObject"))
        {
            Destroy(collision.gameObject);
        }
    }
}
