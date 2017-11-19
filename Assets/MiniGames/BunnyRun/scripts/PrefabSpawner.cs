using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{

    private float nextSpawn = 0;
    public Transform prefabToSpawn;
    public float spawnRate = 1;
    public float randomDelay = 2;
    private int prevType = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            Transform obj = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            
            System.Random rnd = new System.Random();
            int type = rnd.Next(1, 5);

            if (type > 2)
            {
                type = rnd.Next(1, 5);
                if (type == 4)
                {
                    type = rnd.Next(1, 5);
                    if(type == 4)
                    {
                        type = rnd.Next(1, 5);
                    }
                }
            }

            if (prevType == 1)
            {
                type = rnd.Next(2, 5);
            }


            switch (type)
            {
                case 1:
                    obj.tag = "Block";
                    break;

                case 2:
                    obj.tag = "Block";
                    obj.transform.position += new Vector3(0, 2.5f, 0);
                    break;

                case 3:
                    obj.tag = "Spring";
                    obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/spring");
                    break;

                case 4:
                    obj.tag = "Heart";
                    obj.transform.position += new Vector3(0, 2.5f, 0);
                    obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/heart");
                    break;

                default:
                    break;
            }

            type = rnd.Next(1, 10);
            if (type == 1)
            {
                nextSpawn = Time.time + 0.5f;
                prevType = type;
            }
            else
            {
                nextSpawn = Time.time + spawnRate + Random.Range(0, randomDelay);
                prevType = 0;
            }
        }
    }
}
