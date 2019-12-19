using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckForPosterizedImage : MonoBehaviour
{
    private Button button;
    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        button.interactable = WizardController.instance.posterizeResult != null;
    }
}
