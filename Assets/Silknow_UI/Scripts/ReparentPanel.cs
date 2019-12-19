using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReparentPanel : MonoBehaviour
{

    public RectTransform backgroundZone;
    public RectTransform pictoricalZone;

    public void OnChangeParent(bool value)
    {
        
        GetComponent<RectTransform>().parent = value ? backgroundZone: pictoricalZone;
    }
}
