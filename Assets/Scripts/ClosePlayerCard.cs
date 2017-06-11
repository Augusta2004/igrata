using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePlayerCard : MonoBehaviour {

    void OnMouseDown()
    {
        //this.transform.parent.gameObject.SetActive(false);
        GameObject playerCard = GameObject.Find("PlayerCard") as GameObject;
        playerCard.transform.GetChild(0).gameObject.SetActive(false);

        GameObject playerCardOC = GameObject.Find("PlayerCardOC") as GameObject;
        playerCardOC.transform.GetChild(0).gameObject.SetActive(false);
    }
}
