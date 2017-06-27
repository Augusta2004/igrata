using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BunnyController : MonoBehaviour {

	private Rigidbody2D rb;
    private Animator myAnim;

    public float jumpForce = 5000f;
    private float bunnyHurtTime = -1;

    public Text scoreText;

    private float startTime;

    private int jumpsLeft = 2;


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

                rb.AddForce(transform.up * jumpForce);
                jumpsLeft--;
            }

            myAnim.SetFloat("velocity", Mathf.Abs(rb.velocity.y));
            scoreText.text = (Time.time - startTime).ToString("0.0");
        }
        else
        {
            if(Time.time > bunnyHurtTime + 2)
            {
                SceneManager.LoadScene("BunnyRunTitle");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            foreach (PrefabSpawner spawner in FindObjectsOfType<PrefabSpawner>())
            {
                spawner.enabled = false;
            }

            foreach (MoveLeft moveLeft in FindObjectsOfType<MoveLeft>())
            {
                moveLeft.enabled = false;
            }

            bunnyHurtTime = Time.time;
            myAnim.SetBool("bunnyHurt", true);
            Debug.Log("Game Over");
            //game over
            double fish = Math.Ceiling(Convert.ToDouble(scoreText.text));
            NetworkManager.instance.GetComponent<NetworkManager>().AddFish(Convert.ToInt32(fish));

        }

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            jumpsLeft = 2;
        }
    }
}
