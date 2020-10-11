
using System;

public class LooseObject
{
    public ItemStack stack { get; protected set; }
    public Tile tile { get; protected set; }

    static Action<LooseObject> createCB;
    Action<LooseObject> changeCB;
    static Action<LooseObject> removeCB;

    private LooseObject(Tile tile, ItemStack stack)
    {

        this.tile = tile;
        this.stack = stack;
    }

    public static bool PlaceLooseObject(Tile tile, ItemStack stack)
    {
        if (tile.installedObject != null || tile.looseObject != null) return false;
        LooseObject obj = new LooseObject(tile, stack);
        createCB?.Invoke(obj);
        return tile.SetLooseObject(obj);
    }

    public static void RemoveLooseObject(LooseObject obj)
    {
        obj.tile.SetLooseObject(null);
        removeCB?.Invoke(obj);
    }

    public static void RegisterLooseObjectCreatedCB(Action<LooseObject> cb)
    {
        createCB += cb;
    }

    public static void UnregisterLooseObjectCreatedCB(Action<LooseObject> cb)
    {
        createCB -= cb;
    }

    public void RegisterLooseObjectChangedCB(Action<LooseObject> cb)
    {
        changeCB += cb;
    }

    public void UnregisterLooseObjectChangedCB(Action<LooseObject> cb)
    {
        changeCB -= cb;
    }

    public static void RegisterLooseObjectRemovedCB(Action<LooseObject> cb)
    {
        removeCB += cb;
    }

    public static void UnregisterLooseObjectRemovedCB(Action<LooseObject> cb)
    {
        removeCB -= cb;
    }
}
