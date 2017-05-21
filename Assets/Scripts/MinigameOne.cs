using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameOne : MonoBehaviour {

	public string minigame;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayMinigame()
	{
		SceneManager.LoadScene (minigame);	
	}
		
}
