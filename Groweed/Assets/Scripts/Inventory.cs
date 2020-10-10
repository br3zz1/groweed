using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private ItemStack[] slots;
    private Action<Inventory> changeCB;

    public Inventory(int size)
    {
        slots = new ItemStack[size];
    }

    public bool addItemStack(ItemStack stack)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(addItemStack(stack,i))
            {
                return true;
            }
        }
        return false;
    }

    public bool addItemStack(ItemStack stack, int index)
    {
        if (index >= slots.Length) return false;
        if (stack == null)
        {
            slots[index] = null;
            changeCB?.Invoke(this);
            return true;
        }
        if (slots[index] == null)
        {
            slots[index] = stack;
            changeCB?.Invoke(this);
            return true;
        } else
        {
            if(slots[index].item == stack.item)
            {
                slots[index].count += stack.count;
                changeCB?.Invoke(this);
                return true;
            }
        }
        return false;
    }

    public ItemStack getItemStackAt(int index)
    {
        if (index >= slots.Length) return null;
        return slots[index];
    }

    public void RegisterChangeCB(Action<Inventory> cb)
    {
        changeCB += cb;
    }

    public void UnregisterChangeCB(Action<Inventory> cb)
    {
        changeCB -= cb;
    }
}
