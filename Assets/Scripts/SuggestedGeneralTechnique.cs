using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuggestedGeneralTechnique : MonoBehaviour
{
    public Text label;
    public Text value;
    // Start is called before the first frame update
    void Start()
    {
        if (jsonReader.instance.config.technique == "")
        {
            GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            label.gameObject.SetActive(false);
            value.gameObject.SetActive(false);
        }
        else
        {
            value.text = jsonReader.instance.config.technique;
        }
    }

   
}
