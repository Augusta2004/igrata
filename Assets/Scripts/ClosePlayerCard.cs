using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePlayerCard : MonoBehaviour {

    void OnMouseDown()
    {
        //this.transform.parent.gameObject.SetActive(false);
        GameObject playerCard = GameObject.Find("PlayerCard") as GameObject;
        playerCard.transform.Find("DragPlayerCard").transform.position = new Vector3(-1.09f, 3.29f, -1.2f);
        playerCard.transform.GetChild(0).gameObject.SetActive(false);

        /*GameObject itemsHolder = playerCard.transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("ItemsHolder").gameObject;
        GameObject showItems = playerCard.transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("ShowItems").gameObject;

        var x = itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x;
        if (itemsHolder.activeSelf)
        {
            showItems.transform.position = new Vector2(showItems.transform.position.x - x, showItems.transform.position.y);
        }
        itemsHolder.SetActive(false);

        playerCard.transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("SortCanvas").transform.Find("SortHolder").gameObject.SetActive(false);
        */


        GameObject playerCardOC = GameObject.Find("PlayerCardOC") as GameObject;
        playerCardOC.transform.Find("DragPlayerCard").transform.position = new Vector3(-1.09f, 3.29f, -1.2f);
        playerCardOC.transform.GetChild(0).gameObject.SetActive(false);
    }
}
