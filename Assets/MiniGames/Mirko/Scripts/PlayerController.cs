using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
	private const int scorePerLevel = 25;

	public int lives;
	public int score;
	public int level;

	private Rigidbody2D rb;
	private RandomColor rc;
	public GameObject botPrefab;

	public GameAudio audio;
	public GameUI ui;

	
	public void Start ()
	{
		score = 0;
		level = 1;
		lives = 3;

		rb = GetComponent<Rigidbody2D> ();
		rc = new RandomColor ();

		UpdatePlayerColor ();
		this.gameObject.transform.localPosition = new Vector3(0, 0, 0);
		GetComponent<Transform> ().localScale = new Vector3 (0.6f, 0.6f, 0);

		audio.HigherBackgroundSound ();

		ui.DisplayScore (score, lives);
		ui.DisplayLevel(level);

		DestroyBots ();
	}

	void FixedUpdate()
	{
		float moveX = Input.GetAxis ("Horizontal") * 20;

		if (Input.touchCount > 0)
		{
			moveX = Input.touches [0].deltaPosition.x * 10;
		}

		Vector3 v = new Vector3 (moveX, 0.0f, 0.0f);
		rb.AddForce (v);
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
				score += bot.GetComponent<Bot>().points;

				ui.DisplayScore (score, lives);
				audio.PlayGameSound("kill");

				int oldLevel = level;
				level = score/scorePerLevel + 1;

				if(oldLevel < level)
				{
					UpdatePlayerColor();
					ui.DisplayLevel(level);
					audio.PlayGameSound("level");
				}


			}
			else
			{
				lives--;
				ui.DisplayScore (score, lives);
				audio.PlayGameSound("over");

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
		audio.LowerBackgroundSound ();
		audio.PlayGameSound("over");
		
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
		ui.ShowPlayAgainButton ();
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
