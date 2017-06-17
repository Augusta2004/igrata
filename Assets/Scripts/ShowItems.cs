using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowItems : MonoBehaviour
{
    public GameObject itemsHolder;

    void OnMouseDown()
    {
        if(!itemsHolder.activeSelf)
        {
            itemsHolder.SetActive(true);
            var x = itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x;
            this.transform.position = new Vector2(this.transform.position.x + x, this.transform.position.y);
        }
        else
        {
            itemsHolder.SetActive(false);
            var x = itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x;
            this.transform.position = new Vector2(this.transform.position.x - x, this.transform.position.y);
        }

    }
}