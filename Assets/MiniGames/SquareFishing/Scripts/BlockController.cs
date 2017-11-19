using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {

    public bool isStepped = false;
    public bool isMelting = false;
    public bool isMelted = false;
    public bool isRandomMelting = false;
    private bool recovering = false;

    public int blockNum;

    private int countFrames = 0;
    private int countFramesMelting = 0;
    private int countFramesMelted = 0;

    private void Update()
    {
        if((isStepped && !isMelting) || isRandomMelting) //Start melting
        {
            countFrames++;

            if (countFrames > 60 && isMelted == false)
            {
                isMelting = true;
                isRandomMelting = false;
                Debug.Log(this.gameObject.name + " started melting!");
                this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ice-block-melting");
            }
        }
        else
        {
            countFrames = 0;

            if(isMelting)
            {
                countFramesMelting++;

                if(countFramesMelting > 180)
                {
                    if(isStepped)
                    {
                        Debug.Log("Die!");
                        GameObject.Find("Bear").GetComponent<BearController>().isDead = true;
                        isStepped = false;
                        isMelted = false;
                    }

                    this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ice-block-melted");
                    GameObject.Find("FishContainer").transform.Find("Fish" + blockNum).gameObject.SetActive(false);
                    isMelted = true;
                    isMelting = false;
                    
                    countFramesMelting = 0;
                }
            }
            else
            {
                countFramesMelting = 0;
            }
        }

        if(isMelted || recovering)
        {
            countFramesMelted++;

            if(countFramesMelted > 120)
            {
                isMelted = false;
                recovering = true;
                this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ice-block-melting");

                if (countFramesMelted > 300)
                {
                    this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/ice-block");
                    recovering = false;

                    countFramesMelted = 0;
                }
            }
        }
        else
        {
            countFramesMelted = 0;
        }
    }
}