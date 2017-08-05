using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowItems : MonoBehaviour
{
    public GameObject itemsHolder;

    void OnMouseDown()
    {
        if(!itemsHolder.activeSelf)
        {
            itemsHolder.SetActive(true);
            GameObject canvas = GameObject.Find("PlayerCard").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("PlayerCardCanvas").gameObject;
            canvas.transform.Find("SortHolder").gameObject.SetActive(true);

            var x = itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x;
            this.transform.position = new Vector2(this.transform.position.x + x, this.transform.position.y);

            canvas.transform.Find("SortHolder").transform.Find("Type").gameObject.GetComponent<Dropdown>().value = 0;
        }
        else
        {
            GameObject canvas = GameObject.Find("PlayerCard").transform.Find("DragPlayerCard").transform.Find("PlayerCardHolder").transform.Find("PlayerCardCanvas").gameObject;
            canvas.transform.Find("SortHolder").gameObject.SetActive(false);


            itemsHolder.SetActive(false);
            var x = itemsHolder.GetComponent<SpriteRenderer>().bounds.size.x;
            this.transform.position = new Vector2(this.transform.position.x - x, this.transform.position.y);
        }

    }
}