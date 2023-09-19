using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityScript))]
public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI hpText, lvlText, expText, fpsText;
    
    [SerializeField]
    GameObject toolbarSlotsParent;

    [SerializeField]
    GameObject toolbarPanelParent;

    [SerializeField] 
    Color activeSlotColor, defaultSlotColor;

    UnityEngine.UI.Image[] toolbarSlots, toolbarPanel;
    TMPro.TextMeshProUGUI[] toolbarStacks;

    Entity entity;
    Inventory    inventory;

    int  activeToolbarSlot = 0;
    bool inventoryToggle    = false;

    float fps   = 0.0f;
    float timer = 0.0f;

    void Start()
    {
        entity    = GetComponent<EntityScript>().Entity;
        inventory = entity.Inventory;

        toolbarSlots  = new UnityEngine.UI.Image[inventory.Slots()];
        toolbarPanel  = new UnityEngine.UI.Image[inventory.Slots()];
        toolbarStacks = new TMPro.TextMeshProUGUI[inventory.Slots()];

        for(int i = 0; i < inventory.Slots(); i++)
        {
            toolbarSlots[i]   = toolbarSlotsParent.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>();
            toolbarPanel[i]   = toolbarPanelParent.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>();
            toolbarStacks[i]  = toolbarSlotsParent.transform.GetChild(i).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        }

        SelectToolbarSlot(activeToolbarSlot);
    }

    void UpdateInventoryUI()
    {
        for(int i = 0 ; i < inventory.Slots(); i++)
        {
            ItemStack currentItem = inventory.GetItemStack(i);

            Sprite icon = currentItem.Item.Icon();

            toolbarPanel[i].enabled    = i < 9 || inventoryToggle;
            toolbarSlots[i].enabled    = i < 9 || inventoryToggle;
            toolbarStacks[i].text      = "";

            if(icon == null)
            {
                toolbarSlots[i].enabled = false;
                toolbarStacks[i].enabled = false;
            }
            else if (i < 9 || inventoryToggle)
            {
                toolbarSlots[i].enabled = true;
                toolbarSlots[i].sprite = icon;

                toolbarStacks[i].enabled = true;
                toolbarStacks[i].text = currentItem.Stacks >= 1 ? currentItem.Stacks.ToString() : "";
            }
        }

    }

    void UpdateHpUI()
    {
        hpText.text  = "HP: " + ((int)entity.Hp).ToString() + " / " + ((int)entity.MaxHp).ToString();
        lvlText.text = "Nivel: " + entity.Level;
        expText.text = "Exp: " + entity.Exp + " / " + entity.NextLevelExp();
    }


    void SelectToolbarSlot(int slot)
    {          
        toolbarPanel[activeToolbarSlot].color = defaultSlotColor;
        toolbarPanel[slot].color              = activeSlotColor;

        activeToolbarSlot = slot; 
    }


    void Update()
    {
        UpdateHpUI();
        UpdateInventoryUI();

        for ( int i = 1; i < 10; ++i )
        {
            if ( Input.GetKeyDown( "" + i ) )
            {
                SelectToolbarSlot(i-1);
                entity.Inventory.SelectedItemSlot = i-1;
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryToggle = !inventoryToggle;
        }

        fps++;

        if(timer % 60 >= 1.0f)
        {
            fpsText.text = "Fps: " + fps;
            
            timer = 0.0f;
            fps   = 0.0f;

        }
        
        timer += Time.deltaTime;
    }

}
