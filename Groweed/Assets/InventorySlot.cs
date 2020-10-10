using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemStack stack;

    public void setStack(ItemStack stack)
    {
        this.stack = stack;
        if(stack != null)
        {
            transform.GetChild(0).GetComponent<Image>().enabled = true;
            transform.GetChild(0).GetComponent<Image>().sprite = stack.item.sprite;
            transform.GetChild(1).GetComponent<Text>().enabled = true;
            transform.GetChild(1).GetComponent<Text>().text = stack.count.ToString();
        } else
        {
            transform.GetChild(0).GetComponent<Image>().enabled = false;
            transform.GetChild(1).GetComponent<Text>().enabled = false;
        }
    }

    public void clicked()
    {
        Debug.Log("Hello!");
        MouseController.Instance.SetItemStackInHand(stack);
        setStack(null);
    }
}
