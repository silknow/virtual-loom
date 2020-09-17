using System;
using UnityEngine;

namespace Silknow_UI.Scripts
{
    public class LayerPanelManager : MonoBehaviour
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
            foreach (var yarnPanel in WizardController.instance.yarnPanels)
            {
                var panel = Instantiate(prefabYarnLayerPanel, scrollViewContent)
                    .GetComponent<YarnLayerPanel>();
                panel.outputColor = yarnPanel.outputColor;
                panel.yarnNumber = yarnPanel.yarnNumber;
                panel.isVisible = true;
            }
        
        }
    }
}