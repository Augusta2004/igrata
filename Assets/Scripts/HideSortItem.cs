using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSortItem : MonoBehaviour {
    
    private void OnMouseDown()
    {
        Debug.Log("Gonna hide sort items");
        if(this.transform.Find("Dropdown List") != null)
        {
            this.transform.Find("Dropdown List").gameObject.SetActive(!this.transform.Find("Dropdown List").gameObject.activeSelf);
        }
    }
}
