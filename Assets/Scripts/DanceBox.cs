using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceBox : MonoBehaviour
{

    public GameObject danceBox;
    public GameObject emojiBox;

    void OnMouseDown()
    {
        if (danceBox.activeSelf)
        {
            danceBox.SetActive(false);
        }
        else
        {
            danceBox.SetActive(true);
        }

        emojiBox.SetActive(false);
    }
}
