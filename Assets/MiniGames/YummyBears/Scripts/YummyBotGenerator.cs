using UnityEngine;
using System.Collections;
using System;
using System.Timers;

public class YummyBotGenerator : MonoBehaviour {
	
	public GameObject botPrefab;
	public GameObject player;
	private System.Random r;
	public YummyRandomColor rc;

    private int level;
	
	void Start ()
	{
		r = new System.Random ();
        InvokeRepeating("GenerateBot", 0.6f, 0.6f);
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
        float botY = GetRandomY();

        float botGravityScale = 0.5f + 0.05f * (level - 1);

        GameObject bot = Instantiate(botPrefab, new Vector3(botX, botY, 0), Quaternion.identity);
        bot.GetComponent<Transform>().localScale = new Vector3(botScaleX, botScaleY, 0);
        bot.GetComponent<SpriteRenderer>().color = rc.GetRandomColor();
        bot.GetComponent<YummyBot>().points = points;
        bot.GetComponent<Rigidbody2D>().gravityScale = botGravityScale;

       // int onAngle = r.Next(1, 3);
        //if(onAngle == 1)
       // {
            int angle = r.Next(-3, 4);
            bot.GetComponent<YummyBot>().angle = 0.01f * angle;
       // }
    }
	
	private float GetRandomX()
	{
		float n = r.Next (-656, 644) / 100.0f;
		return n;
	}

    private float GetRandomY()
    {
        float n = r.Next(6, 12);
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
