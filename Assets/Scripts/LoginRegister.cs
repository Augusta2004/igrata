using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginRegister : MonoBehaviour {

    //public static LoginRegister instance;
    public SocketIOComponent socket;

    public GameObject login;
    public GameObject register;
    public GameObject loginLink;
    public GameObject regLink;
    public GameObject back;

   /* private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }*/

    private void Start()
    {
        socket.On("user register", OnUserRegister);
    }

    void OnUserRegister(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        //ErrorJSON errorJSON = ErrorJSON.CreateFromJSON(data);

        Debug.Log(data);
    }

    public void LoginLink()
    {
        login.SetActive(true);
        loginLink.SetActive(false);
        regLink.SetActive(false);
        back.SetActive(true);
    }

    public void RegLink()
    {
        register.SetActive(true);
        loginLink.SetActive(false);
        regLink.SetActive(false);
        back.SetActive(true);
    }

    public void Back()
    {
        login.SetActive(false);
        register.SetActive(false);
        loginLink.SetActive(true);
        regLink.SetActive(true);
        back.SetActive(false);
    }

    public void Login()
    {
        Debug.Log("Login");
    }

    public void Register()
    {
        string username = GameObject.Find("RegUsername").transform.Find("Text").GetComponent<Text>().text;
        string mail = GameObject.Find("RegMail").transform.Find("Text").GetComponent<Text>().text;
        string pass = GameObject.Find("RegPass").transform.Find("Text").GetComponent<Text>().text;
        string pass2 = GameObject.Find("RegPass2").transform.Find("Text").GetComponent<Text>().text;

        string data = JsonUtility.ToJson(new RegisterJSON(username, mail, pass, pass2));
        //TODO Networking
        socket.Emit("user register", new JSONObject(data));

        Debug.Log(username + " " + mail + " " + pass + " " + pass2);
    }

    [Serializable]
    public class RegisterJSON
    {
        public string username;
        public string mail;
        public string password;
        public string password2;

        public RegisterJSON(string _username, string _mail, string _password, string _password2)
        {
            username = _username;
            mail = _mail;
            password = _password;
            password2 = _password2;
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

}