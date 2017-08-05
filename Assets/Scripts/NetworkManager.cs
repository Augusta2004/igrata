using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnityEngine.UI;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;
using System.Timers;
using UnitySocketIO.Events;

public class NetworkManager : MonoBehaviour
{

    public static NetworkManager instance;
    public SocketIOController socket;
    private string playerNameInput; 
    public GameObject player;

    public static int fish;

    public GameObject playerCard;
    public GameObject playerCardOC; //other players' cards

    public GameObject item;

    public static bool isServerAvailable = true;
    public static string localUsername;
    private string localId;
    public static string serverName;

    public static string sceneName;
    public static bool isLogging = true;

    public static bool otherPlayersLoaded = false;
    public static bool playerLoaded = false;

    private Timer myTimer;

    public static int itemsCount;
    public static int itemsPage = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        socket.On("user login", OnUserLogin);

        socket.On("other player connected", OnOtherPlayerConnected);
        socket.On("get friend status", OnGetFriendStatus);
        socket.On("show friends", OnShowFriends);
        socket.On("show requests", OnShowRequests);
        socket.On("update friend list", OnUpdateFriendList);
        socket.On("update requests", OnUpdateRequests);
        socket.On("handle friend", OnHandleFriend);
        socket.On("play", OnPlay);
        socket.On("player move", OnPlayerMove);
        socket.On("player stop animation", OnPlayerStopAnimation);
        socket.On("player chat", OnPlayerChat);
        socket.On("collect item", OnCollectItem);
        socket.On("get player items", OnGetPlayerItems);
        socket.On("get player items count", OnGetPlayerItemsCount);
        socket.On("get other player items", OnGetOtherPlayerItems);
        socket.On("get on items", OnGetOnItems);
        socket.On("get on other player items", OnGetOnOtherPlayerItems);
        socket.On("buy item", OnBuyItem);
        socket.On("player change room", OnPlayerChangeRoom);
        socket.On("players loaded", OnPlayersLoaded);
        socket.On("other player disconnected", OnOtherPlayerDisconnected);

        socket.Connect();        
    }
    
    public void LoginLink()
    {
        GameObject.Find("Canvas").transform.Find("Login").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("LoginLink").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Back").gameObject.SetActive(true);
    }
   
    #region Commands

    public void ConnectToServer()
    {
        StartCoroutine(OuterIEnumerator());
    }
        
    IEnumerator OuterIEnumerator()
    {
        yield return StartCoroutine(ConnectToServerCR());

        playerLoaded = true;
    }

    IEnumerator ConnectToServerCR()
    {
        Debug.Log("Connect to Server");
        yield return new WaitForSeconds(0.5f);

        socket.Emit("player connect");
        yield return new WaitForSeconds(1f);

        string playerName = localUsername;
        playerNameInput = playerName;
        
        yield return StartCoroutine(DownloadObject("SpawnPoints/", false, sceneName.ToLower(), false));

        GameObject spawnPoint = GameObject.Find(sceneName).gameObject;

        PlayerJSON playerJSON = new PlayerJSON(playerName, spawnPoint);
        string data = JsonUtility.ToJson(playerJSON);

        socket.Emit("play", data);
    }
   
    public void CommandMove(Vector2 vec2, Vector2 anim, int numAnimation)
    {
        string data = JsonUtility.ToJson(new AnimationPositionJSON(vec2, anim, numAnimation));

        //string animation = JsonUtility.ToJson(new PositionJSON(anim));
        socket.Emit("player move", data);
    }

    public void StopAnimation(int numAnimation)
    {
        socket.Emit("player stop animation", JsonUtility.ToJson(new IntToJSON(numAnimation)));
    }

    public void PlayerChat(String message, String emojiPath)
    {
        Debug.Log("empty string");
        string data = JsonUtility.ToJson(new ChatJSON(message, emojiPath));
        
        socket.Emit("player chat", data);
    }

    public void SendEmoji(String emojiSprite)
    {
        GameObject.Find("EmojiBox").SetActive(false);
        Debug.Log("hide box");
        PlayerChat("", emojiSprite);
    }

    public void CollectItem(string id, string playerName)
    {
        if (isServerAvailable && playerName == localUsername)
        {
            socket.Emit("collect item", JsonUtility.ToJson(new StringToJSON(id)));
            Debug.Log(JsonUtility.ToJson(new StringToJSON(id)));
            Debug.Log("You've tried to pick item with id: " + id + " USER - " + localUsername);

            isServerAvailable = false;
        }
    }

    public void GetPlayerItems(int page, string type)
    {
        if (type == null)
        {
            socket.Emit("get player items", JsonUtility.ToJson(new IntToJSON(page)));
        }
        else
        {
            socket.Emit("get player items by type", JsonUtility.ToJson(new StringIntToJSON(page, type)));
        }
    }

    public void GetPlayerItemsCount(string type)
    {
        if(type == null)
        {
            socket.Emit("get player items count");
        }
        else
        {
            socket.Emit("get player items count by type", JsonUtility.ToJson(new StringToJSON(type)));
        }
    }

    public void GetOtherPlayerItems(String username)
    {
        string data = JsonUtility.ToJson(new StringToJSON(username));
        socket.Emit("get other player items", data);
    }

    public void ChangeItem(string id, string type)
    {
        string data = JsonUtility.ToJson(new ChangeItemJSON(id, type));
        socket.Emit("change item", data);
    }

    public void BuyItem(string id)
    {
        if (isServerAvailable)
        {
            Debug.Log("BUYYYYYYYYYYYYYY");
            socket.Emit("buy item", JsonUtility.ToJson(new StringToJSON(id)));

            isServerAvailable = false;
        }
    }

    public void RemoveItem(string id, string type)
    {
        socket.Emit("remove item", JsonUtility.ToJson(new StringToJSON(id)));
    }
   
    public void UserLogin(string data)
    {
        Debug.Log(data);
        socket.Emit("user login", data);
    }
    
    public void ChangeRoom(string roomName)
    {
        Dictionary<String, String> roomDictionary = new Dictionary<string, string>();
        roomDictionary.Add("Room name", roomName);
        sceneName = roomName;
        Debug.Log(JsonUtility.ToJson(new StringToJSON(roomName)));
        socket.Emit("join room", JsonUtility.ToJson(new StringToJSON(roomName)));
  
        SceneManager.LoadScene("Loading");
    }

    public void AddFish(int addFish)
    {
        fish += addFish;
        socket.Emit("add fish", JsonUtility.ToJson(new IntToJSON(addFish)));
    }

    public void SendFriendRequest(string user_id)
    {
        Debug.Log("You sent a friend list to user with ID " + user_id);
        socket.Emit("friend request", JsonUtility.ToJson(new StringToJSON(user_id)));
    }

    public void RemoveFriend(string user_id)
    {
        Debug.Log("You are about to remove user with ID " + user_id + " from your friend list");
        socket.Emit("remove friend", JsonUtility.ToJson(new StringToJSON(user_id)));
    }

    public void ShowFriends()
    {
        Debug.Log("Show friends from SERVER");
        socket.Emit("show friends");
    }

    public void ShowRequests()
    {
        Debug.Log("Show requests from SERVER");
        socket.Emit("show requests");
    }

    public void CheckForRequestUpdate()
    {
        socket.Emit("check request update");
    }

    public void HandleRequest(string requestUserID, bool acceptRequest)
    {
        socket.Emit("handle request", JsonUtility.ToJson(new HandleRequestJSON(requestUserID, acceptRequest)));
    }

    #endregion

    #region Listening

    void OnUserLogin(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        ErrorJSON errorJSON = ErrorJSON.CreateFromJSON(data);

        if (errorJSON.errors.Length > 0)
        {
            string errStr = "";
            for (int i = 0; i < errorJSON.errors.Length; i++)
            {
                errStr += errorJSON.errors[i] + '\n';
            }

            GameObject.Find("Canvas").transform.Find("Login").transform.Find("LoginErrors").GetComponent<Text>().text = errStr;
        }
        else
        {
            sceneName = "Room1";

            ChangeRoom(sceneName);          
           
            //Emit to server, so we can join a room          
            
            //StartCoroutine(ConnectToServerCR());
        }
    }
    
    void OnOtherPlayerConnected(SocketIOEvent socketIOEvent)
    {
        print("Someone else connected");
        string data = socketIOEvent.data.ToString();
        Debug.Log(data);

        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Vector2 position = new Vector2(userJSON.position[0], userJSON.position[1]);
        GameObject o = GameObject.Find(userJSON.name) as GameObject;
        if (o != null)
        {
            return;
        }
        GameObject p = Instantiate(player, position, Quaternion.identity) as GameObject;
        CharacterController pc = p.GetComponent<CharacterController>();
        Transform t = p.transform.Find("Username");
        Transform t2 = t.transform.Find("Player Name");
        Text playerName = t2.GetComponent<Text>();
        playerName.text = userJSON.name;
        pc.isLocalPlayer = false;
        p.name = userJSON.name;

        Player playerScript = p.GetComponent<Player>();
        playerScript.user_id = userJSON.id;

        string dataUser = JsonUtility.ToJson(new ChangeItemJSON(userJSON.id, userJSON.name));
       
        socket.Emit("get on other player items", dataUser);
        socket.Emit("get friend status", dataUser);
    }

    void OnPlay(SocketIOEvent socketIOEvent)
    {
        print("you joined");
        
        string data = socketIOEvent.data.ToString();
        Debug.Log(data);
        UserJSON currentUserJSON = UserJSON.CreateFromJSON(data);
        localUsername = currentUserJSON.name;
        fish = currentUserJSON.fish;
        localId = currentUserJSON.id;
        serverName = currentUserJSON.server;
        Vector2 position = new Vector2(currentUserJSON.position[0], currentUserJSON.position[1]);
        GameObject p = Instantiate(player, position, Quaternion.identity) as GameObject;

        CharacterController pc = p.GetComponent<CharacterController>();
        Transform t = p.transform.Find("Username");
        Transform t2 = t.transform.Find("Player Name");
        Text playerName = t2.GetComponent<Text>();
        playerName.text = currentUserJSON.name;
        pc.isLocalPlayer = true;
        p.name = currentUserJSON.name;

        socket.Emit("get on items");

        string currentAnimation = p.GetComponent<SpriteRenderer>().sprite.name;
        int numAnimation = Convert.ToInt32(currentAnimation.Split('_')[1]);
        Debug.Log(p.name + " | " + pc.spritesArray.Count);
        foreach (KeyValuePair<string, Sprite[]> entry in pc.spritesArray)
        {
            p.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimation];
        }

        //playerLoaded = true;
        //GameObject playerCardInstance = Instantiate(playerCard, position, Quaternion.identity) as GameObject;
        // playerCardInstance.name = currentUserJSON.name + "PC";
    }

    void OnPlayerMove(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Vector2 position = new Vector2(userJSON.position[0], userJSON.position[1]);
        Vector2 animation = new Vector2(userJSON.animation[0], userJSON.animation[1]);
        int numAnimation = userJSON.numAnimation;

        if (userJSON.name == playerNameInput)
        {
            return;
        }
        GameObject p = GameObject.Find(userJSON.name) as GameObject;
        if (p != null)
        {
            p.transform.position = position;
            p.GetComponent<Animator>().SetBool("isWalking", true);
            p.GetComponent<Animator>().SetFloat("input_x", animation.x);
            p.GetComponent<Animator>().SetFloat("input_y", animation.y);

            var sprArr = p.GetComponent<CharacterController>().spritesArray;

            foreach (KeyValuePair<string, Sprite[]> entry in sprArr)
            {
                p.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimation];
            }
        }

        //p.GetComponent<Animator>();
    }

    void OnPlayerStopAnimation(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
       
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        
        GameObject p = GameObject.Find(userJSON.name) as GameObject;
       
        p.GetComponent<Animator>().SetBool("isWalking", false);
        p.GetComponent<Animator>().Play("Idle");
        
        var sprArr = p.GetComponent<CharacterController>().spritesArray;
        foreach (KeyValuePair<string, Sprite[]> entry in sprArr)
        {
            p.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[userJSON.numAnimation];
        }
    }

    void OnPlayerChat(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        ChatJSON chatJSON = ChatJSON.CreateFromJSON(data);
        
        GameObject p = GameObject.Find(chatJSON.player) as GameObject;

        if(chatJSON.message == "" && chatJSON.emoji == "")
        {
            p.transform.Find("ChatBaloon").gameObject.SetActive(false);
        }
        else if(chatJSON.message == "")
        {
            p.transform.Find("ChatBaloon").gameObject.SetActive(true);
            p.transform.Find("ChatBaloon").transform.Find("TopBaloon").transform.localScale = new Vector3(0.4f, 1.4f, 1);
            p.transform.Find("ChatBaloon").transform.Find("TopBaloon").transform.localPosition = new Vector3(0, 0.14f, 0);
        } 
        else
        {
            p.transform.Find("ChatBaloon").gameObject.SetActive(true);
            p.transform.Find("ChatBaloon").transform.localPosition = new Vector3(0.01f, 0.75f, 0);
            p.transform.Find("ChatBaloon").transform.Find("TopBaloon").transform.localPosition = new Vector3(0, 0, 0);
            p.transform.Find("Canvas").transform.Find("ChatText").transform.localPosition = new Vector3(-230.25f, -138.81f, 0);

            GameObject textObj = p.transform.Find("Canvas").transform.Find("ChatText").gameObject;
            Text textBox = textObj.GetComponent<Text>();


            float textObjWidth = textObj.GetComponent<RectTransform>().rect.width;

            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings = textBox.GetGenerationSettings(textBox.rectTransform.rect.size);
            float width = textGen.GetPreferredWidth(chatJSON.message, generationSettings);
            float height = textGen.GetPreferredHeight(chatJSON.message, generationSettings);

            float scaleX = (0.2f + width / 300);
            if(scaleX > 1.4f)
            {
                scaleX = 1.4f;
            }

            int rows = Convert.ToInt32(Mathf.Ceil(height / 55));

            float addScaleY = (0.73f * (rows - 1));
            float scaleY = 1 + addScaleY;
            float moveBaloonUp = 0.24f * (rows - 1);

            Debug.Log("Text Width: " + width);
            Debug.Log("Text Height: " + height);
            Debug.Log("Text Box Width: " + textObjWidth);
            Debug.Log("ScaleX: " + scaleX);
            Debug.Log("ScaleY: " + scaleY);
            Debug.Log("Rows: " + rows);

            p.transform.Find("ChatBaloon").transform.Find("TopBaloon").transform.localScale = new Vector3(scaleX, scaleY, 1);
            p.transform.Find("ChatBaloon").transform.Find("TopBaloon").transform.localPosition += new Vector3(0, moveBaloonUp, 0);
            p.transform.Find("Canvas").transform.Find("ChatText").transform.localPosition += new Vector3(0, moveBaloonUp, 0);
        }
    
        p.transform.Find("Canvas").transform.Find("ChatText").GetComponent<Text>().text = chatJSON.message;
        
        p.transform.Find("EmojiSprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(chatJSON.emoji);
    }

    void OnCollectItem(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        ItemJSON itemJSON = ItemJSON.CreateFromJSON(data);

        GameObject itemDialogCanvas = GameObject.Find("ItemDialog").transform.Find("ItemDialogHolder").transform.Find("Canvas").gameObject;
        itemDialogCanvas.transform.Find("Text").GetComponent<Text>().text = itemJSON.name;

        itemDialogCanvas.transform.Find("Yes").gameObject.SetActive(false);
        itemDialogCanvas.transform.Find("Cancel").gameObject.SetActive(false);
        itemDialogCanvas.transform.Find("OK").gameObject.SetActive(true);

        isServerAvailable = true;
    }

    void OnGetPlayerItems(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data;
        string[] items = StrToJsonArr(data);

        for (int i = 0; i < items.Length; i++)
        {
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(items[i].ToString());

            GameObject itemObject = Instantiate(item, new Vector2(0, 0), Quaternion.identity) as GameObject;
            itemObject.name = "Item " + itemsJson.item_id;
            itemObject.transform.parent = GameObject.Find("PlayerCard").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("ItemsHolder");

            itemObject.GetComponent<ClickItem>().item_id = itemsJson.item_id;
            itemObject.GetComponent<ClickItem>().item_on = itemsJson.is_on;
            itemObject.GetComponent<ClickItem>().type = itemsJson.type;
            itemObject.GetComponent<ClickItem>().path = itemsJson.picture;

            //string path = "Sprites/Items/" + itemsJson.picture;
            //itemObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path);

            LoadAssetBundles("Items/PlayerCard/", true, itemsJson.picture, true, localUsername, itemObject, i.ToString());            
            
            /*
            GameObject itemsHolder = GameObject.Find("PlayerCard").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("ItemsHolder").gameObject;

            float offset0 = i % 3;
            float x = itemsHolder.transform.position.x - itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x / 2 + (1 + 1.25f * offset0) * itemObject.GetComponent<SpriteRenderer>().bounds.size.x;

            int offset = i / 3 + 1;
            float y = itemsHolder.transform.position.y + itemsHolder.GetComponent<SpriteRenderer>().bounds.size.y / 2 - itemObject.GetComponent<SpriteRenderer>().bounds.size.y * offset;
            itemObject.transform.position = new Vector2(x, y);
            itemObject.GetComponent<SpriteRenderer>().sortingOrder = 201;
            */
        }
    }

    void OnGetPlayerItemsCount(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data;
        IntToJSON item = IntToJSON.CreateFromJSON(data);

        itemsCount = item.count;
    }

    void OnGetOtherPlayerItems(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data;
        Debug.Log(data);
        string username = socketIOEvent.data.Split(':')[1].Split(',')[0];
        username = username.Substring(1, username.Length - 2);

        string itemsStr = socketIOEvent.data.Split(new string[] { "\"items\":" }, StringSplitOptions.None)[1];
        string[] items = StrToJsonArr(itemsStr);


        GameObject otherPlayer = GameObject.Find(username) as GameObject;

        if (otherPlayer != null)
        {
            otherPlayer.GetComponent<CharacterController>().spritesArray = new Dictionary<string, Sprite[]>();
        } 

        GameObject item = GameObject.Find("PlayerCardOC")
                    .transform.Find("DragPlayerCard")
                    .transform.Find("PlayerCardHolder")
                    .transform.Find("Player Clothes").gameObject;

        foreach (Transform child in item.transform)
        {
            if (child.name != "ProfilePicture")
            {
                item.transform.Find(child.name).GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        
        for (int i = 0; i < items.Length; i++)
        {
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(items[i].ToString());

            string type1 = new CultureInfo("en-US").TextInfo.ToTitleCase(itemsJson.type);

            LoadAssetBundles("Items/PlayerCard/", true, itemsJson.picture, true, username, item.transform.Find(type1).gameObject, "item on");
            //item.transform.Find(type1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemsJson.picture + "PC");
        }
    }

    void OnGetOnItems(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data;
        string[] items = StrToJsonArr(data);

        GameObject localPlayer = GameObject.Find(localUsername) as GameObject;
        
        localPlayer.GetComponent<CharacterController>().spritesArray = new Dictionary<string, Sprite[]>();

        GameObject itemHolder = localPlayer
        .transform.Find("ItemHolder").gameObject;
 
        foreach (Transform child in itemHolder.transform)
        {
            child.GetComponent<SpriteRenderer>().sprite = null;
        }
     
        
        for (int i = 0; i < items.Length; i++)
        {            
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(items[i].ToString());
         
            string type1 = new CultureInfo("en-US").TextInfo.ToTitleCase(itemsJson.type);

            GameObject item = GameObject.Find("PlayerCard")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("Player Clothes")
                .transform.Find(type1).gameObject;

            //string url = "http://localhost:3000/images/Items/" + itemsJson.type + "s/" + itemsJson.type + "." + itemsJson.item_id;
            //Debug.Log(url);
           // StartCoroutine(DownloadAsset(url, itemsJson.picture + "PC", itemsJson.item_id));

            //item.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemsJson.picture + "PC");
            LoadAssetBundles("Items/PlayerCard/", true, itemsJson.picture, true, localUsername, item, "item on");
            
            item.GetComponent<ItemRemove>().id = itemsJson.item_id;
            item.GetComponent<ItemRemove>().type = itemsJson.type;

            if (itemsJson.type != "pin" && itemsJson.type != "background")
            {
                //var subSprites = Resources.LoadAll<Sprite>("Animations/"  + itemsJson.item_id);
               // localPlayer.GetComponent<CharacterController>().spritesArray[itemsJson.type] = subSprites;

                LoadAssetBundles("Items/Animations/", true, itemsJson.item_id, false, localUsername, localPlayer, itemsJson.type);
            }
        }
        
        string currentAnimation = localPlayer.GetComponent<SpriteRenderer>().sprite.name;
        int numAnimation = Convert.ToInt32(currentAnimation.Split('_')[1]);
        
        foreach (KeyValuePair<string, Sprite[]> entry in localPlayer.GetComponent<CharacterController>().spritesArray)
        {
            itemHolder.transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimation];
            
        }
    }
    
    void OnGetOnOtherPlayerItems(SocketIOEvent socketIOEvent)
    {
        //string data = socketIOEvent.data.ToString(socketIOEvent.data["items"]);
        Debug.Log(socketIOEvent.data);
        string username = socketIOEvent.data.Split(':')[1].Split(',')[0];
        username = username.Substring(1, username.Length - 2);

        string itemsStr = socketIOEvent.data.Split(new string[] { "\"items\":" }, StringSplitOptions.None)[1];
        string[] items = StrToJsonArr(itemsStr);
        
        GameObject otherPlayer = GameObject.Find(username) as GameObject;
        Debug.Log(username + "GG WP");

        otherPlayer.GetComponent<CharacterController>().spritesArray = new Dictionary<string, Sprite[]>();

        GameObject itemHolder = otherPlayer
                .transform.Find("ItemHolder").gameObject;

        foreach (Transform child in itemHolder.transform)
        {
            child.GetComponent<SpriteRenderer>().sprite = null;
        }

        for (int i = 0; i < items.Length; i++)
        {
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(items[i].ToString());
           
            if (itemsJson.type != "pin" && itemsJson.type != "background")
            {
                //var subSprites = Resources.LoadAll<Sprite>("Animations/" + itemsJson.item_id);
               // otherPlayer.GetComponent<CharacterController>().spritesArray[itemsJson.type] = subSprites;

                LoadAssetBundles("Items/Animations/", true, itemsJson.item_id, false, username, otherPlayer, itemsJson.type); 
            }
        }
    }
    
    void OnBuyItem(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        Debug.Log(data);
        ErrorJSON errorJSON = ErrorJSON.CreateFromJSON(data);
        GameObject itemDialogCanvas = GameObject.Find("ItemDialog").transform.Find("ItemDialogHolder").transform.Find("Canvas").gameObject;
        itemDialogCanvas.transform.Find("Text").GetComponent<Text>().text = errorJSON.errors[0];

        itemDialogCanvas.transform.Find("YesBuy").gameObject.SetActive(false);
        itemDialogCanvas.transform.Find("Cancel").gameObject.SetActive(false);
        itemDialogCanvas.transform.Find("OK").gameObject.SetActive(true);

        isServerAvailable = true;
    }

    void OnPlayerChangeRoom(SocketIOEvent socketIOEvent)
    {        
        string data = socketIOEvent.data.ToString();
        UsernameJSON userJSON = UsernameJSON.CreateFromJSON(data);
        Debug.Log(data);
        Destroy(GameObject.Find(userJSON.playerUsername).gameObject);
    }

    void OnPlayersLoaded(SocketIOEvent socketIOEvent)
    {
        otherPlayersLoaded = true;        
    }

    void OnHandleFriend(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        HandleFriendJSON friendStatus = HandleFriendJSON.CreateFromJSON(data);

        if (friendStatus.playerUsername != "" && GameObject.Find(friendStatus.playerUsername) != null)
        {
            //TODO: show player card again
            GameObject playerCardOC = GameObject.Find("PlayerCardOC").transform.Find("DragPlayerCard").gameObject;
            Debug.Log(GameObject.Find(friendStatus.playerUsername));

            if (playerCardOC.activeSelf)
            {                
                if (playerCardOC.transform.Find("PlayerCardHolder").transform.Find("PlayerCardCanvas").transform.Find("PlayerName").GetComponent<Text>().text == friendStatus.playerUsername)
                {
                    playerCardOC.SetActive(false);
                    Player.instance.GetComponent<Player>().ShowOCPlayerCard(friendStatus.playerUsername, friendStatus.addFriend, friendStatus.playerId);

                    Debug.Log(friendStatus.playerId);
                }
            }

            if (friendStatus.addFriend)
            {
                GameObject.Find(friendStatus.playerUsername).GetComponent<Player>().isFriend = true;
            }
            else
            {
                GameObject.Find(friendStatus.playerUsername).GetComponent<Player>().isFriend = false;
            }

        }

        Debug.Log("ADD NEW FRIEND");
    }

    void OnGetFriendStatus(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        FriendStatusJSON friendStatus = FriendStatusJSON.CreateFromJSON(data);
       
        GameObject player = GameObject.Find(friendStatus.username);
        Player playerScript = player.GetComponent<Player>();
        playerScript.isFriend = friendStatus.isFriend;
    }

    void OnShowFriends(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        
        string[] friends = StrToJsonArr(data);
        
        FriendsController.instance.GetComponent<FriendsController>().ShowFriends(friends);
    }

    void OnShowRequests(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        string[] requests = StrToJsonArr(data);
        Debug.Log("OnShowRequests");
        FriendsController.instance.GetComponent<FriendsController>().ShowRequests(requests);
    }

    void OnUpdateFriendList(SocketIOEvent socketIOEvent) //A friend has logged in/out
    {
        Debug.Log("nekoi priatel se vurza kum igrata");
    
        bool isFriendlistActive = GameObject.Find("Friendlist").transform.Find("Holder").transform.Find("FriendlistHolder").gameObject.activeSelf;
        if (isFriendlistActive)
        {
            FriendsController.updateFriendList = true;
            ShowFriends();
        }
        else
        {
            FriendsController.updateFriendList = true;
        }
    }

    void OnUpdateRequests(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        string[] requests = StrToJsonArr(data);

        bool isRequestsActive = GameObject.Find("Friendlist").transform.Find("Holder").transform.Find("FriendRequestHolder").gameObject.activeSelf;
        if (isRequestsActive)
        {
            FriendsController.updateFriendRequests = true;
            FriendsController.instance.GetComponent<FriendsController>().ShowRequests(requests);
        }
        else
        {
            FriendsController.updateFriendRequests = true;
            //Show them, that you've got 1 new request
        }
    }

    void OnOtherPlayerDisconnected(SocketIOEvent socketIOEvent)
    {
        print("user disconnected");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Destroy(GameObject.Find(userJSON.name));
    }
    #endregion

    #region Miscelanieous

    public void LoadAssetBundles(string assetFolder, bool isSprite, string assetBundleName, bool isPC, string username = null, GameObject obj = null, string itemType = null)
    {
        StartCoroutine(DownloadObject(assetFolder, isSprite, assetBundleName, isPC, username, obj, itemType));
    }

    IEnumerator DownloadObject(string assetFolder, bool isSprite, string assetBundleName, bool isPC, string username = null, GameObject obj = null, string itemType = null)
    {
        //string path = "file:///D:/UnityProjects/The Game/server/server/public/Assets/" + assetBundleName;
        string path = "http://localhost:3000/Assets/" + assetFolder + assetBundleName;
        WWW www = WWW.LoadFromCacheOrDownload(path, 1);
     
        yield return www;

        AssetBundle bundle = www.assetBundle;

        if(isSprite)
        {
            Sprite[] spriteSheet = bundle.LoadAssetWithSubAssets<Sprite>(assetBundleName);            

            if(isPC)
            {
                if(itemType == "item on")
                {
                    obj.GetComponent<SpriteRenderer>().sprite = spriteSheet[0];

                    Destroy(obj.GetComponent<PolygonCollider2D>());
                    obj.AddComponent<PolygonCollider2D>().isTrigger = true;
                }
                else
                {
                    obj.GetComponent<SpriteRenderer>().sprite = spriteSheet[1];

                    obj.AddComponent<BoxCollider2D>();
                    obj.GetComponent<BoxCollider2D>().isTrigger = true;


                    GameObject itemsHolder = GameObject.Find("PlayerCard").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("ItemsHolder").gameObject;

                    int i = Convert.ToInt16(itemType);

                    float offset0 = i % 3;
                    float x = itemsHolder.transform.position.x - itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x / 2 + (1 + 1.25f * offset0) * obj.GetComponent<SpriteRenderer>().bounds.size.x;

                    int offset = i / 3 + 1;
                    float y = itemsHolder.transform.position.y + itemsHolder.GetComponent<SpriteRenderer>().bounds.size.y / 2 - obj.GetComponent<SpriteRenderer>().bounds.size.y * offset;
                    obj.transform.position = new Vector2(x, y);
                    obj.GetComponent<SpriteRenderer>().sortingOrder = 201;
                }
                
            }
            else
            {
                obj.GetComponent<CharacterController>().spritesArray[itemType] = spriteSheet;

                if(username != localUsername)
                {
                    string currentAnimation = obj.GetComponent<SpriteRenderer>().sprite.name;
                    int numAnimation = Convert.ToInt32(currentAnimation.Split('_')[1]);

                    obj.transform.Find("ItemHolder")
                    .transform.Find(itemType + "Sprite")
                    .GetComponent<SpriteRenderer>().sprite = spriteSheet[numAnimation];
                }
            }

            bundle.Unload(false);
            yield return spriteSheet;
        }
        else
        {
            Debug.Log("load 1");
            AssetBundleRequest request = bundle.LoadAssetAsync<GameObject>(assetBundleName);
            Debug.Log("load 2");
            //yield return request;

            GameObject cat = request.asset as GameObject;
      
            GameObject objInstatiate = Instantiate<GameObject>(cat);
            objInstatiate.name = cat.name;
            bundle.Unload(false);
            yield return request;
        }
    }

    private string[] StrToJsonArr(string data)
    {
        string[] result2;
      
        int pFrom = data.IndexOf("[") + 1;
        int pTo = data.LastIndexOf("]");
   
        string result = data.Substring(pFrom, pTo - pFrom);

        if (result.Length > 0)
        {
            result2 = result.Split(new string[] { "}," }, StringSplitOptions.None);
            if (result2.Length > 1)
            {
                for (int i = 0; i < result2.Length - 1; i++)
                {
                    result2[i] += '}';
                }                
            }

            return result2;
        }
        else
        {
            return new string[] {};
        }        
    }

    #endregion

    #region JSONMessageClasses
    [Serializable]
    public class PlayerJSON
    {
        public string name;
        public PointJSON playerSpawnPoint;

        public PlayerJSON(string _name, GameObject _playerSpawnPoint)
        {
            PointJSON pointJSON = new PointJSON(_playerSpawnPoint);
            playerSpawnPoint = pointJSON;   
            name = _name;
        }
    }

    [Serializable]
    public class Player2JSON
    {
        public string name;
        public string id;

        public Player2JSON(string _id, string _name)
        {
            name = _name;
            id = _id;
        }
    }


    [Serializable]
    public class ChatJSON
    {
        public string player;
        public string message;
        public string emoji;

        public ChatJSON(string _message, string _emoji)
        {
            message = _message;
            emoji = _emoji;
        }

        public static ChatJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<ChatJSON>(data);
        }
    }

    [Serializable]
    public class PointJSON
    {
        public float[] position;

        public PointJSON(GameObject spawnPoint)
        {

            position = new float[] {
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y
            };
        }
    }

    [Serializable]
    public class PositionJSON
    {
        public float[] position;

        public PositionJSON(Vector2 _postion)
        {
            position = new float[] { _postion.x, _postion.y };
        }
    }

    //TEST FOR ANIMATION
    [Serializable]
    public class AnimationPositionJSON
    {
        public float[] position;
        public float[] animation;
        public int numAnimation;

        public AnimationPositionJSON(Vector2 _postion, Vector2 _animation, int _numAnimation)
        {
            position = new float[] { _postion.x, _postion.y };
            animation = new float[] { _animation.x, _animation.y };
            numAnimation = _numAnimation;
        }
    }
    //TEST FOR ANIMATION

    [Serializable]
    public class ChangeItemJSON
    {
        public string id;
        public string type;

        public ChangeItemJSON(string _id, string _type)
        {
            id = _id;
            type = _type;
        }
    }

    [Serializable]
    public class ItemJSON
    {
        public string name;
        public string id;

        public static ItemJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<ItemJSON>(data);
        }
    }


    [Serializable]
    public class PlayerItemsJSON
    {
        public string _id;
        public int user_id;
        public string item_id;
        public string name;
        public string picture;
        public string type;
        public bool is_on; 

        public static PlayerItemsJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<PlayerItemsJSON>(data);
        }
    }

    [Serializable]
    public class UserJSON
    {
        public string id;
        public string name;
        public float[] position;
        public float[] animation;
        public int numAnimation;
        public int fish;
        public string server;

        public static UserJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UserJSON>(data);
        }
    }

    [Serializable]
    public class UsernameJSON
    {
        public string playerUsername;

        public static UsernameJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UsernameJSON>(data);
        }
    }

    [Serializable]
    public class HandleFriendJSON
    {
        public string playerId;
        public string playerUsername;
        public bool addFriend;

        public static HandleFriendJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<HandleFriendJSON>(data);
        }
    }

    [Serializable]
    public class StringToJSON
    {
        public string stringVal;

        public StringToJSON(string _stringVal)
        {
            stringVal = _stringVal;
        }
    }

    [Serializable]
    public class IntToJSON
    {
        public int intVal;
        public int count;

        public IntToJSON(int _intVal)
        {
            intVal = _intVal;
        }

        public static IntToJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<IntToJSON>(data);
        }
    }

    [Serializable]
    public class StringIntToJSON
    {
        public int intVal;
        public string stringVal;

        public StringIntToJSON(int _intVal, string _stringVal)
        {
            intVal = _intVal;
            stringVal = _stringVal;
        }
    }

    [Serializable]
    public class FriendStatusJSON
    {
        public string username;
        public bool isFriend;

        public static FriendStatusJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<FriendStatusJSON>(data);
        }
    }

    [Serializable]
    public class ErrorJSON
    {
        public string[] errors;

        public static ErrorJSON CreateFromJSON(string err)
        {
            return JsonUtility.FromJson<ErrorJSON>(err);
        }
    }

    [Serializable]
    public class HandleRequestJSON
    {
        public string requestId;
        public bool acceptRequest;

        public HandleRequestJSON(string _requestId, bool _acceptRequest)
        {
            requestId = _requestId;
            acceptRequest = _acceptRequest;
        }
    }
    #endregion
}