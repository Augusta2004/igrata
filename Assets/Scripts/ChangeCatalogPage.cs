using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCatalogPage : MonoBehaviour
{

    public GameObject Catalog;
    public GameObject PrevPage;
    public GameObject NextPage;

    public int GoToPage;

    private int maxPages = 2;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {

        if (this.gameObject.name == "Close")
        {
            Catalog.SetActive(false);
        }
        else
        {
            string pagename = Catalog.GetComponent<SpriteRenderer>().sprite.ToString().Split(' ')[0];
            int num = Int32.Parse(pagename.Substring(pagename.Length - 1));

            if (this.gameObject.name == "PrevPage")
            {
                num--;
                if (num == 1)
                {
                    this.gameObject.SetActive(false);
                }

                NextPage.SetActive(true);
            }
            else
            {
                if (this.gameObject.name == "NextPage")
                {
                    num++;
                }

                if (num == maxPages)
                {
                    NextPage.SetActive(false);
                }

                PrevPage.SetActive(true);
            }

            pagename = "catalogbackground" + num;

            Sprite spr = Resources.Load<Sprite>("Sprites/Catalog/" + pagename);
            Catalog.GetComponent<SpriteRenderer>().sprite = spr;
        }
    }
}
