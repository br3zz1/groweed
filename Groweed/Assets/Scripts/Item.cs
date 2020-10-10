using System;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string type;
    public Sprite sprite;
    private static List<Item> items;

    private Item(string type, Sprite sprite)
    {
        this.type = type;
        this.sprite = sprite;
    }
    public static void CreateItem(string type, Sprite sprite)
    {
        if (items == null) items = new List<Item>();
        Item item = new Item(type,sprite);
        items.Add(item);
    }

    public static Item getItemByType(string type)
    {
        foreach(Item item in items)
        {
            if (item.type == type)
            {
                return item;
            }
        }
        Debug.Log("Couldn't find Item: " + type);
        return null;
    }
}
