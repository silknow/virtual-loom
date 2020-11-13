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
            am.sendEvent("LoadImage_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks}
            });
         break;
         case  1:
            am.sendEvent("Homography_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks}
            });
            break;
         case  2:
            am.sendEvent("Posterize_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks},
               {"technique", am.technique}
            });
            break;
         case  3:
            am.sendEvent("Yarns_Step", new Dictionary<string, object>
            {
               {"duration", duration},
               {"mousemovement", am.mouseMovement},
               {"leftclicks", am.leftClicks},
               {"rightclicks", am.rightClicks}
            });
            break;
         case  4:
            am.sendEvent("Visualization_Step", new Dictionary<string, object>
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
