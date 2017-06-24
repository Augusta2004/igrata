using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Close : MonoBehaviour {

    public void CloseGame()
    {
        NetworkManager.sceneName = "Room1";
        NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom("Room1");
    }
}
