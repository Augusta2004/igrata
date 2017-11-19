using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeRoom : MonoBehaviour {

	public string roomName;
    public bool clickTravel; //Travel with click or colide

	void OnTriggerEnter2D(Collider2D other)
	{
        if (!clickTravel)
        {
            if (NetworkManager.sceneName != roomName)
            {
                if (other.gameObject.CompareTag("Player") && NetworkManager.localUsername == other.name)
                {
                    NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom(roomName);
                    //NetworkManager.instance.GetComponent<NetworkManager>().ConnectToServer();
                    //SceneManager.LoadScene (roomName);	
                }
            }
            else
            {
                this.transform.parent.gameObject.SetActive(!this.transform.parent.gameObject.activeSelf);
            }
        }
	}

    private void OnMouseDown()
    {
        if(clickTravel)
        {
            if (NetworkManager.sceneName != roomName)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom(roomName);
            }
            else
            {
                this.transform.parent.gameObject.SetActive(!this.transform.parent.gameObject.activeSelf);
            }
        }
    }
}
