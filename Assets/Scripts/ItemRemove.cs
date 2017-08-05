using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRemove : MonoBehaviour {

    public string id;
    public string type;

	// Use this for initialization
	void OnMouseDown()
    {
        this.GetComponent<SpriteRenderer>().sprite = null;
        Destroy(this.GetComponent<PolygonCollider2D>());
        
        if (GameObject.Find("PlayerCard")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("ItemsHolder")
                .transform.Find("Item " + id) != null)
        {
            GameObject.Find("PlayerCard")
                .transform.Find("DragPlayerCard")
                .transform.Find("PlayerCardHolder")
                .transform.Find("ItemsHolder")
                .transform.Find("Item " + id).GetComponent<ClickItem>().item_on = false;
        }
        
        

        NetworkManager.instance.GetComponent<NetworkManager>().RemoveItem(id, type);

        id = null;
    }
	
}
