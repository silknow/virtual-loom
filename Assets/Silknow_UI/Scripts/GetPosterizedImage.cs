using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetPosterizedImage : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<RawImage>().texture = WizardController.instance.posterizeResult;
    }
 
}
