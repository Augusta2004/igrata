using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMap : MonoBehaviour {

    public GameObject map;

    private void OnMouseDown()
    {
        map.SetActive(!map.activeSelf);
    }
}
