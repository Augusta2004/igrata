using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour {

    public float speed = 10;
	
	void Start () {
		
	}
	
	
	void Update () {
        transform.position += Vector3.left * speed * Time.deltaTime;
	}
}
