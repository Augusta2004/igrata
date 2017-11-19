using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BunnyController : MonoBehaviour {

	private Rigidbody2D rb;
    private Animator myAnim;

    private float jumpForce = 3000f;
    private float bunnyHurtTime = -1;

    public Text scoreText;
    public Text livesText;

    private float startTime;

    private int jumpsLeft = 1;
    private int lives = 3;

    private bool isSpringJumping = false;
    private bool isJumping = false;


    void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
        myAnim = GetComponent<Animator>();
        startTime = Time.time;
    }
	
	 
	void Update ()
	{
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("BunnyRunTitle");
        }

        if (bunnyHurtTime == -1)
        {
            if ((Input.GetButtonUp("Jump") || Input.GetButtonUp("Fire1")) && jumpsLeft > 0)
            {
                if(rb.velocity.y < 0)
                {
                    rb.velocity = Vector2.zero;
                }

                if (!isSpringJumping)
                {
                    rb.AddForce(transform.up * jumpForce);
                }

                isJumping = true;

                jumpsLeft--;
            }

            myAnim.SetFloat("velocity", Mathf.Abs(rb.velocity.y));
            scoreText.text = (Time.time - startTime).ToString("0.0");
        }
        else
        {
            if(Time.time > bunnyHurtTime + 3)
            {
                NetworkManager.instance.GetComponent<NetworkManager>().ChangeRoom("BunnyRunTitle");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(collision.gameObject);
            Debug.Log(this.transform.position.y);
            if (collision.gameObject.tag == "Block")
            {
                lives--;

                livesText.text = "Lives: " + lives;

                

                if (lives == 0)
                {
                    myAnim.SetBool("bunnyHurt", true);

                    foreach (PrefabSpawner spawner in FindObjectsOfType<PrefabSpawner>())
                    {
                        spawner.enabled = false;
                    }

                    foreach (MoveLeft moveLeft in FindObjectsOfType<MoveLeft>())
                    {
                        moveLeft.enabled = false;
                    }

                    bunnyHurtTime = Time.time;

                    double fish = Math.Ceiling(Convert.ToDouble(scoreText.text));
                    NetworkManager.instance.GetComponent<NetworkManager>().AddFish(Convert.ToInt32(fish));
                }
            }
            else if(collision.gameObject.tag == "Heart")
            {
                lives++;
                livesText.text = "Lives: " + lives;
            }
            else
            {
                if(!isJumping)
                {
                    rb.AddForce(transform.up * jumpForce * 1.25f);
                }
                
                isSpringJumping = true;
            }
        }

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            jumpsLeft = 1;

            if (isSpringJumping)
            {
                isSpringJumping = false;
            }

            if(isJumping)
            {
                isJumping = false;
            }
        }
    }
}
