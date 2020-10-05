using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class WeaveDetail : MonoBehaviour
{
    public int pixelSize = 35;
    public Image image;
    public UIGridRenderer grid;
    

    public void set(Sprite sprite)
    {
        image.sprite = sprite;
        updateSizes();
    }

    private void updateSizes()
    {
        RectTransform rt = image.GetComponent<RectTransform>();
        var mainTexture = image.mainTexture;
        rt.sizeDelta = pixelSize * new Vector2(mainTexture.width, mainTexture.height);
        rt = grid.GetComponent<RectTransform>().GetComponent<RectTransform>();
        
        rt.sizeDelta = pixelSize * new Vector2(mainTexture.width, mainTexture.height);
        grid.GridColumns = mainTexture.width;
        grid.GridRows = mainTexture.height;
    }
}
