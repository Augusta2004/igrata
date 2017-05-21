using UnityEngine;
using System.Collections;
using System;
using System.Timers;

public class BotGenerator : MonoBehaviour {
	
	public GameObject botPrefab;
	public GameObject player;
	private System.Random r;
	private RandomColor rc;

    private int level;
	
	void Start ()
	{
		r = new System.Random ();
		rc = new RandomColor ();

        StartCoroutine(GenerateBots());
	}
	
	private IEnumerator GenerateBots()
	{
		while (true)
		{
            level = player.GetComponent<PlayerController>().level;
            float speed = 1.3f - (level - 1) * 0.15f;
            if (speed < 0.1f) speed = 0.1f;
            yield return new WaitForSeconds(speed);

            GenerateBot();
        }
	}

    public void GenerateBot()
    {
        int size = GetRandomSize();
        float coef = GetSizeCoef(size);
        int points = GetPoints(size);

        float botScaleX = botPrefab.GetComponent<Transform>().localScale.x * coef;
        float botScaleY = botPrefab.GetComponent<Transform>().localScale.y * coef;

        float botX = GetRandomX();
        Debug.Log(botX);
        float botY = 20;

        float botGravityScale = 0.5f + 0.05f * (level - 1);

        GameObject bot = (GameObject)Instantiate(botPrefab, new Vector3(botX, botY, 0), Quaternion.identity);
        bot.GetComponent<Transform>().localScale = new Vector3(botScaleX, botScaleY, 0);
        bot.GetComponent<SpriteRenderer>().color = rc.GetRandomColor();
        bot.GetComponent<Bot>().points = points;
        bot.GetComponent<Rigidbody2D>().gravityScale = botGravityScale;
    }
	
	private float GetRandomX()
	{
		float n = r.Next (-176, 180) / 10.0f;
		return n;
	}
	
	private int GetRandomSize()
	{
		int n = r.Next (0, 5);
		return n;
	}
	
	private int GetPoints(int n)
	{
		switch (n)
		{
		    case 0: return 1;
		    case 1: return 2;
		    default: return 5;
		}
	}
	
	private float GetSizeCoef(int n)
	{
		switch (n)
		{
		    case 0: return 0.5f;
		    case 1: return 0.625f;
		    case 2: return 0.75f;
		    case 3: return 1.30f;
		    default: return 1.52f;
		}
	}
}
