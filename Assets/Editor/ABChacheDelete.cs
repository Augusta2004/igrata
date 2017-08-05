using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ABChacheDelete : MonoBehaviour
{

    [MenuItem("Window/Delete AB Chache")]
    public static void Open()
    {
        Caching.ClearCache();

        Debug.Log("Cache cleared");
    }
}