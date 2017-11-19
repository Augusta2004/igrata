using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class LoginRegister : MonoBehaviour {


    public static LoginRegister instance;

    public GameObject login;
    public GameObject register;
    public GameObject bear;
    public GameObject registerLink;
    public GameObject back;
    public GameObject registerSuccessText;

    EventSystem system;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        system = EventSystem.current;// EventSystemManager.currentSystem;
    }

    void Update()
    {
       if (system.currentSelectedGameObject != null && system.currentSelectedGameObject.GetComponent<InputField>() != null && Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(system));

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if(login.activeSelf)
            {
                Login();
            }
            else
            {
                Register();
            }
        }
    }

    public void Back(bool registerSuccess = false)
    {
        login.SetActive(true);
        register.SetActive(false);
        bear.SetActive(true);
        registerLink.SetActive(true);
        back.SetActive(false);

        if(registerSuccess)
        {
            registerSuccessText.SetActive(true);
        }
    }

    public void RegiterLink()
    {
        login.SetActive(false);
        register.SetActive(true);
        bear.SetActive(false);
        registerLink.SetActive(false);
        back.SetActive(true);
        registerSuccessText.SetActive(false);
    }

    public void Login()
    {
        string username = GameObject.Find("LoginUsername").transform.Find("Text").GetComponent<Text>().text;
        string pass = GameObject.Find("LoginPass").GetComponent<InputField>().text;

        if(username.Length > 0 && pass.Length > 0)
        {
            string data = JsonUtility.ToJson(new LoginJSON(username, pass));
            NetworkManager.instance.GetComponent<NetworkManager>().UserLogin(data);
        }
    }

    public void Register()
    {
        string username = GameObject.Find("RegUsername").transform.Find("Text").GetComponent<Text>().text;
        string mail = GameObject.Find("RegMail").transform.Find("Text").GetComponent<Text>().text;
        string pass = GameObject.Find("RegPass").GetComponent<InputField>().text;
        string pass2 = GameObject.Find("RegPass2").GetComponent<InputField>().text;

        if (username.Length > 0 && pass.Length > 0 && pass2.Length > 0 && mail.Length > 0)
        {
            string data = JsonUtility.ToJson(new RegisterJSON(username, mail, pass, pass2));
            NetworkManager.instance.GetComponent<NetworkManager>().UserRegister(data);
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