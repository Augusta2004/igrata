using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPlayerCard : MonoBehaviour {
    
    void OnMouseDown()
    {
        GameObject.Find("Player").GetComponent<Player>().ShowItems();
    }
}
