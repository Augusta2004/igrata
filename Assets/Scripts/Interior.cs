using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interior : MonoBehaviour {

    GameObject obj;
    int sortingOrder;

    private void Start()
    {
        if(this.transform.parent.gameObject.layer == 10)
        {
            obj = this.transform.parent.gameObject;
        }
        else if(this.transform.gameObject.layer == 12)
        {
            obj = this.transform.gameObject;
        }

        SetOrder();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((obj.layer == 10 || obj.layer == 12) && (collision.gameObject.layer == 10 || collision.gameObject.layer == 12))
        {
            SetOrder(collision);
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(obj.layer == 12 && collision.gameObject.layer == 10 && collision.GetType() == typeof(PolygonCollider2D))
        {
            float playerPointY = collision.transform.Find("InteriorPoint").transform.position.y;
            float objPointY = obj.transform.Find("InteriorPoint").transform.position.y;
            int playerLayer = collision.GetComponent<SpriteRenderer>().sortingOrder;
            int objLayer = obj.GetComponent<MeshRenderer>().sortingOrder;

            if (playerPointY > objPointY && playerLayer >= objLayer)
            {
                Debug.Log("On stay over interior");
                SetOrder(collision);
            }
            else if(playerPointY < objPointY && playerLayer <= objLayer)
            {
                Debug.Log("On stay below interior");
                SetOrder(collision);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((obj.layer == 10 || obj.layer == 12) && (collision.gameObject.layer == 10 || collision.gameObject.layer == 12))
        {
            SetOrder(collision);
        }
    }

    private void SetOrder(Collider2D collision = null)
    {
        if(obj.layer == 10)
        {
            if(collision != null && collision.gameObject.layer == 10 && collision.GetType() != typeof(PolygonCollider2D))
            {
                return;
            }

            obj.GetComponent<SpriteRenderer>().sortingOrder = (int)((this.transform.position.y + 4.502) * -800);
            obj.transform.Find("PopularityRing").GetComponent<SpriteRenderer>().sortingOrder = (int)((this.transform.position.y + 4.502) * -800) - 1;

            int i = 0;
            foreach(Transform item in obj.transform.Find("ItemHolder"))
            {
                i++;
                item.GetComponent<SpriteRenderer>().sortingOrder = (int)((this.transform.position.y + 4.502) * -800) + i;
            }
        }
        else if(obj.layer == 12)
        {
            obj.GetComponent<MeshRenderer>().sortingOrder = (int)((obj.transform.Find("InteriorPoint").transform.position.y + 4.502) * -800);
        }

        if(collision != null)
        {
            if (collision.gameObject.layer == 10 && collision.GetType() == typeof(PolygonCollider2D))
            {
                collision.GetComponent<SpriteRenderer>().sortingOrder = (int)((collision.transform.Find("InteriorPoint").transform.position.y + 4.52) * -800);

                collision.transform.Find("PopularityRing").GetComponent<SpriteRenderer>().sortingOrder = (int)((collision.transform.Find("InteriorPoint").transform.position.y + 4.52) * -800) - 1;

                int i = 0;
                foreach (Transform item in collision.transform.Find("ItemHolder"))
                {
                    i++;
                    item.GetComponent<SpriteRenderer>().sortingOrder = (int)((collision.transform.Find("InteriorPoint").transform.position.y + 4.502) * -800) + i;
                }
            }
        }
    }
}