using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDance : MonoBehaviour
{


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
        this.transform.parent.gameObject.SetActive(false);
    }
}