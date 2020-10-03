using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{

    string type = "Grass";
    Action<Tile> cbTileTypeChanged;
    TileMeta[] meta;
    
    public string Type { get => type; set
        {
            if (type == value) return;
            type = value;
            if(cbTileTypeChanged != null) cbTileTypeChanged(this);
        } 
    }


    LooseObject looseObject;
    public InstalledObject installedObject { get; protected set; }

    public World world { get; protected set; }
    public int x { get; protected set; }
    public int y { get; protected set; }

    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.x = x;
        this.y = y;
    }

    public void RegisterTileTypeChangedCB(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChangedCB(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    public void InstallObject(InstalledObject obj)
    {
        if (obj == null) installedObject = null;
        if (installedObject != null)
        {
            if (installedObject.layer != "Background") return;
            world.RemoveInstalledObject(this);
        }
        installedObject = obj;
    }

}
