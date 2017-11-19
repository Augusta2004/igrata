using UnityEngine;
using System.Collections;
using System;

public class YummyRandomColor : MonoBehaviour
{
	private System.Random r = new System.Random();

    private void Start()
    {
        //r = new System.Random();
    }

    public Color32 GetRandomColor()
    {
        int n = r.Next (0, 9);
		
		switch (n) {
		case 0:
			return new Color32(70, 70, 70, 255);
			
		case 1:
			return new Color32(192, 32, 0, 255);
		case 2:
			return new Color32(30, 71, 126, 255);
		case 3:
			return new Color32(32, 161, 53, 255);
		case 4:
			return new Color32(226, 234, 0, 255);
		case 5:
			return new Color32(0, 255, 246, 255);
		case 6:
			return new Color32(234, 132, 0, 255);
		case 7:
			return new Color32(206, 58, 204, 255);
		default:
			return new Color32(239, 239, 239, 255);
		}
	}
}
