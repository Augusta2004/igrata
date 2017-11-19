using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public string movementName;
    public float x;
    public float y;

    private void OnMouseDown()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().Movement(movementName, x, y);
        this.transform.parent.gameObject.SetActive(false);
    }
}
