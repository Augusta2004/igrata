using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListToRequest : MonoBehaviour {
    //That's the most stupid for script....

    private void OnMouseDown()
    {
        if (this.name == "FriendRequests") //If we're currently showing our friends, change to requests
        {
            this.transform.parent.gameObject.SetActive(false);
            GameObject.Find("Friendlist").transform.Find("Holder").transform.Find("FriendRequestHolder").transform.Find("RequestDialogBox").gameObject.SetActive(false);
            GameObject.Find("Friendlist").transform.Find("Holder").transform.Find("FriendRequestHolder").gameObject.SetActive(true);

            //Check if we should update our request, nigga

            if (FriendsController.updateFriendRequests)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().ShowRequests();

                //If something goes wrong - comment below
                FriendsController.updateFriendRequests = false;
            }
            else
            {
                NetworkManager.instance.GetComponent<NetworkManager>().CheckForRequestUpdate();
            }

        }
        else if (this.name == "FriendList")
        {
            this.transform.parent.gameObject.SetActive(false);
            GameObject.Find("Friendlist").transform.Find("Holder").transform.Find("FriendlistHolder").gameObject.SetActive(true);

            if (FriendsController.updateFriendList)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().ShowFriends();
            }
        }
    }
}
