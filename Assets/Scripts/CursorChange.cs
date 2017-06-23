using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChange : MonoBehaviour {

    Texture2D cursor;

    void Start()
    {
        cursor = (Texture2D)Resources.Load("Sprites/Cursors/Hand");
    }

    void OnMouseOver()
    {
        Debug.Log("change cursor");
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
