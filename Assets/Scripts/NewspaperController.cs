using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewspaperController : MonoBehaviour {

    public GameObject NewspaperHolder;
    public GameObject PrevPage;
    public GameObject NextPage;
    public GameObject Links;

    public int goToPage;

    private int maxPages = 0;

    private void OnMouseDown()
    {

        int num = 1;

        foreach(Transform g in GameObject.Find("Newspaper").transform.Find("NewspaperHolder").transform.Find("Pages"))
        {
            if(g.transform.Find("sprite").gameObject.activeSelf)
            {
                break;
            }
            num++;
        }

        Debug.Log(num);

        maxPages = GameObject.Find("Newspaper").transform.Find("NewspaperHolder").transform.Find("Pages").childCount;

        if (this.gameObject.name == "Close")
        {
            NewspaperHolder.SetActive(false);
            NextPage.SetActive(true);
            PrevPage.SetActive(false);
            Links.SetActive(true);

            GameObject.Find("Newspaper")
               .transform.Find("NewspaperHolder")
               .transform.Find("Pages")
               .transform.Find("Page " + num)
               .transform.Find("sprite").gameObject.SetActive(false);

            GameObject.Find("Newspaper")
               .transform.Find("NewspaperHolder")
               .transform.Find("Pages")
               .transform.Find("Page 1")
               .transform.Find("sprite").gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(num);

            GameObject.Find("Newspaper")
               .transform.Find("NewspaperHolder")
               .transform.Find("Pages")
               .transform.Find("Page " + num)
               .transform.Find("sprite").gameObject.SetActive(false);

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
                    num = goToPage;
                }

                if (num == maxPages)
                {
                    NextPage.SetActive(false);
                }

                PrevPage.SetActive(true);
                Links.SetActive(false);
            }

            Debug.Log(num);

            GameObject.Find("Newspaper")
                .transform.Find("NewspaperHolder")
                .transform.Find("Pages")
                .transform.Find("Page " + num)
                .transform.Find("sprite").gameObject.SetActive(true);
        }
    }
}