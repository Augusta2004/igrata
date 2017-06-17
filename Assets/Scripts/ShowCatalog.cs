using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCatalog : MonoBehaviour
{

    public GameObject Catalog;
    
    void OnMouseDown()
    {
        Catalog.SetActive(true);
    }
}