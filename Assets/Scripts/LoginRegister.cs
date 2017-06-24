using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginRegister : MonoBehaviour {

    //public static LoginRegister instance;
    public SocketIOComponent socket;

    public GameObject login;
    public GameObject register;
    public GameObject loginLink;
    public GameObject regLink;
    public GameObject registerSuccess;
    public GameObject back;

    private bool isServerAvailable = true;
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
        //socket.On("user register", OnUserRegister);
        //socket.On("user login", OnUserLogin);
    }

    /*
    void OnUserRegister(SocketIOEvent socketIOEvent)
    {
        string data = socketIOEvent.data.ToString();

        ErrorJSON errorJSON = ErrorJSON.CreateFromJSON(data);
        
        Debug.Log(errorJSON.errors.Length);

       if (errorJSON.errors.Length > 0)
        {
            string errStr = "";
            for(int i = 0; i < errorJSON.errors.Length; i++)
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
        }
    }
    */

        /*
    public void LoginLink(bool onRegister = false)
    {
        if(onRegister)
        {
            registerSuccess.SetActive(true);
            register.SetActive(false);
        }

        login.SetActive(true);
        loginLink.SetActive(false);
        regLink.SetActive(false);
        back.SetActive(true);
    }
    */

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
        string username = GameObject.Find("LoginUsername").transform.Find("Text").GetComponent<Text>().text;
        string pass = GameObject.Find("LoginPass").transform.Find("Text").GetComponent<Text>().text;

        string data = JsonUtility.ToJson(new LoginJSON(username, pass));
        //TODO Networking
        NetworkManager.instance.GetComponent<NetworkManager>().UserLogin(new JSONObject(data));
        //socket.Emit("user login", new JSONObject(data));
    }

    public void Register()
    {
        if (isServerAvailable) //Check if prevous request is finished
        {
            string username = GameObject.Find("RegUsername").transform.Find("Text").GetComponent<Text>().text;
            string mail = GameObject.Find("RegMail").transform.Find("Text").GetComponent<Text>().text;
            string pass = GameObject.Find("RegPass").transform.Find("Text").GetComponent<Text>().text;
            string pass2 = GameObject.Find("RegPass2").transform.Find("Text").GetComponent<Text>().text;

            string data = JsonUtility.ToJson(new RegisterJSON(username, mail, pass, pass2));
            //TODO Networking

            NetworkManager.instance.GetComponent<NetworkManager>().UserRegister(new JSONObject(data));
            //socket.Emit("user register", new JSONObject(data));


            Debug.Log(username + " " + mail + " " + pass + " " + pass2);

            isServerAvailable = false;
        }
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
    public class LoginJSON
    {
        public string username;
        public string password;

        public LoginJSON(string _username, string _password)
        {
            username = _username;
            password = _password;
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