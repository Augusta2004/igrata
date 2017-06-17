using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNewsPaper : MonoBehaviour
{

    public GameObject Newspaper;

    void OnMouseDown()
    {
        Newspaper.SetActive(true);
    }
}