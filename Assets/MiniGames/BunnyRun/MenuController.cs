﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
			SceneManager.LoadScene("Room2");
        }
	}

    public void StartGame()
    {
        SceneManager.LoadScene("BunnyRunGame");
    }

    public void ExitGame()
    {
		SceneManager.LoadScene("Room2");
    }
}