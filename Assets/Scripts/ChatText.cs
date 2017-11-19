using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatText : MonoBehaviour
{
    [HideInInspector]
    public float showTime;

    public bool sent = false;
    public bool sendViaButton = false;
    public InputField input;

    private string message;    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessage();
        }

        if (sent && Time.time - showTime > 4)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().PlayerChat("", "");
            sent = false;
        }
    }

    private void OnMouseDown()
    {
        if (sendViaButton)
        {
            SendMessage();
        }
    }

    void SendMessage()
    {
        message = input.GetComponent<InputField>().text.ToString().Trim();

        string[] words = new string[] { "fuck", "gay", "homo", "fag", "twat", "dick", "pussy", "cock", "suck" };

        if (message != "")
        {
            bool send = true;
            string messageLower = message.ToLower();

            foreach (string w in words)
            {
                if (messageLower.Contains(w))
                {
                    input.GetComponent<InputField>().text = "";
                    send = false;
                    break;
                }
            }

            if (send)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().PlayerChat(message, "");

                input.GetComponent<InputField>().text = "";
                showTime = Time.time;

                sent = true;
            }
        }
    }
}
