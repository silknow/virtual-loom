using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class GeneralTechniqueDropdown : Dropdown
{
    private I18NDropdown _i18NDropdown;
    private int count = 0;
    void Start()
    {
        _i18NDropdown = GetComponent<I18NDropdown>();
    }
    protected override DropdownItem CreateItem(DropdownItem itemTemplate)
    {
        DropdownItem ddi=base.CreateItem(itemTemplate);
        ddi.transform.GetChild(3).GetComponentInChildren<BoundTooltipTrigger>().text = _i18NDropdown.getKeys()[count] + "_description";
        count++;
        return ddi;
    }

   protected override GameObject CreateDropdownList(GameObject template)
   {
       count = 0;
       GameObject ddl = base.CreateDropdownList(template);
       return ddl;
   }
}
