using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

    public int id;
    public GameObject dialog;

    private bool isTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered 
            && this.gameObject.CompareTag("Walk")
            && other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.name == NetworkManager.localUsername)
            {
                CollectItem();
            }
        }
    }

    private void OnMouseDown()
    {
        if (this.gameObject.CompareTag("Buy"))
        {
            dialog.SetActive(true);
            dialog.GetComponent<DialogController>().item_id = id;
            dialog.transform.Find("Canvas").transform.Find("Yes").gameObject.SetActive(true);
            dialog.transform.Find("Canvas").transform.Find("Cancel").gameObject.SetActive(true);
            dialog.transform.Find("Canvas").transform.Find("OK").gameObject.SetActive(false);

            dialog.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = "Buy this item?";
        }
        else if(!this.gameObject.CompareTag("Walk"))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        Debug.Log("collect item");

        dialog.SetActive(true);
        dialog.GetComponent<DialogController>().item_id = id;
        dialog.transform.Find("Canvas").transform.Find("Yes").gameObject.SetActive(true);
        dialog.transform.Find("Canvas").transform.Find("Cancel").gameObject.SetActive(true);
        dialog.transform.Find("Canvas").transform.Find("OK").gameObject.SetActive(false);

        dialog.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = "Collect this item?";

        isTriggered = true;
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        isTriggered = false;
    }

    /*
    void OnMouseDown()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().CollectItem(id);
    }
    */
}
