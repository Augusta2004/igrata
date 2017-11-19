using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChange : MonoBehaviour {

    Texture2D cursor;
    Texture2D handCursor;

    void Start()
    {
        handCursor = (Texture2D)Resources.Load("Sprites/Cursors/Hand");
        cursor = (Texture2D)Resources.Load("Sprites/Cursors/Arrow");
    }

    void OnMouseOver()
    {
        //Debug.Log("change cursor");
        Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }
}
