using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
   // [HideInInspector]
    //public GameObject playerCard;

    void OnMouseDown()
    {
        //playerCard.SetActive(true);
        //Debug.Log(this.gameObject.name);
        GameObject playerCard = GameObject.Find("PlayerCard") as GameObject;
        GameObject playerCardOC = GameObject.Find("PlayerCardOC") as GameObject;

        bool isLocal = this.gameObject.GetComponent<CharacterController>().isLocalPlayer;
        if(isLocal)
        {
            Debug.Log("local PC");

            playerCard.transform.GetChild(0).gameObject.SetActive(true);
            playerCardOC.transform.GetChild(0).gameObject.SetActive(false);
            Debug.Log(playerCard.transform.GetChild(0));


            GameObject itemsHolder = GameObject.Find("PlayerCard")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("ItemsHolder").gameObject;

            foreach (Transform child in itemsHolder.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            GameObject.Find("PlayerCard")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("PlayerCardCanvas")
                .transform.Find("Fish").GetComponent<Text>().text = NetworkManager.fish.ToString();

            NetworkManager.instance.GetComponent<NetworkManager>().GetPlayerItems();
        }
        else
        {
            Debug.Log("Other PC");

            GameObject.Find("PlayerCardOC")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("PlayerCardCanvas")
                .transform.Find("PlayerName").GetComponent<Text>().text = this.name;


            NetworkManager.instance.GetComponent<NetworkManager>().GetOtherPlayerItems(this.name);

            playerCard.transform.GetChild(0).gameObject.SetActive(false);
            playerCardOC.transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log(playerCardOC.transform.GetChild(0));
            //TODO open other player card
        }
    }
}
