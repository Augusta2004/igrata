﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRequest : MonoBehaviour {
    
   // [HideInInspector]
    public string request_id;
 
    public int requestOrder;

    private void OnMouseDown()
    {
        FriendsController.requestId = request_id;
        FriendsController.requestOrder = requestOrder;

        GameObject.Find("RequestDialogHolder").transform.Find("RequestDialogBox").gameObject.SetActive(true);        
    }
}
