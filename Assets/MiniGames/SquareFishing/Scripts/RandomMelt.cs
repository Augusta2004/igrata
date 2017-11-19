using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMelt : MonoBehaviour {
	
	void Start ()
    {
        InvokeRepeating("MeltRandomBlock", 2.0f, 5.0f);
    }

    private void MeltRandomBlock()
    {
        Debug.Log("melt");
        System.Random rnd = new System.Random();
        int block = rnd.Next(1, 10);
        Debug.Log(block);
        GameObject blockObj = GameObject.Find("Blocks").transform.Find("Block" + block).gameObject;

        while (blockObj.GetComponent<BlockController>().isMelting)
        {
            block = rnd.Next(1, 10);
            blockObj = GameObject.Find("Blocks").transform.Find("Block" + block).gameObject;

            if(!blockObj.GetComponent<BlockController>().isMelting)
            {
                break;
            }
        }

        blockObj.GetComponent<BlockController>().isRandomMelting = true;

        
    }
}