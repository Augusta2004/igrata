using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour {
    //
    public float _speed;
    Rigidbody2D rbody;
    Animator anim;
    private Vector2 _player;

    private bool _move = true;

    private Vector2 direction = new Vector2();
    private Vector2 position = new Vector2();
    // Use this for initialization
    void Start () {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        position = gameObject.transform.position;


        if (Input.GetMouseButtonDown(0))
        {
			

            _player = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            _move = true;


			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100))
			{
				Debug.Log(hit.transform.gameObject.name);
			}
			Debug.Log(Physics.Raycast (ray, out hit, 10000));
        }
        


        if (_player != Vector2.zero && (_player - position).magnitude >= .06)
        {
            if (_move == true)
            {
                //rbody.isKinematic = false;

                direction = (_player - position).normalized;
                rbody.MovePosition(rbody.position + _speed * direction * Time.deltaTime);
                anim.SetBool("isWalking", true);
                anim.SetFloat("input_x", direction.x);
                anim.SetFloat("input_y", direction.y);
            }
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        anim.SetBool("isWalking", false);
        rbody.MovePosition(rbody.position - _speed * direction * Time.deltaTime);
        _move = false;
    }
}