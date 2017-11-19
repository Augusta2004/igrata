using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPremium : MonoBehaviour {

    public GameObject premium;

    private void OnMouseDown()
    {
        premium.SetActive(!premium.activeSelf);
    }
}
