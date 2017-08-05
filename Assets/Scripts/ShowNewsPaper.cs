using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNewsPaper : MonoBehaviour
{
    void OnMouseDown()
    {
        //Newspaper.SetActive(true);
        if (GameObject.Find("Newspaper") == null)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().LoadAssetBundles("Newspaper/", false, "newspaper", false);
        }
        else
        {
            GameObject newspaper = GameObject.Find("Newspaper").transform.Find("NewspaperHolder").gameObject;
           
            newspaper.SetActive(!newspaper.activeSelf);            
        }
        //NetworkManager.instance.GetComponent<NetworkManager>().LoadAssetBundles("SpawnPoints/", false, "room1", false);
    } 
}