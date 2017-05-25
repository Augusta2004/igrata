using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNewsPaper : MonoBehaviour
{

    public GameObject Newspaper;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        Newspaper.SetActive(true);
    }
}