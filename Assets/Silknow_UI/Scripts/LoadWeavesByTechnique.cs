using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Honeti;
using UnityEngine;

public class LoadWeavesByTechnique : MonoBehaviour
{
   public Transform[] satinUI = new Transform[2];
   public RectTransform satinContent;
   public Transform[] twillUI = new Transform[2];
   public RectTransform twillContent;
   public Transform[] tabbyUI = new Transform[2];
   public RectTransform tabbyContent;
   
   //prefab Weave
   public GameObject weavePanelPrefab;

   public List<Weave> everyWeave;

   private void Awake()
   {
      everyWeave = Resources.LoadAll<Weave>("Weaves").ToList();
   }

   private void OnEnable()
   {
      foreach (RectTransform child in satinContent) {
         GameObject.Destroy(child.gameObject);
      }
      foreach (RectTransform child in twillContent) {
         GameObject.Destroy(child.gameObject);
      }
      foreach (RectTransform child in tabbyContent) {
         GameObject.Destroy(child.gameObject);
      }
      
      
      // SATIN 
      var listOfSatin =
         everyWeave.FindAll(wt => wt.techniques.Contains(WizardController.instance._generalTechnique) && wt.type == Weave.WeavingTechniqueType.Satin);
      if (listOfSatin.Count > 0)
      {
         satinUI[0].gameObject.SetActive(true);
         satinUI[1].gameObject.SetActive(true);

         foreach (var satinWeave in listOfSatin)
         {
            var panel = GameObject.Instantiate(weavePanelPrefab, satinContent);
            panel.GetComponent<WeavePanel>().image = satinWeave.weavePattern;
            panel.GetComponent<WeavePanel>().name = I18N.instance.getValue(satinWeave.primaryName)+satinWeave.code;
            panel.GetComponent<WeavePanel>().weavingTechnique = satinWeave;
            if(WizardController.instance.selectedWeave == satinWeave)
               panel.GetComponent<WeavePanel>().SetSelectedColor();
         }
      }
      else
      {
         satinUI[0].gameObject.SetActive(false);
         satinUI[1].gameObject.SetActive(false);
      }
      
      // TWILL 
      var listOfTwill =
         everyWeave.FindAll(wt => wt.techniques.Contains(WizardController.instance._generalTechnique) && wt.type == Weave.WeavingTechniqueType.Twill);
      if (listOfTwill.Count > 0)
      {
         twillUI[0].gameObject.SetActive(true);
         twillUI[1].gameObject.SetActive(true);

         foreach (var twillWeave in listOfTwill)
         {
            var panel = GameObject.Instantiate(weavePanelPrefab, twillContent);
            panel.GetComponent<WeavePanel>().image = twillWeave.weavePattern;
            panel.GetComponent<WeavePanel>().name = I18N.instance.getValue(twillWeave.primaryName)+twillWeave.code;
            panel.GetComponent<WeavePanel>().weavingTechnique = twillWeave;
            if(WizardController.instance.selectedWeave == twillWeave)
               panel.GetComponent<WeavePanel>().SetSelectedColor();
         }
      }
      else
      {
         twillUI[0].gameObject.SetActive(false);
         twillUI[1].gameObject.SetActive(false);
      }
      
      // TABBY 
      var listOfTabby =
         everyWeave.FindAll(wt => wt.techniques.Contains(WizardController.instance._generalTechnique) && wt.type == Weave.WeavingTechniqueType.Tabby);
      if (listOfTabby.Count > 0)
      {
         tabbyUI[0].gameObject.SetActive(true);
         tabbyUI[1].gameObject.SetActive(true);

         foreach (var tabbyWeave in listOfTabby)
         {
            var panel = GameObject.Instantiate(weavePanelPrefab, tabbyContent);
            panel.GetComponent<WeavePanel>().image = tabbyWeave.weavePattern;
            panel.GetComponent<WeavePanel>().name = I18N.instance.getValue(tabbyWeave.primaryName)+tabbyWeave.code;
            panel.GetComponent<WeavePanel>().weavingTechnique = tabbyWeave;
            if(WizardController.instance.selectedWeave == tabbyWeave)
               panel.GetComponent<WeavePanel>().SetSelectedColor();
         }
      }
      else
      {
         tabbyUI[0].gameObject.SetActive(false);
         tabbyUI[1].gameObject.SetActive(false);
      }
      
   }
}
