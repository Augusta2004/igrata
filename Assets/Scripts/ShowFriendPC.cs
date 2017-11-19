using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFriendPC : MonoBehaviour {

    public string username;
    public string id;

    private void OnMouseDown()
    {
        Debug.Log("Show friend PC");
        Player.instance.GetComponent<Player>().ShowOCPlayerCard(username + "_player", true, id);

        Debug.Log(Player.instance.GetComponent<Player>().name);
    }
}
