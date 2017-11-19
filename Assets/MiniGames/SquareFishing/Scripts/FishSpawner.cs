using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour {

	void Start ()
    {
        InvokeRepeating("SpawnFish", 1.0f, 3.5f);
    }
	
    void SpawnFish()
    {
        System.Random rnd = new System.Random();
        int fish = rnd.Next(1, 10);

        GameObject blockObj = GameObject.Find("Blocks").transform.Find("Block" + fish).gameObject;
        int bearBlock = GameObject.Find("Bear").gameObject.GetComponent<BearController>().blockNum;

        while (blockObj.GetComponent<BlockController>().isMelting || blockObj.GetComponent<BlockController>().isMelted || bearBlock == fish)
        {
            fish = rnd.Next(1, 10);
            blockObj = GameObject.Find("Blocks").transform.Find("Block" + fish).gameObject;

            if (!blockObj.GetComponent<BlockController>().isMelting && !blockObj.GetComponent<BlockController>().isMelted && bearBlock != fish)
            {
                break;
            }
        }


        GameObject.Find("FishContainer").transform.Find("Fish" + fish).gameObject.SetActive(true);
        GameObject.Find("FishContainer").transform.Find("Fish" + fish).GetComponent<Fish>().isHiding = true;

        rnd = new System.Random();
        int enemyRand = rnd.Next(1, 10);
        
        if(enemyRand > 7)
        {
            GameObject.Find("FishContainer").transform.Find("Fish" + fish).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/enemy");
            GameObject.Find("FishContainer").transform.Find("Fish" + fish).GetComponent<Fish>().isEnemy = true;
        }
        else
        {
            rnd = new System.Random();
            int goldenRand = rnd.Next(1, 10);

            if(goldenRand > 7)
            {
                GameObject.Find("FishContainer").transform.Find("Fish" + fish).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/golden");
                GameObject.Find("FishContainer").transform.Find("Fish" + fish).GetComponent<Fish>().isGolden = true;
            }
        }
    }
}
