using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepLinking : MonoBehaviour
{
    void Start()
    {
        ImaginationOverflow.UniversalDeepLinking.DeepLinkManager.Instance.LinkActivated += Instance_LinkActivated;
    }

    private void Instance_LinkActivated(ImaginationOverflow.UniversalDeepLinking.LinkActivation s)
    {
        //
        //  my activation code
        //
        Debug.Log("Link received: "+s.QueryString.Values.ToString());
    }
}
