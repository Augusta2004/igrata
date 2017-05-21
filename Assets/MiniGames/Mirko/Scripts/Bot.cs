using UnityEngine;
using System.Collections;

public class Bot : MonoBehaviour
{
    public BotGenerator botGenerator = new BotGenerator();
    public int points = 0;

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Bot"))
        {
            GameObject bot = c.gameObject;
            Destroy(bot);

            botGenerator.GenerateBot();
			botGenerator.GenerateBot();
        }
    }
}
