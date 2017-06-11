using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public int id;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            NetworkManager.instance.GetComponent<NetworkManager>().CollectItem(id);
        }
    }

    /*
    void OnMouseDown()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().CollectItem(id);
    }
    */
}
