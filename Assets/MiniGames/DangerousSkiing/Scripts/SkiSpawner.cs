using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiSpawner : MonoBehaviour {

    private float nextSpawn = 0;
    private float randomDelay = 2;
    public GameObject prefab;

    void Update ()
    {
        if (Time.time > nextSpawn)
        {
            System.Random rnd = new System.Random();
            int coridor = rnd.Next(1, 4);

            int yRandMax = 5;
            if(GameObject.Find("Bear").GetComponent<BearSkiController>().level < 5)
            {
                yRandMax = 2;
            }
            else if(GameObject.Find("Bear").GetComponent<BearSkiController>().level < 8)
            {
                yRandMax = 4;
            }

            int yRand = rnd.Next(1, yRandMax);

            float x = 0;
            float y = 0;

            switch(yRand)
            {
                case 1:
                    y = 5.9f;
                    break;
                case 2:
                    y = 8.3f;
                    break;
                case 3:
                    y = 10.7f;
                    break;
                case 4:
                    y = 13.1f;
                    break;

                default: break;
            }

            switch(coridor)
            {
                case 1:
                    x = -2.7f;
                break;

                case 2:
                    x = 0.06f;
                    break;

                case 3:
                    x = 2.88f;
                    break;

                default: break;

            }

            GameObject obj = Instantiate(prefab, new Vector2(x, y), Quaternion.identity);

            int type = rnd.Next(1, 5);
            Sprite sprite = null;

            if(type == 2 || type == 3)
            {
                type = rnd.Next(1, 5);
            }

            string objName = "";

            switch (type)
            {
                case 1:
                    sprite = Resources.Load<Sprite>("Sprites/hole");
                    objName = "Hole";
                    break;

                case 2:
                    sprite = Resources.Load<Sprite>("Sprites/arrow");
                    objName = "Arrow";
                    break;

                case 3:
                    sprite = Resources.Load<Sprite>("Sprites/shield");
                    objName = "Shield";
                    break;

                case 4:
                    sprite = Resources.Load<Sprite>("Sprites/fish");
                    objName = "Fish";
                    break;

                default: break;

            }

            obj.GetComponent<SpriteRenderer>().sprite = sprite;
            obj.AddComponent<PolygonCollider2D>();
            obj.name = objName;


            int level = GameObject.Find("Bear").GetComponent<BearSkiController>().level;
            nextSpawn = Time.time + Random.Range(1, randomDelay - (level / 3));
        }
    }
}
