using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class EnterPremiumCode : MonoBehaviour {

    public GameObject input;

    public void EnterCode()
    {
        string text = input.transform.Find("Text").GetComponent<Text>().text.Trim();
        NetworkManager.instance.GetComponent<NetworkManager>().CheckPremiumCode(text);
    }
}
