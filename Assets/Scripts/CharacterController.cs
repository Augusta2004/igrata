using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private bool _move = false;
    public bool _otherMove = false;
    public float _speed;
    private Vector2 _player;
    [HideInInspector]
    public Vector2 _otherPlayer;
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
    private int numAnimation;
    private int prevAnimation;

    private int countCollisions = 0;
    

    // Use this for initialization
    void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        anim.SetFloat("input_x", 0);
        anim.SetFloat("input_y", -1);

        Physics2D.IgnoreLayerCollision(10, 10);

        prevAnimation = -1;
    }

    // Update is called once per frame
    void Update()
    {
        string currentAnimation = this.GetComponent<SpriteRenderer>().sprite.name;
        numAnimation = Convert.ToInt32(currentAnimation.Split('_')[1]);

        if (numAnimation != prevAnimation)
        {
            foreach (KeyValuePair<string, Sprite[]> entry in spritesArray)
            {
                this.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimation];
            }

            prevAnimation = numAnimation;
        }

        position = gameObject.transform.position;

        if (!isLocalPlayer)
        {
            if(_otherMove)
            {
                CharacterMove(_otherPlayer);
            }
            return;
        }


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
                _player.y += 0.35f;
            }

            if (!_move)
            {
                direction = (_player - position).normalized;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(rbody.position + _speed * direction * Time.deltaTime * 5 + new Vector2(0, -0.24f));
                Ray ray2 = Camera.main.ScreenPointToRay(screenPos);
                RaycastHit2D[] hit2 = Physics2D.CircleCastAll(ray2.origin, 0.08f, ray2.direction, Mathf.Infinity);


                for (int i = 0; i < hit2.Length; i++)
                {
                    RaycastHit2D hit = hit2[i];
                    if(hit.collider.gameObject.layer == 11)
                    {
                        return;
                    } 
                }
            }

            NetworkManager.instance.GetComponent<NetworkManager>().CommandMove(transform.position, _player, numAnimation);
            _move = true;
        }

        CharacterMove(_player);

        count++;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (Input.GetKeyDown("w"))
        {
            //Wave();
            NetworkManager.instance.GetComponent<NetworkManager>().Movement("wave");
        }
        else if (Input.GetKeyDown("d"))
        {
            //Dance();
            NetworkManager.instance.GetComponent<NetworkManager>().Movement("dance");
        }
        else if (Input.GetKeyDown("s"))
        {
            //Sit();
            Vector2 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 sit_direction = (mouse_position - position).normalized;
            NetworkManager.instance.GetComponent<NetworkManager>().Movement("sit", sit_direction.x, sit_direction.y);
        }
    }

    public void Wave()
    {
        if (!_move || !isLocalPlayer)
        {
            anim.SetTrigger("wave");
            anim.SetFloat("input_x", 0);
            anim.SetFloat("input_y", -1);
        }

        //Debug.Log("Wave " + )
    }

    public void Dance()
    {
        if (!_move || !isLocalPlayer)
        {
            anim.SetTrigger("isDancing");
        }
    }

    public void Sit(float x = 10, float y = 10)
    {
        if (!_move || !isLocalPlayer)
        {
            if (x == 10 && y == 10)
            {
                Vector2 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 sit_direction = (mouse_position - position).normalized;

                anim.SetFloat("input_x", sit_direction.x);
                anim.SetFloat("input_y", sit_direction.y);
            }
            else
            {
                anim.SetFloat("input_x", x);
                anim.SetFloat("input_y", y);
            }

            anim.SetTrigger("sit");
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        countCollisions++;
        string currentAnimation2 = this.GetComponent<SpriteRenderer>().sprite.name;
        double numAnimation2 = Convert.ToDouble(currentAnimation2.Split('_')[1]);
        numAnimation2 = Math.Floor(numAnimation2 / 9) * 9;
        int numAnimationInt = Convert.ToInt32(numAnimation2);

        _player = rbody.position;
        anim.SetBool("isWalking", false);
        anim.Play("Idle");
        NetworkManager.instance.GetComponent<NetworkManager>().StopAnimation(rbody.position); //Send command to server, so other players will know we stopped moving
        if (countCollisions == 1)
        {
            rbody.MovePosition(rbody.position - _speed * direction * Time.deltaTime * 0.4f);
        }
        
        _move = false;
        _otherMove = false;

        foreach (KeyValuePair<string, Sprite[]> entry in spritesArray)
        {
            this.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimationInt];
        }
        //TODO NETWORKING                
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        countCollisions = 0;
    }

    void CharacterMove(Vector2 _player)
    {
        if (_player != Vector2.zero && (_player - position).magnitude >= .06)
        {
            if (_move == true || _otherMove)
            {
                direction = (_player - position).normalized;
                rbody.MovePosition(rbody.position + _speed * direction * Time.deltaTime);

                anim.SetBool("isWalking", true);
                anim.Play("Walking");
                anim.SetFloat("input_x", direction.x);
                anim.SetFloat("input_y", direction.y);

                //string currentAnimation = this.GetComponent<SpriteRenderer>().sprite.name;
                //numAnimation = Convert.ToInt32(currentAnimation.Split('_')[1]);

                //NetworkManager.instance.GetComponent<NetworkManager>().CommandMove(transform.position, direction, numAnimation);

                /*
                foreach (KeyValuePair<string, Sprite[]> entry in spritesArray)
                {
                    this.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimation];
                }
                */
            }
        }
        else
        {
            if (_move == true || _otherMove)
            {
                string currentAnimation2 = this.GetComponent<SpriteRenderer>().sprite.name;
                double numAnimation2 = Convert.ToDouble(currentAnimation2.Split('_')[1]);
                numAnimation2 = Math.Floor(numAnimation2 / 9) * 9;
                int numAnimationInt = Convert.ToInt32(numAnimation2);

                anim.SetBool("isWalking", false);
                anim.Play("Idle");
                NetworkManager.instance.GetComponent<NetworkManager>().StopAnimation(rbody.position);

                foreach (KeyValuePair<string, Sprite[]> entry in spritesArray)
                {
                    this.transform.Find("ItemHolder").transform.Find(entry.Key + "Sprite").GetComponent<SpriteRenderer>().sprite = entry.Value[numAnimationInt];
                }
            }

            _move = false;
            _otherMove = false;
        }
    }
}