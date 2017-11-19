using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {

    public bool isEnemy = false;
    public bool isGolden = false;
    public bool isHiding = false;
    private int countHiding = 0;

    void Update ()
    {
	    if(isHiding)
        {
            countHiding++;
            if(countHiding > 180)
            {
                isHiding = false;
                countHiding = 0;
                this.gameObject.SetActive(false);
            }
        }
	}
}
