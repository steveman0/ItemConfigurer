using System;
using System.Collections.Generic;
using FortressCraft.Community.Utilities;
using UnityEngine;

public class ItemConfigurerMod : FortressCraftMod
{
    public static int ItemConfigurerID = ModManager.mModMappings.ItemsByKey["steveman0.ItemConfigurer"].ItemId;
    public static ItemConfigurerWindow TheWindow = new ItemConfigurerWindow();

    public override ModRegistrationData Register()
    {
        ModRegistrationData modRegistrationData = new ModRegistrationData();
        //modRegistrationData.HandledCustomRegistrationDataIds.Add("steveman0.ItemConfigurerInterface");

        //GameObject Sync = new GameObject("ModUIHandler");
        //Sync.AddComponent<ModUIHandler>();
        //Sync.SetActive(true);
        //Sync.GetComponent<ModUIHandler>().enabled = true;

        Debug.Log("Item Configurer Mod v1 registered");

        return modRegistrationData;
    }

    public override ModItemActionResults PerformItemAction(ModItemActionParameters parameters)
    {
        ModItemActionResults results = new ModItemActionResults();
        results.Consume = false;

        if (parameters.ItemToUse.mnItemID == ItemConfigurerID)
        {
            SegmentEntity segmentEntity = WorldScript.instance.localPlayerInstance.mPlayerBlockPicker.selectedEntity;
            ConveyorEntity conveyor = segmentEntity as ConveyorEntity;
            MassStorageOutputPort port = segmentEntity as MassStorageOutputPort;
            ItemConfigurerInterface machine = segmentEntity as ItemConfigurerInterface;
            if (conveyor == null && port == null && machine == null || (conveyor != null && conveyor.mValue != 12))
            {
                Debug.LogWarning("ItemConfigurer didn't find right item found " + segmentEntity.mType.ToString());
                return results;
            }
            ModManager.OpenModUI(TheWindow, segmentEntity);
            //UIUtil.HandleThisMachineWindow(segmentEntity, ModUIHandler.instance.ConfigurerWindow);
            //if (segmentEntity != null && UIManager.AllowInteracting && UIManager.HudShown)
            //{
            //    GenericMachinePanelScript panel = UIManager.instance.mGenericMachinePanel;
            //    panel.gameObject.SetActive(true);
            //    panel.Background_Panel.SetActive(true);
            //    panel.RepositionWindow();
            //    panel.targetEntity = segmentEntity;
            //    panel.currentWindow = ModUIHandler.instance.ConfigurerWindow;
            //    panel.currentWindow.SpawnWindow(segmentEntity);

            //    UIManager.AddUIRules("Machine", UIRules.RestrictMovement | UIRules.RestrictLooking | UIRules.RestrictBuilding | UIRules.RestrictInteracting | UIRules.AllowInventory | UIRules.ShowCursor | UIRules.SetUIUpdateRate);
            //    DragAndDropManager.instance.EnableDragBackground();
            //}
            //ModUIHandler.instance.Entity = segmentEntity;
        }
        return results;
    }

    //public override void HandleCustomRegistrationData(ICustomModRegistrationData registrationData)
    //{
    //    if (registrationData.Id == "steveman0.ItemConfigurerInterface")
    //    {

    //    }
    //}
}

//public class ItemConfigurerRegistrationData : ICustomModRegistrationData
//{
//    public string Id {get {return "steveman0.ItemConfigurerInterface";}}
//    public Type targetentity;
//    public string ItemConfigMachineName;
//    public delegate void ItemConfigurerHandler(ItemBase item);
//}

public abstract class ItemConfigurerHandler
{
    public abstract void HandleConfiguredItem();
}


//class ModUIHandler : MonoBehaviour
//{
//    public static ModUIHandler instance = null;
//    public GenericMachinePanelScript panel;
//    public ItemConfigurerWindow ConfigurerWindow = new ItemConfigurerWindow();
//    public bool WindowActive = false;
//    public SegmentEntity Entity;

//    void Start()
//    {
//        instance = this;
//    }

//    void Update()
//    {
//        if (panel == null)
//            panel = UIManager.instance.mGenericMachinePanel;
//        if (panel.gameObject.activeSelf && panel.currentWindow == this.ConfigurerWindow)
//        {
//            this.WindowActive = true;
//        }
//        else if (!panel.gameObject.activeSelf && this.WindowActive)
//        {
//            if (Entity != null && UIManager.instance.mGenericMachinePanel.targetEntity != null)
//                UIUtil.DisconnectUI(Entity);
//            if (UIManager.instance.mGenericMachinePanel.targetEntity == null)
//            {
//                this.Entity = null;
//                this.WindowActive = false;
//            }
//        }
//    }
//}

