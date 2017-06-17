using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buy : MonoBehaviour {

    public int id;

    private void OnMouseDown()
    {
        string name = this.gameObject.name;
        Debug.Log("Buy " + name);

        NetworkManager.instance.GetComponent<NetworkManager>().BuyItem(id);
    }
}
