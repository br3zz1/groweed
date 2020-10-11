using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMenuScript : MonoBehaviour
{
    public static ButtonMenuScript Instance { get; protected set; }
    public Sprite bulldozeSprite;

    

    public GameObject mouseController;

    public GameObject creativeHotbar;
    public GameObject inventoryHotbar;
    bool creative = false;

    public GameObject terraformMenu;
    bool terraformMenuEnabled = false;
    int terraformMenuButtons = 0;

    public GameObject objectMenu;
    bool objectMenuEnabled = false;
    int objectMenuButtons = 0;

    public GameObject buttonPrefab;

    public GameObject objectInventoryPanel;
    public GameObject inventorySlotPrefab;

    public InventorySlot[] invSlots;
    public InventorySlot[] objectInvSlots;

    Inventory objectInventory;

    public Inventory playerInv;

    public void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Why are there more than one ButtonMenuScript instances?");
        }
        Instance = this;

        terraformMenuEnabled = terraformMenu.activeSelf;
        objectMenuEnabled = objectMenu.activeSelf;

        playerInv = new Inventory(9);

        invSlots = new InventorySlot[9];
        int i = 0;
        foreach(Transform child in inventoryHotbar.transform)
        {
            InventorySlot invSlot = child.GetComponent<InventorySlot>();
            invSlot.index = i;
            invSlot.inv = playerInv;
            invSlots[i] = invSlot;
            i++;
        }

        GenerateButton("objects", "Remove", "Remove", bulldozeSprite);
    }

    public void GenerateButton(string menu, string visibleName, string name, Sprite sprite)
    {
        if(menu == "terraform")
        {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(terraformMenu.transform);
            button.name = "Button - " + visibleName;
            button.GetComponentInChildren<Text>().text = visibleName;
            button.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            button.GetComponent<Button>().onClick.AddListener(() => { SetTerraform(name); });
            terraformMenuButtons++;
        } else if(menu == "objects")
        {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(objectMenu.transform);
            button.name = "Button - " + visibleName;
            button.GetComponentInChildren<Text>().text = visibleName;
            button.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            button.GetComponent<Button>().onClick.AddListener(() => { SetObject(name); });
            objectMenuButtons++;
        }
        UpdateMenuSizes();
    }

    public void UpdateMenuSizes()
    {
        Vector2 size = terraformMenu.GetComponent<RectTransform>().sizeDelta;
        if(terraformMenuButtons > 5)
        {
            size.y = 70 + Mathf.CeilToInt((terraformMenuButtons - 5) / 5f) * 65;
        } else
        {
            size.y = 70;
        }
        terraformMenu.GetComponent<RectTransform>().sizeDelta = size;

        size = objectMenu.GetComponent<RectTransform>().sizeDelta;
        if (objectMenuButtons > 5)
        {
            size.y = 70 + Mathf.CeilToInt((objectMenuButtons - 5) / 5f) * 65;
        }
        else
        {
            size.y = 70;
        }
        objectMenu.GetComponent<RectTransform>().sizeDelta = size;
    }

    public void SetTerraform(string s)
    {
        MouseControllerCreative.Instance.SetTerraformTile(s);
    }

    public void SetObject(string s)
    {
        MouseControllerCreative.Instance.SetInstalledObject(s);
    }

    public void terraformMenuEnable()
    {
        terraformMenuEnabled = !terraformMenuEnabled;
        if(terraformMenuEnabled)
        {
            terraformMenu.SetActive(true);
            objectMenu.SetActive(false);
            objectMenuEnabled = false;
        } else
        {
            terraformMenu.SetActive(false);
        }
        
    }
    public void objectMenuEnable()
    {
        objectMenuEnabled = !objectMenuEnabled;
        if (objectMenuEnabled)
        {
            objectMenu.SetActive(true);
            terraformMenu.SetActive(false);
            terraformMenuEnabled = false;
        } else
        {
            objectMenu.SetActive(false);
        }
    }

    public void switchHotbar()
    {
        creative = !creative;

        if(mouseController.GetComponent<MouseControllerCreative>().enabled)
        {
            MouseControllerCreative.Instance.SetSelect();
        }

        creativeHotbar.SetActive(creative);
        mouseController.GetComponent<MouseControllerCreative>().enabled = creative;
        

        inventoryHotbar.SetActive(!creative);
        mouseController.GetComponent<MouseController>().enabled = !creative;
    }

    public void onInventoryChanged(Inventory inv)
    {
        for(int i = 0; i < invSlots.Length; i++)
        {
            invSlots[i].setStack(inv.getItemStackAt(i));
        }
    }

    public void openObjectInventory(InstalledObject obj)
    {
        closeObjectInventory();
        objectInventory = obj.inventory;
        objectInventoryPanel.SetActive(true);
        objectInvSlots = new InventorySlot[objectInventory.getSlotCount()];
        for (int i = 0; i < objectInventory.getSlotCount(); i++)
        {
            GameObject invSlot = Instantiate(inventorySlotPrefab);
            invSlot.transform.SetParent(objectInventoryPanel.transform);
            InventorySlot inventorySlot = invSlot.GetComponent<InventorySlot>();
            inventorySlot.index = i;
            inventorySlot.inv = obj.inventory;
            objectInvSlots[i] = inventorySlot;
        }
        onObjectInventoryChanged(obj.inventory);
    }

    public void onObjectInventoryChanged(Inventory inv)
    {
        if (objectInventory != inv) return;
        for (int i = 0; i < objectInvSlots.Length; i++)
        {
            Debug.Log(objectInvSlots[i]);
            objectInvSlots[i].setStack(inv.getItemStackAt(i));
        }
    }

    public void closeObjectInventory()
    {
        objectInventory = null;
        foreach (Transform child in objectInventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        objectInventoryPanel.SetActive(false);
    }
}
