using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YummyClose : MonoBehaviour {

    public void CloseGame()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom("Room1");
    }
}