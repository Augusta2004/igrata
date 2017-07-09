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
    public Canvas canvas;
    public SocketIOController socket;
    private string playerNameInput;  //Later should take data from login form  //DB
    public GameObject player;

    public static int fish;

    public GameObject playerCard;
    public GameObject playerCardOC; //other players' cards

    public GameObject item;

    public static bool isServerAvailable = true;
    public static string localUsername;
    private string localId;


    public static string sceneName;
    public static bool isLogging = true;

    public static bool otherPlayersLoaded = false;
    public static bool playerLoaded = false;

    private Timer myTimer;

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
        socket.On("play", OnPlay);
        socket.On("player move", OnPlayerMove);
        socket.On("player stop animation", OnPlayerStopAnimation);
        socket.On("player chat", OnPlayerChat);
        socket.On("collect item", OnCollectItem);
        socket.On("get player items", OnGetPlayerItems);
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
    
    /*
    public void InitTimer()
    {
        myTimer = new System.Timers.Timer();
        myTimer.Elapsed += new ElapsedEventHandler(CheckServer);
        myTimer.Interval = 500;     
        myTimer.Enabled = true;
    }

    private void CheckServer(object source, ElapsedEventArgs e)
    {
    }
    */





    
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

        //int rnd = UnityEngine.Random.Range(0, 100);

        string playerName = localUsername;
        playerNameInput = playerName;
        List<SpawnPoint> playerSpawnPoints = GetComponent<PlayerSpawner>().playerSpawnPoints;
        PlayerJSON playerJSON = new PlayerJSON(playerName, playerSpawnPoints);
        string data = JsonUtility.ToJson(playerJSON);

        socket.Emit("play", data);
        Debug.Log(data);
    }

    IEnumerator DownloadAsset(string BundleURL, string AssetName, int version)
    {
        // Wait for the Caching system to be ready
        while (!Caching.ready) yield return null;

        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
        {
            Debug.Log(www);
            yield return www;
            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);

            AssetBundle bundle = www.assetBundle;

            if (AssetName == "")
            {
                Instantiate(bundle.mainAsset);
            }
            else
            {
                Sprite sprite = bundle.LoadAsset<Sprite>(AssetName);
                Debug.Log(sprite);
                yield return sprite;
            }

            bundle.Unload(false);
        }
    }

    public void CommandMove(Vector2 vec2, Vector2 anim, int numAnimation)
    {
        string data = JsonUtility.ToJson(new AnimationPositionJSON(vec2, anim, numAnimation));

        //string animation = JsonUtility.ToJson(new PositionJSON(anim));
        socket.Emit("player move", data);
    }

    public void StopAnimation(int numAnimation)
    {
        socket.Emit("player stop animation", JsonUtility.ToJson(numAnimation));
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

    public void GetPlayerItems()
    {
        socket.Emit("get player items");
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
        
        string dataUser = JsonUtility.ToJson(new ChangeItemJSON(userJSON.id, userJSON.name));
       
        socket.Emit("get on other player items", dataUser);
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

        GameObject itemDialogCanvas = GameObject.Find("ItemDialog").transform.Find("Canvas").gameObject;
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
        Debug.Log(items.Length);
        for (int i = 0; i < items.Length; i++)
        {
            //Debug.Log(items[i]);
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(items[i].ToString());

            GameObject itemObject = Instantiate(item, new Vector2(0, 0), Quaternion.identity) as GameObject;
            itemObject.name = "Item " + itemsJson.item_id;
            itemObject.transform.parent = GameObject.Find("PlayerCard").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("ItemsHolder");

            itemObject.GetComponent<ClickItem>().item_id = itemsJson.item_id;
            itemObject.GetComponent<ClickItem>().item_on = itemsJson.is_on;
            itemObject.GetComponent<ClickItem>().type = itemsJson.type;
            itemObject.GetComponent<ClickItem>().path = itemsJson.picture;

            string path = "Sprites/Items/" + itemsJson.picture;
            itemObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path);

            itemObject.AddComponent<BoxCollider2D>();
            itemObject.GetComponent<BoxCollider2D>().isTrigger = true;

            GameObject itemsHolder = GameObject.Find("PlayerCard").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("ItemsHolder").gameObject;

            float offset0 = i % 3;
            float x = itemsHolder.transform.position.x - itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x / 2 + (1 + 1.25f * offset0) * itemObject.GetComponent<SpriteRenderer>().bounds.size.x;

            int offset = i / 3 + 1;
            float y = itemsHolder.transform.position.y + itemsHolder.GetComponent<SpriteRenderer>().bounds.size.y / 2 - itemObject.GetComponent<SpriteRenderer>().bounds.size.y * offset;
            itemObject.transform.position = new Vector2(x, y);
            itemObject.GetComponent<SpriteRenderer>().sortingOrder = 201;
        }
    }

    void OnGetOtherPlayerItems(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data;
        Debug.Log(data);
        string username = socketIOEvent.data.Split(':')[1].Split(',')[0];
        Debug.Log(username);
        username = username.Substring(1, username.Length - 2);

        string itemsStr = socketIOEvent.data.Split(new string[] { "\"items\":" }, StringSplitOptions.None)[1];
        string[] items = StrToJsonArr(itemsStr);


        GameObject otherPlayer = GameObject.Find(username) as GameObject;
       
        otherPlayer.GetComponent<CharacterController>().spritesArray = new Dictionary<string, Sprite[]>();

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
                        
            item.transform.Find(type1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemsJson.picture + "PC");
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

            item.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemsJson.picture + "PC");
            Destroy(item.GetComponent<PolygonCollider2D>());
            item.AddComponent<PolygonCollider2D>().isTrigger = true;
            item.GetComponent<ItemRemove>().id = itemsJson.item_id;
            item.GetComponent<ItemRemove>().type = itemsJson.type;

            if (itemsJson.type != "pin" && itemsJson.type != "background")
            {
                var subSprites = Resources.LoadAll<Sprite>("Animations/"  + itemsJson.item_id);
                localPlayer.GetComponent<CharacterController>().spritesArray[itemsJson.type] = subSprites;
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

        string currentAnimation = otherPlayer.GetComponent<SpriteRenderer>().sprite.name;
        int numAnimation = Convert.ToInt32(currentAnimation.Split('_')[1]);

        for (int i = 0; i < items.Length; i++)
        {
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(items[i].ToString());
           
            if (itemsJson.type != "pin" && itemsJson.type != "background")
            {
                var subSprites = Resources.LoadAll<Sprite>("Animations/" + itemsJson.item_id);
                Debug.Log(otherPlayer);
                otherPlayer.GetComponent<CharacterController>().spritesArray[itemsJson.type] = subSprites;
               
                otherPlayer.transform.Find("ItemHolder")
                    .transform.Find(itemsJson.type + "Sprite")
                    .GetComponent<SpriteRenderer>().sprite = subSprites[numAnimation];
            }
        }
    }
    
    void OnBuyItem(SocketIOEvent socketIOEvent)
    {
        Debug.Log(socketIOEvent);
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
        Debug.Log("TRUEEEEEEEEEEEEEE WEeeeeeeeeeeeeee");
        otherPlayersLoaded = true;        
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
        public List<PointJSON> playerSpawnPoints;

        public PlayerJSON(string _name, List<SpawnPoint> _playerSpawnPoints)
        {
            playerSpawnPoints = new List<PointJSON>();
            name = _name;
            foreach (SpawnPoint playerSpawnPoint in _playerSpawnPoints)
            {
                PointJSON pointJSON = new PointJSON(playerSpawnPoint);
                playerSpawnPoints.Add(pointJSON);
            }

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

        public PointJSON(SpawnPoint spawnPoint)
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

        public IntToJSON(int _intVal)
        {
            intVal = _intVal;
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
    #endregion
}