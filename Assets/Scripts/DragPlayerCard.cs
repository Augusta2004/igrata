using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPlayerCard : MonoBehaviour {

    void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(pos.x > 4.4)
        {
            pos.x = 4.4f;
        }
        if (pos.x < -5)
        {
            pos.x = -5f;
        }
        if (pos.y > 4.1)
        {
            pos.y = 4.1f;
        }
        if (pos.y < 1)
        {
            pos.y = 1f;
        }


        transform.position = (pos);
    }
}
