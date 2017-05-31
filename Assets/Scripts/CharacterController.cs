using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private bool _move = true;
    public float _speed;
    private Vector2 _player;
    private Vector2 direction = new Vector2();
    private Vector2 position = new Vector2();

    Rigidbody2D rbody;
    Animator anim;

    public bool isLocalPlayer = false;
    //Vector2 oldPosition;
    //Vector2 currentPosition;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        Physics2D.IgnoreLayerCollision(10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        position = gameObject.transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            _player = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            _move = true;
        }

        if (_player != Vector2.zero && (_player - position).magnitude >= .06)
        {
            if (_move == true)
            {
                direction = (_player - position).normalized;
                rbody.MovePosition(rbody.position + _speed * direction * Time.deltaTime);
                anim.SetBool("isWalking", true);
                anim.SetFloat("input_x", direction.x);
                anim.SetFloat("input_y", direction.y);

                NetworkManager.instance.GetComponent<NetworkManager>().CommandMove(transform.position, direction);
            }
        }
        else
        {
            if (_move == true)
            {
                anim.SetBool("isWalking", false);
                NetworkManager.instance.GetComponent<NetworkManager>().StopAnimation();
            }

            _move = false;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        anim.SetBool("isWalking", false);
        NetworkManager.instance.GetComponent<NetworkManager>().StopAnimation(); //Send command to server, so other players will know we stopped moving

        rbody.MovePosition(rbody.position - _speed * direction * Time.deltaTime);
        _move = false;
        //TODO NETWORING
    }
}