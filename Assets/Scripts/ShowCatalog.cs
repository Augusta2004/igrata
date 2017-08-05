using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCatalog : MonoBehaviour
{    
    void OnMouseDown()
    {        
        if (GameObject.Find("Catalog") == null)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().LoadAssetBundles("Catalog/", false, "catalog", false);
        }
        else
        {
            GameObject catalog = GameObject.Find("Catalog").transform.Find("CatalogHolder").gameObject;

            catalog.SetActive(!catalog.activeSelf);
        }
    }
}