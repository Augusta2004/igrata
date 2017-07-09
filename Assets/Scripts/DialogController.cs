using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour {

    [HideInInspector]
    public string item_id;


    public void ConfirmCollect()
    {
        Debug.Log(item_id);
        NetworkManager.instance.GetComponent<NetworkManager>().CollectItem(item_id, NetworkManager.localUsername);
    }

    public void ConfirmBuy()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().BuyItem(item_id);
    }

    public void CancelDialog()
    {
        this.gameObject.SetActive(false);
    }
}
