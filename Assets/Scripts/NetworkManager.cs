using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{

    public static NetworkManager instance;
    public Canvas canvas;
    public SocketIOComponent socket;
    private string playerNameInput;  //Later should take data from login form  //DB
    public GameObject player;

    public GameObject playerCard;
    public GameObject playerCardOC; //other players' cards

    public GameObject item;

    private bool isServerAvailable = true;
    private string localUsername;
    private int localId;

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
            //DontDestroyOnLoad(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        //subscription
        //StartCoroutine(ConnectToServer());

        socket.On("user register", OnUserRegister);
        socket.On("user login", OnUserLogin);

        socket.On("other player connected", OnOtherPlayerConnected);
        socket.On("play", OnPlay);
        socket.On("player move", OnPlayerMove);
        socket.On("player stop animation", OnPlayerStopAnimation);
        socket.On("player chat", OnPlayerChat);
        socket.On("collect item", OnCollectItem);
        socket.On("get player items", OnGetPlayerItems);
        socket.On("get on items", OnGetOnItems);
        socket.On("get on other player items", OnGetOnOtherPlayerItems);
        socket.On("buy item", OnBuyItem);
        socket.On("other player disconnected", OnOtherPlayerDisconnected);
    }

    public void LoginLink(bool onRegister = false)
    {
        if (onRegister)
        {
            GameObject.Find("Canvas").transform.Find("Login").transform.Find("RegisterSuccess").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("Register").gameObject.SetActive(false);
        }

        GameObject.Find("Canvas").transform.Find("Login").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("LoginLink").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("RegLink").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Back").gameObject.SetActive(true);
    }
    #region Commands

    IEnumerator ConnectToServer()
    {        
        yield return new WaitForSeconds(0.5f);

        socket.Emit("player connect");
        yield return new WaitForSeconds(1f);

        int rnd = UnityEngine.Random.Range(0, 100);
        
        string playerName = localUsername;
        playerNameInput = playerName;
        List<SpawnPoint> playerSpawnPoints = GetComponent<PlayerSpawner>().playerSpawnPoints;
        PlayerJSON playerJSON = new PlayerJSON(playerName, playerSpawnPoints);
        string data = JsonUtility.ToJson(playerJSON);

        socket.Emit("play", new JSONObject(data));
        Debug.Log(data);
    }

    public void CommandMove(Vector2 vec2, Vector2 anim, int numAnimation)
    {
        string data = JsonUtility.ToJson(new AnimationPositionJSON(vec2, anim, numAnimation));

        //string animation = JsonUtility.ToJson(new PositionJSON(anim));
        socket.Emit("player move", new JSONObject(data));
    }

    public void StopAnimation(int numAnimation)
    {
        socket.Emit("player stop animation", new JSONObject(numAnimation));
    }

    public void PlayerChat(String message, String emojiPath)
    {
        string data = JsonUtility.ToJson(new ChatJSON(message, emojiPath));
        
        socket.Emit("player chat", new JSONObject(data));
    }

    public void SendEmoji(String emojiSprite)
    {
        StartCoroutine(SendEmojiToServer(emojiSprite));
    }

    IEnumerator SendEmojiToServer(String emojiPath)
    {
        GameObject.Find("EmojiBox").SetActive(false);
        PlayerChat("", emojiPath);

        yield return new WaitForSeconds(3.5f);
        //TODO НЕ ИЗТРИВАЙ СЪОБЩЕНИЕТО, А САМО ЕМОЖИТО, КАЛТАК

        PlayerChat("", "");
    }

    public void CollectItem(int id)
    {
        if (isServerAvailable)
        {
            socket.Emit("collect item", new JSONObject(id));
            Debug.Log("You've tried to pick item with id: " + id);

            isServerAvailable = false;
        }
    }

    public void GetPlayerItems()
    {
        socket.Emit("get player items");
    }

    public void ChangeItem(int id, string type)
    {
        string data = JsonUtility.ToJson(new ChangeItemJSON(id, type));
        socket.Emit("change item", new JSONObject(data));
    }

    public void BuyItem(int id)
    {
        socket.Emit("buy item", new JSONObject(id));
    }

    public void RemoveItem(int id, string type)
    {
        socket.Emit("remove item", new JSONObject(id));

        Debug.Log(GameObject.Find(localUsername)
            .transform.Find("ItemHolder")
            .transform.Find(type + "Sprite").GetComponent<SpriteRenderer>().sprite);

        /*GameObject.Find(localUsername)
            .transform.Find("ItemHolder")
            .transform.Find(type + "Sprite")
            .GetComponent<SpriteRenderer>().sprite = null;


        Debug.Log(GameObject.Find(localUsername)
            .transform.Find("ItemHolder")
            .transform.Find(type + "Sprite").GetComponent<SpriteRenderer>().sprite);*/
    }

    public void UserRegister(JSONObject data)
    {
        socket.Emit("user register", data);
    }

    public void UserLogin(JSONObject data)
    {
        socket.Emit("user login", data);
    }
    
    #endregion

    #region Listening

    void OnUserRegister(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        ErrorJSON errorJSON = ErrorJSON.CreateFromJSON(data);

        Debug.Log(errorJSON.errors.Length);

        if (errorJSON.errors.Length > 0)
        {
            string errStr = "";
            for (int i = 0; i < errorJSON.errors.Length; i++)
            {
                errStr += errorJSON.errors[i] + '\n';
            }

            this.transform.Find("Register").transform.Find("RegErrors").GetComponent<Text>().text = errStr;
        }
        else
        {
            LoginLink(true);
        }

        isServerAvailable = true;
    }

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

            this.transform.Find("Login").transform.Find("LoginErrors").GetComponent<Text>().text = errStr;
        }
        else
        {
            SceneManager.LoadScene("Room1");
            StartCoroutine(ConnectToServer());
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
       
        socket.Emit("get on other player items", new JSONObject(dataUser));
    }

    void OnPlay(SocketIOEvent socketIOEvent)
    {
        print("you joined");
        
        string data = socketIOEvent.data.ToString();
        Debug.Log(data);
        UserJSON currentUserJSON = UserJSON.CreateFromJSON(data);
        localUsername = currentUserJSON.name;
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
       
        p.transform.Find("Canvas").transform.Find("ChatText").GetComponent<Text>().text = chatJSON.message;
        
        p.transform.Find("EmojiSprite").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(chatJSON.emoji);
    }

    void OnCollectItem(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        ItemJSON itemJSON = ItemJSON.CreateFromJSON(data);
        
        isServerAvailable = true;
    }

    void OnGetPlayerItems(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        for (int i = 0; i < socketIOEvent.data["items"].Count; i++)
        {
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(socketIOEvent.data["items"][i].ToString());

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

            float offset0 = i % 2;
            float x = itemsHolder.transform.position.x - itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x / 2 + (1 + 1.25f * offset0) * itemObject.GetComponent<SpriteRenderer>().bounds.size.x;

            float offset = Mathf.Ceil((i + 1) / 3) + 1;
            float y = itemsHolder.transform.position.y + itemsHolder.GetComponent<SpriteRenderer>().bounds.size.y / 2 - itemObject.GetComponent<SpriteRenderer>().bounds.size.y * offset;
            itemObject.transform.position = new Vector2(x, y);
            itemObject.GetComponent<SpriteRenderer>().sortingOrder = 201;
        }
    }
    
    void OnGetOnItems(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();
        GameObject localPlayer = GameObject.Find(localUsername) as GameObject;
        //Debug.Log(socketIOEvent.data);

        localPlayer.GetComponent<CharacterController>().spritesArray = new Dictionary<string, Sprite[]>();

        GameObject itemHolder = localPlayer
        .transform.Find("ItemHolder").gameObject;

        foreach (Transform child in itemHolder.transform)
        {
            child.GetComponent<SpriteRenderer>().sprite = null;
        }

        for (int i = 0; i < socketIOEvent.data["items"].Count; i++)
        {            
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(socketIOEvent.data["items"][i].ToString());

            string type1 = new CultureInfo("en-US").TextInfo.ToTitleCase(itemsJson.type);

            GameObject item = GameObject.Find("PlayerCard")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("Player Clothes")
                .transform.Find(type1).gameObject;

            item.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + itemsJson.picture + "PC");
            item.AddComponent<PolygonCollider2D>().isTrigger = true;
            item.GetComponent<ItemRemove>().id = itemsJson.item_id;
            item.GetComponent<ItemRemove>().type = itemsJson.type;

            if (itemsJson.type != "pin" && itemsJson.type != "background")
            {
                var subSprites = Resources.LoadAll<Sprite>("Animations/" + itemsJson.picture + itemsJson.item_id + "sprite");
                localPlayer.GetComponent<CharacterController>().spritesArray[itemsJson.type] = subSprites;

                //Debug.Log(itemsJson.picture);
            }
        }
    }

    void OnGetOnOtherPlayerItems(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString(socketIOEvent.data["items"]);
        
        GameObject otherPlayer = GameObject.Find(socketIOEvent.data["username"].str) as GameObject;
        Debug.Log(socketIOEvent.data["username"].str);

        otherPlayer.GetComponent<CharacterController>().spritesArray = new Dictionary<string, Sprite[]>();

        GameObject itemHolder = otherPlayer
                .transform.Find("ItemHolder").gameObject;

        foreach (Transform child in itemHolder.transform)
        {
            child.GetComponent<SpriteRenderer>().sprite = null;
        }

        for (int i = 0; i < socketIOEvent.data["items"].Count; i++)
        {
            PlayerItemsJSON itemsJson = PlayerItemsJSON.CreateFromJSON(socketIOEvent.data["items"][i].ToString());
           
            if (itemsJson.type != "pin" && itemsJson.type != "background")
            {
                var subSprites = Resources.LoadAll<Sprite>("Animations/" + itemsJson.picture + itemsJson.item_id + "sprite");
                Debug.Log(otherPlayer);
                otherPlayer.GetComponent<CharacterController>().spritesArray[itemsJson.type] = subSprites;
               
                otherPlayer.transform.Find("ItemHolder")
                    .transform.Find(itemsJson.type + "Sprite")
                    .GetComponent<SpriteRenderer>().sprite = subSprites[0];
            }
        }
    }

    void OnBuyItem(SocketIOEvent socketIOEvent)
    {
        Debug.Log(socketIOEvent);
    }

    void OnOtherPlayerDisconnected(SocketIOEvent socketIOEvent)
    {
        print("user disconnected");
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Destroy(GameObject.Find(userJSON.name));
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
        public int id;
        public String type;

        public ChangeItemJSON(int _id, String _type)
        {
            id = _id;
            type = _type;
        }
    }

    [Serializable]
    public class ItemJSON
    {
        public string name;
        public int id;

        public static ItemJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<ItemJSON>(data);
        }
    }


    [Serializable]
    public class PlayerItemsJSON
    {
        public int _id;
        public int user_id;
        public int item_id;
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
        public int id;
        public string name;
        public float[] position;
        public float[] animation;
        public int numAnimation;

        public static UserJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UserJSON>(data);
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