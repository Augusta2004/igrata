using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BearSkiController : MonoBehaviour {

    private int coridor = 2;
    public int score = 0;
    public int lives = 3;
    public int level = 1;
    private bool shield = false;
    public bool moveForward = false;
    private int countMoveForward = 0;

    void Update ()
    {
		if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if(moveForward)
        {
            if(countMoveForward < 90)
            {
                countMoveForward++;
            }
            else
            {
                countMoveForward = 0;
                moveForward = false;
            }
        }
    }

    private void MoveLeft()
    {
        if(coridor > 1 && lives > 0)
        {
            coridor--;

            float x = this.transform.position.x;
            float y = this.transform.position.y;

            x -= 2.7f;

            this.transform.position = new Vector2(x, y);
        }
    }


    private void MoveRight()
    {
        if (coridor < 3 && lives > 0)
        {
            coridor++;

            float x = this.transform.position.x;
            float y = this.transform.position.y;

            x += 2.7f;

            this.transform.position = new Vector2(x, y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.name)
        {
            case "Fish":

                if (lives > 0)
                {
                    score++;
                }

                GameObject.Find("Canvas").transform.Find("Score").GetComponent<Text>().text = "Score: " + score;

                level = (int)Mathf.Floor(score / 6) + 1;
                GameObject.Find("Canvas").transform.Find("Level").GetComponent<Text>().text = "Level: " + level;

                break;

            case "Hole":
                if(!shield)
                {
                    if (lives > 0)
                    {
                        lives--;

                        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("SkiObject");

                        for (var i = 0; i < gameObjects.Length; i++)
                        {
                            Destroy(gameObjects[i]);
                        }
                    }
                    GameObject.Find("Canvas").transform.Find("Lives").GetComponent<Text>().text = "Lives: " + lives;
                    
                }
                else
                {
                    shield = false;
                    this.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
                }
                
             break;

            case "Arrow":
                if (!moveForward)
                {
                    moveForward = true;
                }
                break;

            case "Shield":
                shield = true;
                this.GetComponent<SpriteRenderer>().color = new Color32(30, 200, 30, 255);
                break;

            default: break;
        }
        
        
        if(lives == 0)
        {
            GameObject.Find("GameOver").transform.Find("GameOverContainer").gameObject.SetActive(true);
            GameObject.Find("GameOver").transform.Find("GameOverContainer").transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = "Game Over \nScore: " + score;
            NetworkManager.instance.GetComponent<NetworkManager>().AddFish((score));
        }

        Destroy(collision.gameObject);
    }

}
