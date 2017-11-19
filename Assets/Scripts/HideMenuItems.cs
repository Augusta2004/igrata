using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMenuItems : MonoBehaviour {

    public GameObject emojis;
    public GameObject moves;
    public GameObject friends;

    public GameObject emojiButton;
    public GameObject movesButton;
    public GameObject friendsButton;

    private void Update()
    {
        if (Input.GetMouseButton(0) && (emojis.activeSelf || moves.activeSelf || friends.activeSelf))
        {
            if(emojis.activeSelf &&
             !RectTransformUtility.RectangleContainsScreenPoint(
                 emojis.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 Camera.main) && !RectTransformUtility.RectangleContainsScreenPoint(
                 emojiButton.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 Camera.main))
            {
                emojis.SetActive(false);
            }

            if (moves.activeSelf &&
             !RectTransformUtility.RectangleContainsScreenPoint(
                 moves.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 Camera.main) && !RectTransformUtility.RectangleContainsScreenPoint(
                 movesButton.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 Camera.main))
            {
                moves.SetActive(false);
            }

            /*
            Debug.Log(friends.activeSelf);
            if (friends.activeSelf &&
             !RectTransformUtility.RectangleContainsScreenPoint(
                 friends.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 Camera.main)
                 && !RectTransformUtility.RectangleContainsScreenPoint(
                 friendsButton.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 Camera.main))
            {
                Debug.Log("Set false" + friends.activeSelf);
                friends.SetActive(false);
            }
            */
        }
    }
}
