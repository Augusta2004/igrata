using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatText : MonoBehaviour
{
    private float showTime;
    public float emojiShowTime;
    public Text chatMessage;
    public GameObject emojiSprite;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - showTime > 4)
        {
            chatMessage.text = "";
        }
        
        //hide emoji
        if (Time.time - emojiShowTime > 4)
        {
            emojiSprite.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public void ShowText()
    {
        chatMessage.text = this.GetComponent<InputField>().text;
        emojiSprite.GetComponent<SpriteRenderer>().sprite = null;
        showTime = Time.time;
    }
}
