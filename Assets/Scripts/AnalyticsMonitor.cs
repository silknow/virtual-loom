using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Configuration;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AnalyticsMonitor : MonoBehaviour
{
    public string server;
    public int mouseMovement;

    public int leftClicks;

    public int rightClicks;
    
    public int middleClicks;

    public GeneralTechnique technique
    {
        get => WizardController.instance._generalTechnique;
    }
    private Vector3 _lastMousePosition;
    void Start()
    {
        Analytics.enabled = true;
        _lastMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) leftClicks++;
        if (Input.GetMouseButtonUp(1)) rightClicks++;
        if (Input.GetMouseButtonUp(2)) middleClicks++;

        Vector3 incremento = Input.mousePosition - _lastMousePosition;
        mouseMovement += (int)(Mathf.Abs(incremento[0])+Mathf.Abs(incremento[1]));
        _lastMousePosition = Input.mousePosition;
    }

    public void clear()
    {
        leftClicks = rightClicks = mouseMovement = middleClicks = 0;
    }

    public void sendEvent(string name, Dictionary<string, object> dictionary)
    {
        if (!enabled) 
            return;
        AnalyticsEvent.Custom(name, dictionary);
        Dictionary<string,string> dict2 = new Dictionary<string, string>();
        dictionary["sessionId"] = AnalyticsSessionInfo.sessionId;
        dictionary["platform"] = Application.platform.ToString();
        dictionary["event"]=name;
        foreach (var entry in dictionary)
        {
            dict2[entry.Key] = entry.Value.ToString();
        }

        StartCoroutine(UploadToServer(dict2));
    }
    IEnumerator UploadToServer( Dictionary<string,string> eventDictionary)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(server, eventDictionary))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}
