using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameOne : MonoBehaviour {

	public string minigame;


    // Use this for initialization

    private void OnMouseDown()
    {
        PlayMinigame();
    }

    public void PlayMinigame()
	{
        NetworkManager.sceneName = minigame;
        NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom(minigame);
    }
		
}
