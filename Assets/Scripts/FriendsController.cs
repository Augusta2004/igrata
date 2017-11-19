using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsController : MonoBehaviour {

    public static FriendsController instance;

    public static bool updateFriendList;
    public static bool updateFriendRequests;

    private bool instantiateFriends;
    private bool instantiateRequests;
    
    private int statusSpritePath;
    public static string requestId;
    public static int requestOrder;

    public GameObject friendObject;
    public GameObject requestObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            //Destroy(gameObject);
        }
    }

    private void Start()
    {
        updateFriendList = true;
        updateFriendRequests = true;

        instantiateFriends = true;
        instantiateRequests = true;

        Debug.Log("Start FriendsController");
}

    public void ShowFriends(string[] friends)
    {
        Transform FriendsHolder = this.transform.Find("Holder").transform.Find("FriendlistHolder").transform.Find("FriendsHolder");
        if (instantiateFriends)
        {
            for (int i = 0; i < friends.Length; i++)
            {
                //Instantiate
                GameObject friendObj = Instantiate(friendObject, FriendsHolder) as GameObject;
                friendObj.transform.parent = FriendsHolder;

                friendObj.name = "Friend " + i;                

                float friendObjY = friendObj.transform.position.y - 0.4f * i;
                ///////////SELSKO NAMESTVANE/////////////
                friendObj.transform.position = new Vector3(friendObj.transform.position.x, friendObjY, friendObj.transform.position.z);
                /////////////////////////////////////////
                /////Player.instance.GetComponent<Player>().showOCPlayerCard("kon");
                FriendJSON friend = FriendJSON.CreateFromJSON(friends[i]);

                if (friend.is_logged && friend.server == NetworkManager.serverName)
                {
                    statusSpritePath = 7;
                }
                else
                {
                    statusSpritePath = 6;
                }

                friendObj.GetComponent<ShowFriendPC>().username = friend.username;
                friendObj.GetComponent<ShowFriendPC>().id = friend.id;
                friendObj.transform.Find("Status").GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Menu/friend_list")[statusSpritePath];
                friendObj.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = friend.username;
            }

            Debug.Log("Friends instantiated");
            instantiateFriends = false;
            updateFriendList = false;

            NetworkManager.instance.GetComponent<NetworkManager>().GetFriendsCount();
        }
        else
        {
            if (updateFriendList)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().GetFriendsCount();
                Debug.Log("UPDATE FRIENDS");
                int currentFriendsCount = friends.Length;
                int instantiatedFriendsCount = FriendsHolder.childCount;

                if (currentFriendsCount > instantiatedFriendsCount) //We have new friends
                {
                    int dif = currentFriendsCount - instantiatedFriendsCount;
                    int newFriendNumber = 0;
                    for (int i = 0; i < dif; i++)
                    {
                        GameObject friendObj = Instantiate(friendObject, FriendsHolder) as GameObject;
                        friendObj.transform.parent = FriendsHolder;

                        float friendObjY = friendObj.transform.position.y - 0.4f * (instantiatedFriendsCount + i);
                        ///////////SELSKO NAMESTVANE/////////////
                        friendObj.transform.position = new Vector3(friendObj.transform.position.x, friendObjY, friendObj.transform.position.z);
                        /////////////////////////////////////////

                        newFriendNumber = instantiatedFriendsCount + i;
                        friendObj.name = "Friend " + newFriendNumber;
                    }
                }
                else if(currentFriendsCount < instantiatedFriendsCount) //Someone removed us from their friend list _)_
                {
                    int friendNumber = 0;
                    for (int i = instantiatedFriendsCount; i > currentFriendsCount; i--)
                    {
                        friendNumber = i - 1;
                        Destroy(FriendsHolder.transform.Find("Friend " + friendNumber).gameObject);

                        Debug.Log("Iztrihme tuka neshto " + friendNumber);
                    }
                    Debug.Log("Iztrihme tuka neshto");
                    
                }
                
                int friendCounter = 0;
                Debug.Log("Total friends - " + friends.Length);
                foreach (Transform child in FriendsHolder)
                {
                    if (friendCounter < friends.Length)
                    {
                        Debug.Log(FriendsHolder.childCount);
                        FriendJSON friend = FriendJSON.CreateFromJSON(friends[friendCounter]);

                        if (friend.is_logged && friend.server == NetworkManager.serverName)
                        {
                            statusSpritePath = 7;
                        }
                        else
                        {
                            statusSpritePath = 6;
                        }

                        child.GetComponent<ShowFriendPC>().username = friend.username;
                        child.GetComponent<ShowFriendPC>().id = friend.id;
                        child.transform.Find("Status").GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Menu/friend_list")[statusSpritePath];
                        child.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = friend.username;

                        friendCounter++;
                        Debug.Log(friendCounter);
                    }
                }

                updateFriendList = false;
            }
        }
    }

    public void PrevPage(bool isRequests = false)
    {

        if (NetworkManager.currentFriendsPage > 1)
        {

            if (!isRequests)
            {
                updateFriendList = true;
                NetworkManager.instance.GetComponent<NetworkManager>().ShowFriends(NetworkManager.currentFriendsPage - 1);
            }
            else
            {
                updateFriendRequests = true;
                NetworkManager.instance.GetComponent<NetworkManager>().ShowRequests(NetworkManager.currentFriendsPage - 1);
            }

            NetworkManager.currentFriendsPage--;
        }
    }

    public void NextPage(bool isRequests = false)
    {
        int maxPage = Mathf.CeilToInt((float)NetworkManager.friendsCount / 10);
        if (isRequests)
        {
            maxPage = Mathf.CeilToInt(((float)NetworkManager.requestsCount / 10));
        }
        
        if(maxPage < 1)
        {
            maxPage = 1;
        }

        Debug.Log(maxPage);
        Debug.Log(NetworkManager.requestsCount);
        if (NetworkManager.currentFriendsPage < maxPage)
        {
            if(!isRequests)
            {
                updateFriendList = true;
                NetworkManager.instance.GetComponent<NetworkManager>().ShowFriends(NetworkManager.currentFriendsPage + 1);
            }
            else
            {
                updateFriendRequests = true;
                NetworkManager.instance.GetComponent<NetworkManager>().ShowRequests(NetworkManager.currentFriendsPage + 1);
            }
            
            NetworkManager.currentFriendsPage++;
        }

    }

    public void ShowRequests(string[] requests)
    {
        Debug.Log("update frreq " + updateFriendRequests);

        Transform RequestsHolder = this.transform.Find("Holder").transform.Find("FriendRequestHolder").transform.Find("RequestsHolder");
        if (instantiateRequests)
        {
            for (int i = 0; i < requests.Length; i++)
            {
                //Instantiate
                GameObject requestObj = Instantiate(requestObject, RequestsHolder) as GameObject;
                requestObj.transform.parent = RequestsHolder;

                requestObj.name = "Request " + i;

                float requestObjY = requestObj.transform.position.y - 0.4f * i;
                ///////////SELSKO NAMESTVANE/////////////
                requestObj.transform.position = new Vector3(requestObj.transform.position.x, requestObjY, requestObj.transform.position.z);
                /////////////////////////////////////////
                /////Player.instance.GetComponent<Player>().showOCPlayerCard("kon");
                RequestJSON request = RequestJSON.CreateFromJSON(requests[i]);

                requestObj.GetComponent<ShowRequest>().request_id = request._id;
                requestObj.GetComponent<ShowRequest>().requestOrder = i;
                requestObj.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = request.sender_username;
            }

            Debug.Log("Requests instantiated");
            instantiateRequests = false;
            updateFriendRequests = false;

            NetworkManager.instance.GetComponent<NetworkManager>().GetRequestsCount();
        }
        else
        {
            if (updateFriendRequests)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().GetRequestsCount();

                Debug.Log("UPDATE REQUESTS");
                int currentRequestsCount = requests.Length;
                int instantiatedRequestsCount = RequestsHolder.childCount;

                if (currentRequestsCount > instantiatedRequestsCount) //We have new requests
                {
                    int dif = currentRequestsCount - instantiatedRequestsCount;
                    int newRequestNumber = 0;
                    for (int i = 0; i < dif; i++)
                    {
                        GameObject requestObj = Instantiate(requestObject, RequestsHolder) as GameObject;
                        requestObj.transform.parent = RequestsHolder;

                        float requestObjY = requestObj.transform.position.y - 0.4f * (instantiatedRequestsCount + i);
                        ///////////SELSKO NAMESTVANE/////////////
                        requestObj.transform.position = new Vector3(requestObj.transform.position.x, requestObjY, requestObj.transform.position.z);
                        /////////////////////////////////////////

                        newRequestNumber = instantiatedRequestsCount + i;
                        requestObj.name = "Request " + newRequestNumber;
                    }
                }
                else if (currentRequestsCount < instantiatedRequestsCount) //Someone removed us from their friend list _)_
                {
                    int requestNumber = 0;
                    for (int i = instantiatedRequestsCount; i > currentRequestsCount; i--)
                    {
                        requestNumber = i - 1;
                        Destroy(RequestsHolder.transform.Find("Request " + requestNumber).gameObject);
                    }
                }

                int requestCounter = 0;
                foreach (Transform child in RequestsHolder)
                {
                    if (requestCounter < requests.Length)
                    {
                        RequestJSON request = RequestJSON.CreateFromJSON(requests[requestCounter]);
                        Debug.Log("name " + request.sender_username);
                        Debug.Log(requestCounter);
                        Debug.Log(RequestsHolder.childCount);

                        child.GetComponent<ShowRequest>().request_id = request._id;
                        child.GetComponent<ShowRequest>().requestOrder = requestCounter;
                        child.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = request.sender_username;

                        requestCounter++;
                    }
                }

                updateFriendRequests = false;
            }
        }
    }

    public void AcceptRequest()
    {
        Debug.Log("ADD FRIEND");
        updateFriendRequests = true;
        NetworkManager.instance.GetComponent<NetworkManager>().HandleRequest(requestId, true);
        
        GameObject.Find("RequestDialogHolder").transform.Find("RequestDialogBox").gameObject.SetActive(false);
        NetworkManager.instance.GetComponent<NetworkManager>().ShowRequests(NetworkManager.currentFriendsPage);
        //RearrangeRequests();
        //TODO: add friend
        //TODO: update friend list
    }

    public void DeclineRequest()
    {
        Debug.Log("Decline FRIEND Request");
        updateFriendRequests = true;
        NetworkManager.instance.GetComponent<NetworkManager>().HandleRequest(requestId, false);

        GameObject.Find("RequestDialogHolder").transform.Find("RequestDialogBox").gameObject.SetActive(false);

        
        Debug.Log(requestId);
        NetworkManager.instance.GetComponent<NetworkManager>().ShowRequests(NetworkManager.currentFriendsPage);
        //TODO: add friend
        //TODO: update friend list
    }

    private void RearrangeRequests()
    {
        Transform RequestsHolder = this.transform.Find("Holder").transform.Find("FriendRequestHolder").transform.Find("RequestsHolder");

        int instantiatedRequestsCount = RequestsHolder.childCount;

        int counter = 0;
        foreach (Transform child in RequestsHolder)
        {
            if(counter == requestOrder)
            {
                Destroy(child.gameObject);
            }
            else if (counter > requestOrder)
            {                                        
                float requestObjY = child.transform.position.y + 0.4f;
                ///////////SELSKO NAMESTVANE/////////////
                child.transform.position = new Vector3(child.transform.position.x, requestObjY, child.transform.position.z);
                /////////////////////////////////////////
                child.GetComponent<ShowRequest>().requestOrder = counter-1;
            }

            counter++;
        }
    }

    [Serializable]
    public class FriendJSON
    {
        public string username;
        public string id;
        public bool is_logged;
        public string server;
        
        public static FriendJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<FriendJSON>(data);
        }
    }

    [Serializable]
    public class RequestJSON
    {
        public string _id;
        public string sender_username;

        public static RequestJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<RequestJSON>(data);
        }
    }
}