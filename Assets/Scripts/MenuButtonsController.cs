using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonsController : MonoBehaviour {

    public GameObject emojis;
    public GameObject moves;
    public GameObject friends;

    private void OnMouseDown()
    {
        if(this.name == "Emojis")
        {
            Debug.Log("emojis");
            if (emojis.activeSelf)
            {
                emojis.SetActive(false);
            }
            else
            {
                emojis.SetActive(true);
            }

            moves.SetActive(false);
            friends.SetActive(false);
        }
        else if (this.name == "Moves")
        {
            Debug.Log("moves");
            if (moves.activeSelf)
            {
                moves.SetActive(false);
            }
            else
            {
                moves.SetActive(true);
            }

            emojis.SetActive(false);
            friends.SetActive(false);
        }
        else if (this.name == "Friends")
        {
            Debug.Log("friends");
            if (friends.activeSelf)
            {
                friends.SetActive(false);
            }
            else
            {
                friends.SetActive(true);
                friends.transform.Find("FriendlistHolder").gameObject.SetActive(true);
                friends.transform.Find("FriendRequestHolder").gameObject.SetActive(false);

                if (FriendsController.updateFriendList)
                {
                    NetworkManager.instance.GetComponent<NetworkManager>().ShowFriends();
                }
                else
                {
                    //TODO: check from DB if we should update our friend list

                    //NetworkManager.instance.GetComponent<NetworkManager>().CheckFriendsUpdate();
                }
            }

            emojis.SetActive(false);
            moves.SetActive(false);
        }
    }
}
