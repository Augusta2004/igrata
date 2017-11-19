using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyPremium : MonoBehaviour {

	public void LoadPremiumPage()
    {
        Application.OpenURL("http://polar-adventures.com/premium");
    }
}
