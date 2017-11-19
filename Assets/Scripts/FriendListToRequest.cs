using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListToRequest : MonoBehaviour {
    //That's the most stupid for script....

    private void OnMouseDown()
    {        
        if (this.name == "ToRequests" && this.transform.parent.gameObject.name == "FriendlistHolder") //If we're currently showing our friends, change to requests
        {
            Debug.Log("Fr to req");
            this.transform.parent.gameObject.SetActive(false);
            GameObject.Find("RequestDialogHolder").transform.Find("RequestDialogBox").gameObject.SetActive(false);
            GameObject.Find("Friendlist").transform.Find("Holder").transform.Find("FriendRequestHolder").gameObject.SetActive(true);

            //Check if we should update our request, nigga - nigga e losha duma

            Debug.Log("PAGE" + NetworkManager.currentFriendsPage);
            if (FriendsController.updateFriendRequests || NetworkManager.currentFriendsPage != 1)
            {
                FriendsController.updateFriendRequests = true;
                NetworkManager.instance.GetComponent<NetworkManager>().ShowRequests(1);

                NetworkManager.currentFriendsPage = 1;
                //If something goes wrong - comment below
                //FriendsController.updateFriendRequests = false;
            }
            else
            {
                NetworkManager.instance.GetComponent<NetworkManager>().CheckForRequestUpdate();
                NetworkManager.currentFriendsPage = 1;
            }

        }
        else if (this.name == "ToFriendlist" && this.transform.parent.gameObject.name == "FriendRequestHolder")
        {
            Debug.Log("req to fr");
            this.transform.parent.gameObject.SetActive(false);
            GameObject.Find("Friendlist").transform.Find("Holder").transform.Find("FriendlistHolder").gameObject.SetActive(true);

            if (FriendsController.updateFriendList)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().ShowFriends(1);
                NetworkManager.currentFriendsPage = 1;
            }
        }
    }
}
