using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDance : MonoBehaviour
{
    void OnMouseDown()
    {
        this.transform.parent.gameObject.SetActive(false);
    }
}