using System;
using System.Linq;
using UnityEngine;

namespace Silknow_UI.Scripts
{
    public class LayerPanelManager : Singleton<LayerPanelManager>
    {
        public GameObject prefabYarnLayerPanel;
        public Transform scrollViewContent;

        private void OnEnable()
        {
        
                foreach (RectTransform child in scrollViewContent)
                {
                    if (child != scrollViewContent)
                        Destroy(child.gameObject);
                }

        }

        public void RefreshLayers()
        {
            
            if (WizardController.instance.yarnEntities.FirstOrDefault(y => y.geometryIndex == 0) == null)
            {
                var panel = Instantiate(prefabYarnLayerPanel, scrollViewContent)
                    .GetComponent<YarnLayerPanel>();
                panel.outputColor = WizardController.instance.warpPanel.outputColor;
                panel.yarnNumber = "Warp";
                panel.isVisible = true;
                panel.yarnEntity = new YarnEntity(Color.black, -1,WizardController.instance.warpPanel,0);
            }
            var listOfEntities = WizardController.instance.yarnEntities.OrderBy(y => y.geometryIndex);

            
            foreach (var yarnEntity in listOfEntities)
            {
                var panel = Instantiate(prefabYarnLayerPanel, scrollViewContent)
                    .GetComponent<YarnLayerPanel>();
                panel.outputColor = yarnEntity.yarnPanel.outputColor;
                panel.yarnNumber = yarnEntity.yarnPanel.yarnNumber;
                panel.isVisible = true;
                panel.yarnEntity = yarnEntity;

            }
        
        }
    }
}