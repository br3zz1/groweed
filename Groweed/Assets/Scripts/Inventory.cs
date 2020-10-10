using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private ItemStack[] slots;

    public Inventory(int size)
    {
        slots = new ItemStack[size];
    }

    public bool addItemStack(ItemStack stack)
    {
        for(int i = 0; i < slots.Length; )
        {

        }
        return false;
    }

    public void addItemStack(ItemStack stack, int slot)
    {
        if (slot > slots.Length - 1) return;
        ItemStack ss = slots[slot];
        if (ss != null) 
        {
            if(ss.item == stack.item)
            {
                ss.count += stack.count;
                stack.count = 0;
            } else
            {
                ItemStack _ = new ItemStack(ss.item, ss.count);
                ss.item = stack.item;
                ss.count = stack.count;
                stack.item = _.item;
                stack.count = _.count;
            }
        } else
        {
            ss = new ItemStack(stack.item, stack.count);
            stack.count = 0;
        }
    }
}
