using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuggestedWeavingTechnique : MonoBehaviour
{
    public Text label;
    public Text value;
    // Start is called before the first frame update
    void Start()
    {
        if (jsonReader.instance.config.weaving.Count == 0)
        {
            GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            label.gameObject.SetActive(false);
            value.gameObject.SetActive(false);
        }
        else
        {
            foreach (var weaving in jsonReader.instance.config.weaving)
            {
                value.text = weaving + ", ";
            }
            value.text = value.text.Substring(0, value.text.Length - 2);
        }
    }

   
}
