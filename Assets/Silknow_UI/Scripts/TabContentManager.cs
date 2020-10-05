using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class TabContentManager : MonoBehaviour
{
   public int tab;
   private float enabledTime;
   public float duration = 0;
   public AnalyticsMonitor am;
   private void OnEnable()
   {
      enabledTime = Time.time;
      WizardController.instance.selectedTab = tab;
      if(tab ==0)
         FirstTabManager.instance.imgIsLoaded = false;
      if (tab != 4)
      {
         WizardController.instance.SetVisualizationState(false);
      }

      if (am != null)
         am.clear();
   }

   private void OnDisable()
   {
      duration = Time.time - enabledTime;
      if(am == null)
         return;
      switch (tab)
      {
         case  0:
            Analytics.CustomEvent("LoadImage_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks}
            });
         break;
         case  1:
            Analytics.CustomEvent("Homography_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks}
            });
            break;
         case  2:
            Analytics.CustomEvent("Posterize_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks},
               {"technique", am.technique}
            });
            break;
         case  3:
            Analytics.CustomEvent("Yarns_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks}
            });
            break;
         case  4:
            Analytics.CustomEvent("Visualization_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks}
            });
            break;
         
      }
   }
}
