using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickEmoji : MonoBehaviour
{
    //public GameObject emojiSprite;
    // public GameObject ChatInput;
    //public Text chatText;  //Players chatText

    void OnMouseDown()
    {
        /*emojiSprite.GetComponent<SpriteRenderer>().sprite = this.gameObject.GetComponent<SpriteRenderer>().sprite;
        chatText.text = "";
        ChatInput.GetComponent<ChatText>().emojiShowTime = Time.time;
        this.transform.parent.gameObject.SetActive(false);
        Debug.Log(emojiSprite.GetComponent<SpriteRenderer>().sprite);
        */
        //Debug.Log(AssetDatabase.GetAssetPath(this.gameObject.GetComponent<SpriteRenderer>().sprite));
        string path = "Sprites/Menu/" + this.gameObject.GetComponent<SpriteRenderer>().sprite.name;

        //Debug.Log(path);

        NetworkManager.instance.GetComponent<NetworkManager>().SendEmoji(path);
    }    
}