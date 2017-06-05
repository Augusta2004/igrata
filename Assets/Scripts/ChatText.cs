using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatText : MonoBehaviour
{
    private float showTime;
    public float emojiShowTime;
    
    public void ShowText()
    {
        StartCoroutine(SendText());
    }

    IEnumerator SendText()
    {        
        //chatMessage.text = this.GetComponent<InputField>().text;
        //emojiSprite.GetComponent<SpriteRenderer>().sprite = null;

        NetworkManager.instance.GetComponent<NetworkManager>().PlayerChat(this.GetComponent<InputField>().text.ToString(), "");
        
        this.GetComponent<InputField>().text = "";
        showTime = Time.time;

        yield return new WaitForSeconds(4.5f);
        
        if (Time.time - showTime > 4)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().PlayerChat("", "");
        }
    }
}
