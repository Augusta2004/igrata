using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControllerSkiing : MonoBehaviour {

    public void Play()
    {
        SceneManager.LoadScene("PlaySkiing");
    }

    public void Exit()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom("Room1");
    }

}