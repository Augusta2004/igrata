using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitFishing : MonoBehaviour {

	void OnMouseDown()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom("Room1");
    }
}