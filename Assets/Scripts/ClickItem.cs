using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ClickItem : MonoBehaviour {

    public int item_id;
    public bool item_on;
    public string type;
    public string path;

    private void OnMouseDown()
    {
        Debug.Log("gg");
        if(!item_on)
        {
            NetworkManager.instance.GetComponent<NetworkManager>().ChangeItem(item_id, type);

            string type1 = new CultureInfo("en-US").TextInfo.ToTitleCase(type);
            Debug.Log(type1);
            GameObject.Find("PlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("Player Clothes")
                .transform.Find(type1)
                .GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + path + "PC");
        }
    }
}
