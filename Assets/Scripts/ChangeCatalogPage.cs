using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCatalogPage : MonoBehaviour
{
    public GameObject Catalog;
    public GameObject PrevPage;
    public GameObject NextPage;

    private int maxPages = 0;

    private void OnMouseDown()
    {
        int num = 1;

        foreach (Transform g in GameObject.Find("Catalog").transform.Find("CatalogHolder").transform.Find("Pages"))
        {
            if (g.gameObject.activeSelf)
            {
                break;
            }
            num++;
        }

        Debug.Log(num);

        maxPages = GameObject.Find("Catalog").transform.Find("CatalogHolder").transform.Find("Pages").childCount;

        if (this.gameObject.name == "Close")
        {
            Catalog.SetActive(false);

            GameObject.Find("Catalog")
               .transform.Find("CatalogHolder")
               .transform.Find("Pages")
               .transform.Find("Page " + num)
               .gameObject.SetActive(false);

            GameObject.Find("Catalog")
               .transform.Find("CatalogHolder")
               .transform.Find("Pages")
               .transform.Find("Page 1")
               .gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("Catalog")
              .transform.Find("CatalogHolder")
              .transform.Find("Pages")
              .transform.Find("Page " + num)
              .gameObject.SetActive(false);

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

            GameObject.Find("Catalog")
                .transform.Find("CatalogHolder")
                .transform.Find("Pages")
                .transform.Find("Page " + num)
                .gameObject.SetActive(true);

            Debug.Log(num);
        }
    }
}