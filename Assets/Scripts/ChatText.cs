using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatText : MonoBehaviour
{
    [HideInInspector]
    public float showTime;

    public bool sent = false;

    private string message;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            message = this.GetComponent<InputField>().text.ToString().Trim();
            if (message != "")
            {
                NetworkManager.instance.GetComponent<NetworkManager>().PlayerChat(message, "");

                this.GetComponent<InputField>().text = "";
                showTime = Time.time;

                sent = true;
            }
        }

        if (sent && Time.time - showTime > 4)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().PlayerChat("", "");
            sent = false;
        }
    }
}
