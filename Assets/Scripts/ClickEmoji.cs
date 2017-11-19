using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickEmoji : MonoBehaviour
{
    public GameObject chatText;

    void OnMouseDown()
    {
        NetworkManager.instance.GetComponent<NetworkManager>().PlayerChat("", "");

        chatText.GetComponent<ChatText>().sent = true;
        chatText.GetComponent<ChatText>().showTime = Time.time;

        string path = "Sprites/Menu/Emoji/" + this.gameObject.GetComponent<SpriteRenderer>().sprite.name;

        NetworkManager.instance.GetComponent<NetworkManager>().SendEmoji(path);
    }

}