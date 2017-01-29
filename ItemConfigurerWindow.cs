using System;
using System.Collections.Generic;
using System.Linq;
using FortressCraft.Community.Utilities;
using UnityEngine;

public class ItemConfigurerWindow : BaseMachineWindow
{
    private static bool dirty;
    private List<ItemBase> SearchResults;
    private string EntryString;
    private int Counter;

    public override void SpawnWindow(SegmentEntity targetEntity)
    {
        ConveyorEntity conveyor = targetEntity as ConveyorEntity;
        MassStorageOutputPort port = targetEntity as MassStorageOutputPort;
        ItemConfigurerInterface machine = targetEntity as ItemConfigurerInterface;
        //Catch for when the window is called on an inappropriate machine
        if (conveyor == null && port == null && machine == null || (conveyor != null && conveyor.mValue != 12))
        {
            //GenericMachinePanelScript.instance.Hide();
            UIManager.RemoveUIRules("Machine");
            return;
        }
        UIUtil.UIdelay = 0;
        UIUtil.UILock = true;

        UIManager.mbEditingTextField = true;

        if (conveyor != null)
            this.manager.SetTitle("Conveyor Filter - Item Search");
        else if (port != null)
            this.manager.SetTitle("Output Port - Item Search");
        else if (machine != null)
            this.manager.SetTitle(machine.ItemConfigMachineName());
        this.manager.AddButton("searchcancel", "Cancel", 100, 0);
        this.manager.AddBigLabel("searchtitle", "Enter Item Search Term", Color.white, 50, 40);
        this.manager.AddBigLabel("searchtext", "_", Color.cyan, 50, 65);
        if (this.SearchResults != null)
        {
            int count = this.SearchResults.Count;
            int spacing = 60; //Spacing between each registry line
            int yoffset = 100; //Offset below button row
            int labeloffset = 60; //x offset for label from icon

            for (int n = 0; n < count; n++)
            {
                this.manager.AddIcon("itemicon" + n, "empty", Color.white, 0, yoffset + (spacing * n));
                this.manager.AddBigLabel("iteminfo" + n, "Inventory Item", Color.white, labeloffset, yoffset + (spacing * n));
            }
        }
        else
            UIManager.AddUIRules("TextEntry", UIRules.RestrictMovement | UIRules.RestrictLooking | UIRules.RestrictBuilding | UIRules.RestrictInteracting | UIRules.SetUIUpdateRate);
        dirty = true;
    }

    public override void UpdateMachine(SegmentEntity targetEntity)
    {
        ConveyorEntity conveyor = targetEntity as ConveyorEntity;
        MassStorageOutputPort port = targetEntity as MassStorageOutputPort;
        ItemConfigurerInterface machine = targetEntity as ItemConfigurerInterface;
        //Catch for when the window is called on an inappropriate machine
        if (conveyor == null && port == null && machine == null || (conveyor != null && conveyor.mValue != 12))
        {
            GenericMachinePanelScript.instance.Hide();
            UIManager.RemoveUIRules("Machine");
            return;
        }
        UIUtil.UIdelay = 0;
        GenericMachinePanelScript.instance.Scroll_Bar.GetComponent<UIScrollBar>().scrollValue -= Input.GetAxis("Mouse ScrollWheel");

        if (!dirty)
            return;

        if (this.SearchResults == null)
        {
            this.Counter++;
            foreach (char c in Input.inputString)
            {
                if (c == "\b"[0])  //Backspace
                {
                    if (this.EntryString.Length != 0)
                        this.EntryString = this.EntryString.Substring(0, this.EntryString.Length - 1);
                }
                else if (c == "\n"[0] || c == "\r"[0]) //Enter or Return
                {
                    this.SearchResults = new List<ItemBase>();

                    for (int n = 0; n < ItemEntry.mEntries.Length; n++)
                    {
                        if (ItemEntry.mEntries[n] == null) continue;
                        if (ItemEntry.mEntries[n].Name.ToLower().Contains(this.EntryString.ToLower()))
                            this.SearchResults.Add(ItemManager.SpawnItem(ItemEntry.mEntries[n].ItemID));
                    }
                    for (int n = 0; n < TerrainData.mEntries.Length; n++)
                    {
                        bool foundvalue = false;
                        if (TerrainData.mEntries[n] == null) continue;
                        if (TerrainData.mEntries[n].Name.ToLower().Contains(this.EntryString.ToLower()))
                        {
                            int count = TerrainData.mEntries[n].Values.Count;
                            for (int m = 0; m < count; m++)
                            {
                                if (TerrainData.mEntries[n].Values[m].Name.ToLower().Contains(this.EntryString.ToLower()))
                                {
                                    this.SearchResults.Add(ItemManager.SpawnCubeStack(TerrainData.mEntries[n].CubeType, TerrainData.mEntries[n].Values[m].Value, 1));
                                    foundvalue = true;
                                }
                            }
                            if (!foundvalue)
                                this.SearchResults.Add(ItemManager.SpawnCubeStack(TerrainData.mEntries[n].CubeType, TerrainData.mEntries[n].DefaultValue, 1));
                        }
                        if ((this.EntryString.ToLower().Contains("component") || this.EntryString.ToLower().Contains("placement") || this.EntryString.ToLower().Contains("multi")) && TerrainData.mEntries[n].CubeType == 600)
                        {
                            int count = TerrainData.mEntries[n].Values.Count;
                            for (int m = 0; m < count; m++)
                            {
                                this.SearchResults.Add(ItemManager.SpawnCubeStack(600, TerrainData.mEntries[n].Values[m].Value, 1));
                            }
                        }
                    }
                    if (this.SearchResults.Count == 0)
                        this.SearchResults = null;

                    UIManager.mbEditingTextField = false;
                    UIManager.RemoveUIRules("TextEntry");

                    this.manager.RedrawWindow();
                    return;
                }
                else
                    this.EntryString += c;
            }
            this.manager.UpdateLabel("searchtext", this.EntryString + (this.Counter % 20 > 10 ? "_" : ""), Color.cyan);
            dirty = true;
            return;
        }
        else
        {
            this.manager.UpdateLabel("searchtitle", "Searching for:", Color.white);
            this.manager.UpdateLabel("searchtext", this.EntryString, Color.cyan);
            int count = this.SearchResults.Count;
            for (int n = 0; n < count; n++)
            {
                ItemBase item = this.SearchResults[n];
                string itemname = ItemManager.GetItemName(item);
                string iconname = ItemManager.GetItemIcon(item);

                this.manager.UpdateIcon("itemicon" + n, iconname, Color.white);
                this.manager.UpdateLabel("iteminfo" + n, itemname, Color.white);
            }
        }
    }


    public override bool ButtonClicked(string name, SegmentEntity targetEntity)
    {
        ConveyorEntity conveyor = targetEntity as ConveyorEntity;
        MassStorageOutputPort port = targetEntity as MassStorageOutputPort;
        ItemConfigurerInterface machine = targetEntity as ItemConfigurerInterface;
        if (conveyor == null && port == null && machine == null || (conveyor != null && conveyor.mValue != 12))
            return false;

        if (name.Contains("searchcancel"))
        {
            this.SearchResults = null;
            UIManager.mbEditingTextField = false;
            UIManager.RemoveUIRules("TextEntry");
            this.EntryString = "";
            GenericMachinePanelScript.instance.Scroll_Bar.GetComponent<UIScrollBar>().scrollValue = 0.0f;
            GenericMachinePanelScript.instance.Hide();
            return true;
        }
        else if (name.Contains("itemicon"))
        {
            int slotNum = -1;
            int.TryParse(name.Replace("itemicon", ""), out slotNum); //Get slot name as number
            if (slotNum > -1)
            {
                ItemBase item = this.SearchResults[slotNum];
                int itemid = item.mnItemID;
                ushort cube = 0;
                ushort val = 0;
                if (itemid == -1)
                {
                    cube = (item as ItemCubeStack).mCubeType;
                    val = (item as ItemCubeStack).mCubeValue;
                }
                if (conveyor != null)
                    ConveyorFilterWindow.SetExemplar(conveyor, itemid, cube, val, false);
                else if (port != null)
                    MassStorageOutputPortWindow.SetExemplar(WorldScript.mLocalPlayer, port, item);
                else if (machine != null)
                    machine.HandleItemSelected(item);
                this.SearchResults = null;
                this.EntryString = "";
                GenericMachinePanelScript.instance.Hide();
                return true;
            }
        }
        return false;
    }

    public override void OnClose(SegmentEntity targetEntity)
    {
        this.SearchResults = null;
        this.EntryString = "";
        UIManager.mbEditingTextField = false;
        UIManager.RemoveUIRules("TextEntry");
        base.OnClose(targetEntity);
    }
}

