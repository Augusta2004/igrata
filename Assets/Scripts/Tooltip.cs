using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    private GameObject tooltip;
    public string text;
    private bool isClicked;

    private void Start()
    {
        tooltip = GameObject.Find("Tooltip").transform.Find("TooltipHolder").gameObject;
    }

    private void OnMouseOver()
    {
        if(!isClicked)
        {
            float add = 0;

            float height = this.GetComponent<SpriteRenderer>().bounds.max.y;

            if (this.transform.position.y <= 3.6f)
            {
                add = height + 0.25f;
            }
            else
            {
                height = this.GetComponent<SpriteRenderer>().bounds.min.y;
                add = height - 0.25f;
            }

            tooltip.SetActive(true);
            tooltip.transform.Find("TooltipCanvas").transform.Find("TooltipText").GetComponent<Text>().text = text;
            tooltip.transform.position = new Vector3(this.transform.position.x, add, this.transform.position.z);
        }
    }

    private void OnMouseExit()
    {
        isClicked = false;
        tooltip.SetActive(false);
    }

    private void OnMouseDown()
    {
        isClicked = true;
        tooltip.SetActive(false);
    }
}
