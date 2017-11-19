using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // [HideInInspector]
    //public GameObject playerCard;
    public static Player instance;

    private string itemType = null;
    public string user_id;
    public bool isFriend = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void OnMouseDown()
    {
        bool isLocal = this.gameObject.GetComponent<CharacterController>().isLocalPlayer;
        if (isLocal)
        {
            //Close character info
            ShowItems();
        }
        else
        {
            ShowOCPlayerCard(this.name, isFriend);
        }
    }

    public void ShowOCPlayerCard(string username, bool isFriend = false, string userId = null)
    {
        //CLOSE CHARACTER INFO

        GameObject playerCard = GameObject.Find("PlayerCard") as GameObject;
        GameObject playerCardOC = GameObject.Find("PlayerCardOC") as GameObject;

        bool addButton;
        bool removeButton;

        GameObject playerCardOCCanvas = playerCardOC
            .transform.Find("DragPlayerCard")
            .transform.Find("PlayerCardHolder")
            .transform.Find("PlayerCardCanvas").gameObject;

        playerCardOC
            .transform.Find("DragPlayerCard")
            .transform.Find("PlayerCardHolder")
            .transform.Find("FriendDialog").gameObject.SetActive(false);

        string usernameToShow = username.Substring(0, username.Length - 7);

        playerCardOCCanvas.transform.Find("PlayerName").GetComponent<Text>().text = usernameToShow;

        if (isFriend)
        {
            addButton = false;
            removeButton = true;

            playerCardOCCanvas.transform.Find("ShowInfo").gameObject.SetActive(true);
        }
        else
        {
            addButton = true;
            removeButton = false;

            playerCardOCCanvas.transform.Find("ShowInfo").gameObject.SetActive(false);
        }

        playerCardOCCanvas.transform.Find("AddFriend").gameObject.SetActive(addButton);
        playerCardOCCanvas.transform.Find("RemoveFriend").gameObject.SetActive(removeButton);
        GameObject playerController = GameObject.Find("Player") as GameObject;
        playerController.GetComponent<Player>().isFriend = isFriend;
        if (userId != null)
        {
            user_id = userId;
        }
        Debug.Log(user_id);

        playerController.GetComponent<Player>().user_id = user_id;

        NetworkManager.instance.GetComponent<NetworkManager>().GetOtherPlayerItems(username, user_id);

        playerCard.transform.GetChild(0).gameObject.SetActive(false);
        playerCardOC.transform.GetChild(0).gameObject.SetActive(true);

        //TODO open other player card
    }

    public void GetItemsByType()
    {
        NetworkManager.itemsPage = 1;

        GameObject dropdown = GameObject.Find("PlayerCard")
            .transform.Find("DragPlayerCard")
            .transform.Find("PlayerCardHolder")
            .transform.Find("SortCanvas")
            .transform.Find("SortHolder")
            .transform.Find("Type")
            .gameObject;

        int menuIndex = dropdown.GetComponent<Dropdown>().value;

        switch(menuIndex)
        {
            case 0:
                itemType = null;
                break;

            case 1:
                itemType = "badge";
                break;

            case 2:
                itemType = "background";
                break;

            case 3:
                itemType = "head";
                break;
            case 4:
                itemType = "face";
                break;
            case 5:
                itemType = "neck";
                break;

            case 6:
                itemType = "body";
                break;

            case 7:
                itemType = "hands";
                break;

            case 8:
                itemType = "boots";
                break;

            default: break;
        }

        ShowItems(itemType);
    }

    public void ShowItems(string type = null)
    {
        GameObject playerCard = GameObject.Find("PlayerCard") as GameObject;
        GameObject playerCardOC = GameObject.Find("PlayerCardOC") as GameObject;

        playerCard.transform.GetChild(0).gameObject.SetActive(true);
        playerCardOC.transform.GetChild(0).gameObject.SetActive(false);

        NetworkManager.instance.GetComponent<NetworkManager>().GetPlayerItemsCount(type);
        NetworkManager.instance.GetComponent<NetworkManager>().GetPlayerItems(NetworkManager.itemsPage, type);

        GameObject itemsHolder = GameObject.Find("PlayerCard")
            .transform.Find("DragPlayerCard")
            .transform.Find("PlayerCardHolder")
            .transform.Find("ItemsHolder").gameObject;

        if (!itemsHolder.activeSelf)
        {
            Debug.Log("SHOW ITEMS");
            itemsHolder.SetActive(true);
            GameObject canvas = GameObject.Find("PlayerCard").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("SortCanvas").gameObject;
            canvas.transform.Find("SortHolder").gameObject.SetActive(true);

            GameObject showItems = GameObject.Find("PlayerCard")
            .transform.Find("DragPlayerCard")
            .transform.Find("PlayerCardHolder")
            .transform.Find("ShowItems").gameObject;

            var x = itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x;
            showItems.transform.position = new Vector2(showItems.transform.position.x + x, showItems.transform.position.y);

            canvas.transform.Find("SortHolder").transform.Find("Type").gameObject.GetComponent<Dropdown>().value = 0;
        }

        foreach (Transform child in itemsHolder.transform)
        {
            if (child.gameObject.name.Contains("Item"))
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        GameObject.Find("PlayerCard")
            .transform.Find("DragPlayerCard")
            .transform.Find("PlayerCardHolder")
            .transform.Find("PlayerCardCanvas")
            .transform.Find("Fish").GetComponent<Text>().text = NetworkManager.fish.ToString();
        
    }

    public void PrevPage()
    {
        NetworkManager.itemsPage--;

        if (NetworkManager.itemsPage == 0)
        {
            NetworkManager.itemsPage = 1;
        }
        else
        {
            ShowItems(itemType);
        }
    }

    public void NextPage()
    {       
        float count = NetworkManager.itemsCount;

        int maxPage = Mathf.CeilToInt(count / 12);
        if(maxPage <= 0)
        {
            maxPage = 1;
        }

        NetworkManager.itemsPage++;

        if(NetworkManager.itemsPage > maxPage)
        {
            NetworkManager.itemsPage = maxPage;
        }
        else
        {
            ShowItems(itemType);
        }
    }

    public void ShowInfo(bool isLocal)
    {
        if(isLocal)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().showCharacterInfo(NetworkManager.localId);            
        }
        else
        {
            NetworkManager.instance.GetComponent<NetworkManager>().showCharacterInfo(user_id);
        }
    }

    public void ShowFriendDialog()
    {
        GameObject friendDialog = GameObject.Find("PlayerCardOC")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("FriendDialog").gameObject;

        //bool isFriend = this.isFriend;
        string dialogText = "";
        if (isFriend)
        {
            dialogText = "Are you sure you want to remove " + user_id + "?";
        }
        else
        {
            dialogText = "Send friend request to " + user_id + "?";
        }

        friendDialog.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = dialogText;
        friendDialog.gameObject.SetActive(true);
    }

    public void CloseFriendDialog()
    {
        GameObject.Find("PlayerCardOC")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("FriendDialog").gameObject.SetActive(false);
    }

    public void HandleFriendRequest()
    {
        GameObject playerCardOC = GameObject.Find("PlayerCardOC").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").gameObject;
        playerCardOC.transform.Find("FriendDialog").gameObject.SetActive(false);

        if (isFriend)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().RemoveFriend(user_id);

            GameObject.Find("PlayerCardOC").transform.Find("DragPlayerCard").gameObject.SetActive(false);
        }
        else
        {
            NetworkManager.instance.GetComponent<NetworkManager>().SendFriendRequest(user_id);
            
        }


        //TODO: HIDE REQUEST DIALOG
    }
}