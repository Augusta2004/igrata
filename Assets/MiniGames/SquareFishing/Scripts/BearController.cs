using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BearController : MonoBehaviour {

    public int blockNum = 5; //1-top left; 9 - bottom right
    private int oldBlock;
    public bool isDead = false;
    private int lives = 3;
    private int fish = 0;
    private int deadCounter = 0;

	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
        }

        if(isDead)
        {
            deadCounter++;

            this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/deadBear");

            if (deadCounter > 90)
            {
                this.transform.position = new Vector2(-0.49f, 2.51f);
                lives--;
                isDead = false;
                GameObject.Find("Canvas").transform.Find("Lives").GetComponent<Text>().text = "Lives: " + lives;
                blockNum = 5;
                deadCounter = 0;
                this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/bear");

                for(int i = 1; i <= 9; i++)
                {
                    BlockController blockController = GameObject.Find("Blocks").transform.Find("Block" + i).GetComponent<BlockController>();
                    blockController.isStepped = false;
                    blockController.isMelting = false;
                    blockController.isRandomMelting = false;
                    blockController.isMelted = false;

                    GameObject.Find("Blocks").transform.Find("Block" + i).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ice-block");
                    GameObject fish = GameObject.Find("FishContainer").transform.Find("Fish" + i).gameObject;
                    fish.SetActive(false);
                    fish.GetComponent<Fish>().isGolden = false;
                    fish.GetComponent<Fish>().isEnemy = false;
                    fish.GetComponent<Fish>().isHiding = false;
                }

                if(lives == 0)
                {
                    GameObject.Find("GameOver").transform.Find("GameOverContainer").gameObject.SetActive(true);
                    GameObject.Find("GameOver").transform.Find("GameOverContainer").transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = "Game Over \nScore: " + fish;
                    NetworkManager.instance.GetComponent<NetworkManager>().AddFish((fish));
                }
            }
        }
    }

    void MoveRight()
    {
        if (blockNum % 3 != 0)
        {
            oldBlock = blockNum;
            blockNum++;
            MoveToBlock(2.3f, 0);
        }
    }

    void MoveLeft()
    {
        if (blockNum % 3 != 1)
        {
            oldBlock = blockNum;
            blockNum--;
            MoveToBlock(-2.3f, 0);
        }
    }

    void MoveDown()
    {
        if (blockNum < 7)
        {
            oldBlock = blockNum;
            blockNum += 3;
            MoveToBlock(-0.4f, -0.92f);
        }
    }

    void MoveUp()
    {
        if (blockNum > 3)
        {
            oldBlock = blockNum;
            blockNum -= 3;
            MoveToBlock(0.4f, 0.92f);
        }
    }

    void MoveToBlock(float moveX, float moveY)
    {
        if(!isDead && lives > 0)
        {
            float x = this.transform.position.x;
            float y = this.transform.position.y;

            x += moveX;
            y += moveY;

            this.transform.position = new Vector2(x, y);

            GameObject.Find("Blocks").transform.Find("Block" + blockNum).GetComponent<BlockController>().isStepped = true;
            GameObject.Find("Blocks").transform.Find("Block" + oldBlock).GetComponent<BlockController>().isStepped = false;

            if (GameObject.Find("Blocks").transform.Find("Block" + blockNum).GetComponent<BlockController>().isMelted)
            {
                isDead = true;
            }

            if(GameObject.Find("FishContainer").transform.Find("Fish" + blockNum).gameObject.activeSelf)
            {
                if(GameObject.Find("FishContainer").transform.Find("Fish" + blockNum).GetComponent<Fish>().isEnemy)
                {
                    isDead = true;
                }
                else
                {
                    if (!GameObject.Find("FishContainer").transform.Find("Fish" + blockNum).GetComponent<Fish>().isGolden)
                    {
                        fish += 2;
                    }
                    else
                    {
                        fish += 15;
                    }

                    GameObject.Find("Canvas").transform.Find("Score").GetComponent<Text>().text = "Score: " + fish;
                    GameObject.Find("FishContainer").transform.Find("Fish" + blockNum).gameObject.SetActive(false);
                }
            }
        }
    }
}