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
        if(!item_on)
        {
            string type1 = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type.ToLower());
            int id = this.transform.parent.transform.parent.Find("Player Clothes").transform.Find(type1).GetComponent<ItemRemove>().id;

            if (id != 0)
            {
                this.transform.parent.transform.Find("Item " + id).GetComponent<ClickItem>().item_on = false;
            }
            item_on = true;

            NetworkManager.instance.GetComponent<NetworkManager>().ChangeItem(item_id, type);
            //item_on = true;
            /*string type1 = new CultureInfo("en-US").TextInfo.ToTitleCase(type);
            Debug.Log(type1);
            GameObject item = GameObject.Find("PlayerCard")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("Player Clothes")
                .transform.Find(type1).gameObject;

                item.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + path + "PC");

            item.AddComponent<PolygonCollider2D>().isTrigger = true;
            item.GetComponent<ItemRemove>().id = item_id;
            */
        }        
    }
}
