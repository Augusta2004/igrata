using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeRoom : MonoBehaviour {

	public string roomName;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag ("Player") && NetworkManager.localUsername == other.name)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom(roomName);
            //NetworkManager.instance.GetComponent<NetworkManager>().ConnectToServer();
            //SceneManager.LoadScene (roomName);	
        }		
	}   
}
