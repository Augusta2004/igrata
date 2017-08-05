using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
	}

    public void StartGame()
    {
        SceneManager.LoadScene("BunnyRunGame");
    }

    public void ExitGame()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom("Room2");
    }
}
