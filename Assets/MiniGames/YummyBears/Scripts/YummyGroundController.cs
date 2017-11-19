using UnityEngine;
using System.Collections;

public class YummyGroundController : MonoBehaviour
{
	void OnCollisionEnter2D(Collision2D c)
	{
		if (c.gameObject.CompareTag ("Bot"))
		{
            Destroy(c.gameObject);
		}
	}
}
