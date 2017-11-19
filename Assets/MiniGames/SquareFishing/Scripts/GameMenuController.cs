using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour {

    public void Play()
    {
        SceneManager.LoadScene("SquareFishingPlay");
    }

    public void Exit()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom("Room1");
    }
}