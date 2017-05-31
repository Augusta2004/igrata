using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerCard;

    void OnMouseDown()
    {
        playerCard.SetActive(true);
    }
}
