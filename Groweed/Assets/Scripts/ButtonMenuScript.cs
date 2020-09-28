using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMenuScript : MonoBehaviour
{
    public static ButtonMenuScript Instance { get; protected set; }

    public GameObject terraformMenu;
    bool terraformMenuEnabled = false;
    int terraformMenuButtons = 0;

    public GameObject objectMenu;
    bool objectMenuEnabled = false;
    int objectMenuButtons = 0;

    public GameObject buttonPrefab;

    public void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Why are there more than one ButtonMenuScript instances?");
        }
        Instance = this;

        terraformMenuEnabled = terraformMenu.activeSelf;
        objectMenuEnabled = objectMenu.activeSelf;

        GenerateButton("objects", "Remove", "Remove");
    }

    public void GenerateButton(string menu, string visibleName, string name)
    {
        if(menu == "terraform")
        {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(terraformMenu.transform);
            button.name = "Button - " + visibleName;
            button.GetComponentInChildren<Text>().text = visibleName;
            button.GetComponent<Button>().onClick.AddListener(() => { SetTerraform(name); });
            terraformMenuButtons++;
        } else if(menu == "objects")
        {
            GameObject button = Instantiate(buttonPrefab);
            button.transform.SetParent(objectMenu.transform);
            button.name = "Button - " + visibleName;
            button.GetComponentInChildren<Text>().text = visibleName;
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
        MouseController.Instance.SetTerraformTile(s);
    }

    public void SetObject(string s)
    {
        MouseController.Instance.SetInstalledObject(s);
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


}
