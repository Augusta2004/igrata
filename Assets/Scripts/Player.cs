using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            //TODO open our player card
        }
        else
        {
            Debug.Log("Other PC");

            playerCard.transform.GetChild(0).gameObject.SetActive(false);
            playerCardOC.transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log(playerCardOC.transform.GetChild(0));
            //TODO open other player card
        }
    }
}
