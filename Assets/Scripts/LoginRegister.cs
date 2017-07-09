﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginRegister : MonoBehaviour {

    //public static LoginRegister instance;

    public GameObject login;
    public GameObject loginLink;
    public GameObject back;


    public void Back()
    {
        login.SetActive(false);
        loginLink.SetActive(true);
        back.SetActive(false);
    }

    public void Login()
    {
        string username = GameObject.Find("LoginUsername").transform.Find("Text").GetComponent<Text>().text;
        string pass = GameObject.Find("LoginPass").GetComponent<InputField>().text;

        string data = JsonUtility.ToJson(new LoginJSON(username, pass));

        NetworkManager.instance.GetComponent<NetworkManager>().UserLogin(data);
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