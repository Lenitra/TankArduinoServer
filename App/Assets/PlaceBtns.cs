using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBtns : MonoBehaviour
{
    public GameObject childPrefab;
    private GameObject Canvas;
    void Start()
    {
        Canvas = gameObject;
        // instaciate 5 children
        for (int i = 0; i < 10; i++)
        {
            GameObject child = Instantiate(childPrefab, Canvas.transform);
            child.name = "Panel " + i;
        }


        // get the x and y size of the canvas
        float x = Canvas.GetComponent<RectTransform>().rect.width;
        float y = Canvas.GetComponent<RectTransform>().rect.height;
        // for each child of the canvas
        int count = 0;
        foreach (Transform child in Canvas.transform)
        {
            count++;
            // set the size of the child to 1/5 of the canvas in x and 1/2 of the canvas in y
            // child.GetComponent<RectTransform>().sizeDelta = new Vector2(x / 5, y / 2.1f);
            // set the position of the first child to be -2x
            // set the position of the second child to be -x
            // set the position of the third child to be 0
            child.GetComponent<RectTransform>().localPosition = new Vector3((count - 3) * x / 5, y / 4, 0);
            if (count > 5)
            {
                child.GetComponent<RectTransform>().localPosition = new Vector3((count - 8) * x / 5, -y / 4, 0);
            }
        }
    }

}
