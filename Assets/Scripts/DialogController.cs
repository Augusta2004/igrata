using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour {

    [HideInInspector]
    public string item_id;
    [HideInInspector]
    public bool refreshPage = false;

    public void ConfirmCollect()
    {
        Debug.Log(item_id);
        NetworkManager.instance.GetComponent<NetworkManager>().CollectItem(item_id, NetworkManager.localUsername);
        this.gameObject.SetActive(false);
    }

    public void ConfirmBuy()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().BuyItem(item_id);
        this.gameObject.SetActive(false);
    }

    public void CancelDialog()
    {
        if (!refreshPage)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            Application.ExternalEval("document.location.reload(true)");
            this.gameObject.SetActive(false);
        }
    }
}
