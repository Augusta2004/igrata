using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ClickItem : MonoBehaviour {

    public string item_id;
    public bool item_on;
    public string type;
    public string path;

    private void OnMouseDown()
    {
        if(!item_on)
        {
            Debug.Log("tried to put item on");

            string type1 = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type.ToLower());
            string id = this.transform.parent.transform.parent.Find("Player Clothes").transform.Find(type1).GetComponent<ItemRemove>().id;
            Debug.Log(this.transform.parent.transform.Find("Item " + id));
            if (id != "" && id != null && this.transform.parent.transform.Find("Item " + id) != null)
            {
                this.transform.parent.transform.Find("Item " + id).GetComponent<ClickItem>().item_on = false;
            }
            item_on = true;
            
            NetworkManager.instance.GetComponent<NetworkManager>().ChangeItem(item_id, type);
        }        
    }
}
