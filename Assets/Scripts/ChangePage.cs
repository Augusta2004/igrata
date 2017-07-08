using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePage : MonoBehaviour {

    public GameObject Newspaper;
    public GameObject PrevPage;
    public GameObject NextPage;
    public GameObject Link1;
    public GameObject Link2;

    public int GoToPage;

    private int maxPages = 3;

    private void OnMouseDown()
    {
        
        if(this.gameObject.name == "Close")
        {
            Newspaper.SetActive(false);
        }
        else
        {
            string pagename = Newspaper.GetComponent<SpriteRenderer>().sprite.ToString().Split(' ')[0];
            int num = Int32.Parse(pagename.Substring(pagename.Length - 1));

            if (this.gameObject.name == "PrevPage")
            {
                num--;
                if (num == 1)
                {
                    this.gameObject.SetActive(false);
                    Link1.SetActive(true);
                    Link2.SetActive(true);
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
                Link1.SetActive(false);
                Link2.SetActive(false);
            }

            pagename = "newspaperpage" + num;

            Sprite spr = Resources.Load<Sprite>("Sprites/Newspaper/" + pagename);
            Newspaper.GetComponent<SpriteRenderer>().sprite = spr;
        }
    }
}
