using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{

    public string id;
    private GameObject dialog;
    public bool isForCollect = false;

    private bool isTriggered = false;

    public bool collectOnClick = false;
    private bool collectOnMove = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered
            && this.gameObject.CompareTag("Walk")
            && other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.name == NetworkManager.localUsername)
            {
                Debug.Log("Collide without click");

                if (collectOnMove)
                {
                    Debug.Log("Collect on move");
                    CollectItem();
                    collectOnMove = false;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (!isForCollect)
        {
            Debug.Log("GONNA BUY THIS SHIT");
            dialog = GameObject.Find("ItemDialog").transform.Find("ItemDialogHolder").gameObject;
            dialog.SetActive(true);
            dialog.GetComponent<DialogController>().item_id = id;
            dialog.transform.Find("Canvas").transform.Find("YesBuy").gameObject.SetActive(true);
            dialog.transform.Find("Canvas").transform.Find("Yes").gameObject.SetActive(false);
            dialog.transform.Find("Canvas").transform.Find("Cancel").gameObject.SetActive(true);

            dialog.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = "Buy this item?";
        }
        else if (!this.gameObject.CompareTag("Walk"))
        {
            Debug.Log("Click");
            if (collectOnClick)
            {
                Debug.Log("Collect on click");
                CollectItem();
            }
        }
        else
        {
            Debug.Log("Set collect on move");
            collectOnMove = true;
        }
    }

    private void CollectItem()
    {
        Debug.Log("collect item");

        dialog = GameObject.Find("ItemDialog").transform.Find("ItemDialogHolder").gameObject;
        dialog.SetActive(true);
        dialog.GetComponent<DialogController>().item_id = id;
        dialog.transform.Find("Canvas").transform.Find("Yes").gameObject.SetActive(true);
        dialog.transform.Find("Canvas").transform.Find("YesBuy").gameObject.SetActive(false);
        dialog.transform.Find("Canvas").transform.Find("Cancel").gameObject.SetActive(true);

        dialog.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = "Collect this item?";

        isTriggered = true;
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        isTriggered = false;
    }
}
