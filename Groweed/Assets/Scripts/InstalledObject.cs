using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InstalledObject
{
    public string type { get; protected set; }
    public Tile tile { get; protected set; }

    float movementCost = 1f;
    public bool ruleTile { get; protected set; }
    public string dragBuildPattern;

    int width;
    int height;

    Action<InstalledObject> changeCB;

    // TODO - object rotation

    protected InstalledObject()
    {

    }

    public static InstalledObject CreatePrototype(string type, int width=1, int height=1, float moveCost=1f, bool ruleTile=false, string dragBuildPattern = "Single")
    {
        InstalledObject obj = new InstalledObject();
        obj.type = type;
        obj.width = width;
        obj.height = height;
        obj.movementCost = moveCost;
        obj.ruleTile = ruleTile;
        obj.dragBuildPattern = dragBuildPattern;
        return obj;
    }

    public static InstalledObject InstallObject(InstalledObject proto, Tile tile)
    {
        InstalledObject obj = new InstalledObject();
        obj.type = proto.type;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.movementCost = proto.movementCost;
        obj.ruleTile = proto.ruleTile;
        obj.dragBuildPattern = proto.dragBuildPattern;

        obj.tile = tile;
        if(tile.installedObject != null)
        {
            Debug.LogError("Object already installed at this tile!");
            return null;
        }
        tile.InstallObject(obj);

        if(obj.ruleTile)
        {
            UpdateNeighbours(tile, obj);
        }

        return obj;
    }

    public static void UpdateNeighbours(Tile tile, InstalledObject obj)
    {
        Tile t;
        t = tile.world.GetTileAt(tile.x, tile.y + 1);
        if (t != null && t.installedObject != null && t.installedObject.type == obj.type)
        {
            t.installedObject.changeCB(t.installedObject);
        }
        t = tile.world.GetTileAt(tile.x + 1, tile.y);
        if (t != null && t.installedObject != null && t.installedObject.type == obj.type)
        {
            t.installedObject.changeCB(t.installedObject);
        }
        t = tile.world.GetTileAt(tile.x, tile.y - 1);
        if (t != null && t.installedObject != null && t.installedObject.type == obj.type)
        {
            t.installedObject.changeCB(t.installedObject);
        }
        t = tile.world.GetTileAt(tile.x - 1, tile.y);
        if (t != null && t.installedObject != null && t.installedObject.type == obj.type)
        {
            t.installedObject.changeCB(t.installedObject);
        }
    }

    public void RegisterInstalledObjectChangedCB(Action<InstalledObject> cb)
    {
        changeCB += cb;
    }

    public void UnregisterInstalledObjectChangedCB(Action<InstalledObject> cb)
    {
        changeCB -= cb;
    }
}
