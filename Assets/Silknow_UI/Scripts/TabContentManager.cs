using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabContentManager : MonoBehaviour
{
   public int tab;

   private void OnEnable()
   {
      WizardController.instance.selectedTab = tab;
      if(tab ==0)
         FirstTabManager.instance.imgIsLoaded = false;
   }
}
