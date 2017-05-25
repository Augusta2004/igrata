using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiBox : MonoBehaviour {

    public GameObject emojiBox;
    public GameObject danceBox;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDown()
    {
        if (emojiBox.activeSelf)
        {
            emojiBox.SetActive(false);
        }
        else
        {
            emojiBox.SetActive(true);
        }

        danceBox.SetActive(false);
    }
}
