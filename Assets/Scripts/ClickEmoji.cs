using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickEmoji : MonoBehaviour
{

    public GameObject emojiSprite;
    public GameObject ChatInput;
    public Text chatText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnMouseDown()
    {
        emojiSprite.GetComponent<SpriteRenderer>().sprite = this.gameObject.GetComponent<SpriteRenderer>().sprite;
        chatText.text = "";
        ChatInput.GetComponent<ChatText>().emojiShowTime = Time.time;
        this.transform.parent.gameObject.SetActive(false);
        Debug.Log(emojiSprite.GetComponent<SpriteRenderer>().sprite);
    }
}