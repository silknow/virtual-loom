using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class TranslationRequest: SilknowAPIRequest
{
    private Hashtable _table;
    private string _key;
    private string _attribute;
    private JSonSilknowAPIResponse _jSonSilknowApiResponse;
    public void parseResponse(string response)
    {
        try
        {
            _jSonSilknowApiResponse=JsonUtility.FromJson<JSonSilknowAPIResponse>(response);
            _table[_key] = _jSonSilknowApiResponse.prefLabel;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    } 

    public void getTranslation(Hashtable table,string key,string attribute,string URI)
    {
        string endpoint = PlayerPrefs.GetString("endpoint");
        string uri = server + "?id=" + UnityWebRequest.EscapeURL(URI) + "&endpoint"+UnityWebRequest.EscapeURL(endpoint)+"&lang=" + I18N.instance.gameLang.ToString().ToLower();
        _table = table;
        _key = key;
        _attribute = _attribute;

        StartCoroutine(getTranslation(URI));
    }

    IEnumerator getTranslation(string uri)
    {
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                response = "Error: " + webRequest.error;
            }
            else
            {
                response = webRequest.downloadHandler.text;
                response = response.Substring(1, response.Length - 3);
                parseResponse(response);
                response = "Received: " + response;
                try
                {
                    jsonResponse = JsonUtility.FromJson<JSonSilknowAPIResponse>(response);

                    _table[_key] = jsonResponse.prefLabel;
                    if (debugText != null)
                        debugText.text = jsonResponse.description;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        Destroy(this);

    }
}
