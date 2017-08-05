using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePage : MonoBehaviour {

    public GameObject NewspaperHolder;
    public GameObject PrevPage;
    public GameObject NextPage;
    public GameObject Links;

    public int GoToPage;

    private int maxPages = 3;

    private void OnMouseDown()
    {
        if(this.gameObject.name == "Close")
        {
            NewspaperHolder.SetActive(false);
            NextPage.SetActive(true);
            PrevPage.SetActive(false);
            Links.SetActive(true);
        }
        else
        {
            string pagename = NewspaperHolder.GetComponent<SpriteRenderer>().sprite.ToString().Split(' ')[0];
            int num = Int32.Parse(pagename.Substring(pagename.Length - 1));

            if (this.gameObject.name == "PrevPage")
            {
                num--;
                if (num == 1)
                {
                    this.gameObject.SetActive(false);
                    Links.SetActive(true);
                }

                NextPage.SetActive(true);
            }
            else
            {
                if (this.gameObject.name == "NextPage")
                {
                    num++;
                }
                else
                {
                    num = GoToPage;
                }

                if (num == maxPages)
                {
                    NextPage.SetActive(false);
                }

                PrevPage.SetActive(true);
                Links.SetActive(false);
            }

            pagename = "newspaperpage" + num;

            Sprite spr = Resources.Load<Sprite>("Sprites/Newspaper/" + pagename);
            NewspaperHolder.GetComponent<SpriteRenderer>().sprite = spr;
        }
    }
}
