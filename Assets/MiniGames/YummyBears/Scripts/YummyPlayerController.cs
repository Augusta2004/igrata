using UnityEngine;
using System.Collections;
using System;

public class YummyPlayerController : MonoBehaviour
{
	private const int scorePerLevel = 25;

	public int lives;
	public int score;
	public int level;

	private Rigidbody2D rb;
	public YummyRandomColor rc;
	public GameObject botPrefab;

	public YummyGameAudio yummyAudio;
	public YummyGameUI ui;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    public void Start ()
	{
		score = 0;
		level = 1;
		lives = 3;

		rb = GetComponent<Rigidbody2D> ();

		UpdatePlayerColor ();
	    //this.gameObject.transform.localPosition = new Vector3(0, -2.98f, 0);
		//GetComponent<Transform> ().localScale = new Vector3 (0.25f, 0.25f, 0);

        yummyAudio.HigherBackgroundSound ();

		ui.DisplayScore (score, lives);
		ui.DisplayLevel(level);

		DestroyBots ();
	}

	void FixedUpdate()
	{
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += new Vector3(-0.15f, 0, 0);
        }


        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += new Vector3(0.15f, 0, 0);
        }

        /*
        float moveX = Input.GetAxis("Horizontal") * 0.1f;
		Vector3 v = new Vector3 (moveX, 0.0f, 0.0f);
		rb.AddForce (v, ForceMode2D.Impulse);
        */
	}

	void OnCollisionEnter2D(Collision2D c)
	{
		if (c.gameObject.CompareTag("Bot"))
		{
			float playerX = GetComponent<Transform>().localScale.x;
			float playerY = GetComponent<Transform>().localScale.y;
			float playerSize = playerX * playerY;
			Color playerColor = GetComponent<SpriteRenderer>().color;

			GameObject bot = c.gameObject;
			float botX = bot.GetComponent<Transform>().localScale.x;
			float botY = bot.GetComponent<Transform>().localScale.y;
			float botSize = botX * botY;
			Color botColor = bot.GetComponent<SpriteRenderer>().color;

			if(playerSize > botSize && playerColor != botColor)
			{
				score += bot.GetComponent<YummyBot>().points;

				ui.DisplayScore (score, lives);
                yummyAudio.PlayGameSound("kill");

				int oldLevel = level;
				level = score/scorePerLevel + 1;

				if(oldLevel < level)
				{
					UpdatePlayerColor();
					ui.DisplayLevel(level);
                    yummyAudio.PlayGameSound("level");
				}


			}
			else
			{
				lives--;
				ui.DisplayScore (score, lives);
                yummyAudio.PlayGameSound("over");

				if (lives == 0)
				{
					GameOver ();
				}
			}

			Destroy(bot);
		}
	}
	
	private void GameOver()
	{
        yummyAudio.LowerBackgroundSound ();
        yummyAudio.PlayGameSound("over");
		
		GetComponent<Transform>().localScale = new Vector3 (0, 0, 0);

        //game over
        int fish = Convert.ToInt32(score);
        NetworkManager.instance.GetComponent<NetworkManager>().AddFish(fish);

        StartCoroutine(GameOverUI());
	}

	private IEnumerator GameOverUI()
	{
		yield return new WaitForSeconds(0.5f);

		this.gameObject.SetActive(false);

		ui.HideLevelAndScore();
		ui.DisplayGameOver(score);
		ui.ShowCloseButton ();
	}

	private void DestroyBots ()
	{
		GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");

		foreach(GameObject bot in bots)
		{
			Destroy(bot);
		}
	}

	private void UpdatePlayerColor()
	{
		GetComponent<SpriteRenderer> ().color = rc.GetRandomColor ();
	}
}