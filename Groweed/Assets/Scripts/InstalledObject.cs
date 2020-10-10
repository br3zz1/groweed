using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;

public class InstalledObject
{
    public string type { get; protected set; }
    public Tile tile { get; protected set; }

    public float movementCost { get; protected set; }
    public bool ruleTile { get; protected set; }
    public string dragBuildPattern;
    public string layer { get; protected set; }
    int stage; 
    public int Stage{ get => stage; set 
        {
            if (stage == value) return;
            stage = value;
            changeCB?.Invoke(this);
        }
    }
    public int stages { get; protected set; }
    int width;
    int height;

    Action<InstalledObject> changeCB;

    // TODO - object rotation

    protected InstalledObject()
    {

    }

    public static InstalledObject CreatePrototype(string type, int width=1, int height=1, float moveCost=1f, bool ruleTile=false, string dragBuildPattern = "Single", string layer = "Default", int stages = 0)
    {
        InstalledObject obj = new InstalledObject();
        obj.type = type;
        obj.width = width;
        obj.height = height;
        obj.movementCost = moveCost;
        obj.ruleTile = ruleTile;
        obj.dragBuildPattern = dragBuildPattern;
        obj.layer = layer;
        obj.stages = stages;
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
        obj.layer = proto.layer;
        obj.stages = proto.stages;
        obj.stage = 0;

        obj.tile = tile;
        if(tile.installedObject != null)
        {
            if(tile.installedObject.layer != "Background")
            {
                Debug.LogError("Object already installed at this tile!");
                return null;
            }
            tile.world.RemoveInstalledObject(tile);
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
