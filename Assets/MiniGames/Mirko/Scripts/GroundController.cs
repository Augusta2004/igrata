using UnityEngine;
using System.Collections;

public class GroundController : MonoBehaviour
{
	void OnCollisionEnter2D(Collision2D c)
	{
		if (c.gameObject.CompareTag ("Bot"))
		{
			c.gameObject.SetActive(false);
		}
	}
}
