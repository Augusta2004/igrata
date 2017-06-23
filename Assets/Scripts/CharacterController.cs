using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private bool _move = true;
    public float _speed;
    private Vector2 _player;
    private Vector2 direction = new Vector2();
    private Vector2 position = new Vector2();

    //[HideInInspector]
    public Dictionary<string, Sprite[]> spritesArray = new Dictionary<string, Sprite[]>();

    Rigidbody2D rbody;
    Animator anim;

    public bool isLocalPlayer = false;
    
    private Ray ray;
    private RaycastHit2D hit;
    private Ray ray1;
    private RaycastHit2D hit1;
    private int count = 0;


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
            ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            hit1 = Physics2D.Raycast(ray1.origin, ray1.direction, Mathf.Infinity);

            if ((hit && hit.collider.tag != "Walk") || (hit1 && hit1.collider.tag != "Walk"))
            {
                return;
            }
            else
            {
                _player = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

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

                string currentAnimation = this.GetComponent<SpriteRenderer>().sprite.name;
                int numAnimation = Convert.ToInt32(currentAnimation.Split('_')[1]);

                NetworkManager.instance.GetComponent<NetworkManager>().CommandMove(transform.position, direction, numAnimation);
                
                foreach (KeyValuePair<string, Sprite[]> entry in spritesArray)
                {
                    this.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimation];
                }
            }
        }
        else
        {           
            if (_move == true)
            {
                string currentAnimation = this.GetComponent<SpriteRenderer>().sprite.name;
                double numAnimation = Convert.ToDouble(currentAnimation.Split('_')[1]);
                numAnimation = Math.Floor(numAnimation / 9) * 9;
                int numAnimationInt = Convert.ToInt32(numAnimation);
               
                anim.SetBool("isWalking", false);
                anim.Play("Idle");
                NetworkManager.instance.GetComponent<NetworkManager>().StopAnimation(numAnimationInt);

                foreach (KeyValuePair<string, Sprite[]> entry in spritesArray)
                {
                    this.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimationInt];
                }
            }
            
            _move = false;
        }

        count++;
         ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        string currentAnimation = this.GetComponent<SpriteRenderer>().sprite.name;
        double numAnimation = Convert.ToDouble(currentAnimation.Split('_')[1]);
        numAnimation = Math.Floor(numAnimation / 9) * 9;
        int numAnimationInt = Convert.ToInt32(numAnimation);

        anim.SetBool("isWalking", false);
        anim.Play("Idle");
        NetworkManager.instance.GetComponent<NetworkManager>().StopAnimation(numAnimationInt); //Send command to server, so other players will know we stopped moving

        rbody.MovePosition(rbody.position - _speed * direction * Time.deltaTime);
        _move = false;

        foreach (KeyValuePair<string, Sprite[]> entry in spritesArray)
        {
            this.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimationInt];
        }
        //TODO NETWORING
    }
}