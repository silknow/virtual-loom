using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuggestedGroundColor : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        if (!jsonReader.instance.config.backgroundColorReceived) 
            return;
        
        YarnPanel[] yarnpanels = GetComponentsInChildren<YarnPanel>();
        int index=0;
        float colordistance = Vector3.Distance(
            new Vector3(jsonReader.instance.config.backgroundColor.r, jsonReader.instance.config.backgroundColor.g,
                jsonReader.instance.config.backgroundColor.b),
            new Vector3(yarnpanels[index].inputColor.r, yarnpanels[index].inputColor.g, yarnpanels[index].inputColor.b));
        for (int i = 1; i < yarnpanels.Length; i++)
        {
            float candidateDistance = Vector3.Distance(
                new Vector3(jsonReader.instance.config.backgroundColor.r, jsonReader.instance.config.backgroundColor.g,
                    jsonReader.instance.config.backgroundColor.b),
                new Vector3(yarnpanels[i].inputColor.r, yarnpanels[i].inputColor.g, yarnpanels[i].inputColor.b));
            if (colordistance > candidateDistance)
                index = i;
        }
        yarnpanels[index].backgroundToggle.isOn=true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
