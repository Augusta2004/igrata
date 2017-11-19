using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Badge : MonoBehaviour {

    public string roomName;

    void Start()
    {
        NetworkManager.tempItemsLoaded = true;

        if (roomName == NetworkManager.sceneName)
        {
            this.transform.Find("Badge").gameObject.SetActive(true);
        }
    }
}
