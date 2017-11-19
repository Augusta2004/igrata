using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPlayerCard : MonoBehaviour {

    void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(pos.x > 6.157f)
        {
            pos.x = 6.157f;
        }
        if (pos.x < -6.17)
        {
            pos.x = -6.17f;
        }
        if (pos.y > 4.296)
        {
            pos.y = 4.296f;
        }
        if (pos.y < 1.367f)
        {
            pos.y = 1.367f;
        }


        transform.position = new Vector3 (pos.x, pos.y, -1.2f);
    }
}
