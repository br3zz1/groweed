using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class World
{

    Tile[,] tiles;

    public Dictionary<string, InstalledObject> installedObjectPrototypes;
    Action<InstalledObject> installedObjectCreatedCB;
    Action<InstalledObject> installedObjectRemovedCB;

    int width;
    public int Width { get => width; }
    int height;
    public int Height { get => height; }

    public World(int width = 100, int height = 100)
    {
        this.width = width;
        this.height = height;

        tiles = new Tile[width, height];

        installedObjectPrototypes = new Dictionary<string, InstalledObject>();
        AddInstalledObjectPrototypes();

        SimplexNoiseGenerator sng = new SimplexNoiseGenerator();


        // Tile generation
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                float xCoord = (float)x*2;
                float yCoord = (float)y*2;
                float dist = Vector2.Distance(new Vector2(x,y),new Vector2(width/2,height/2)) / (width/2);
                //Debug.Log(sng.getDensity(new Vector3(xCoord, yCoord, 0)));
                if (1 - dist + sng.coherentNoise(x, y, 0) > 0.6f)
                {
                    tiles[x, y].Type = "Grass";
                }
                else if (1 - dist + sng.coherentNoise(x, y, 0) > 0.5f)
                {
                    tiles[x, y].Type = "Sand";
                }
                else if (1 - dist + sng.coherentNoise(x, y, 0) > 0.4f)
                {
                    tiles[x, y].Type = "WaterShallow";
                }
                else if (1 - dist + sng.coherentNoise(x, y, 0) > 0.3f)
                {
                    tiles[x, y].Type = "Water";
                }
                else 
                {
                    tiles[x, y].Type = "WaterDeep";
                }
            }
        }
    }

    public void generateTrees()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height-1; y >= 0; y--)
            {
                if (UnityEngine.Random.Range(0, 1000) > 925 && tiles[x, y].Type == "Grass")
                {
                    PlaceInstalledObject("Tree", tiles[x, y]);
                }
                else if (UnityEngine.Random.Range(0, 1000) > 975 && (tiles[x, y].Type == "Grass" || tiles[x,y].Type == "Sand"))
                {
                    PlaceInstalledObject("Rocks1", tiles[x, y]);
                }
                else if (UnityEngine.Random.Range(0, 1000) > 975 && (tiles[x, y].Type == "Grass" || tiles[x, y].Type == "Sand"))
                {
                    PlaceInstalledObject("Rocks2", tiles[x, y]);
                }
            }
        }
    }

    public void PlaceInstalledObject(string objectType, Tile t)
    {
        if (installedObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("Can't find prototype named: " + objectType);
            return;
        }
        InstalledObject proto = installedObjectPrototypes[objectType];
        if (proto.blacklistedTiles != null)
        {
            if (proto.blacklistedTiles.Contains(t.Type))
            {
                return;
            }
        }
        if (proto.whitelistedTiles != null)
        {
            if (!proto.whitelistedTiles.Contains(t.Type))
            {
                return;
            }
        }
        InstalledObject obj = InstalledObject.InstallObject(proto, t);
        if (obj == null) return;
        if (installedObjectCreatedCB != null) installedObjectCreatedCB(obj);
    }

    public void RemoveInstalledObject(Tile t)
    {
        if(t.installedObject == null)
        {
            return;
        }
        InstalledObject obj = t.installedObject;
        t.InstallObject(null);
        if (obj.ruleTile)
        {
            InstalledObject.UpdateNeighbours(t, obj);
        }
        if (installedObjectRemovedCB != null)installedObjectRemovedCB(obj);
    }

    public void PlaceLooseObject(ItemStack stack, Tile t)
    {
        LooseObject looseObject = new LooseObject(t,stack);
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= width || x < 0 || y >= height || y < 0)
        {
            return null;
        }
        return tiles[x, y];
    }

    public Tile[] GetTileNeighbours(int x, int y)
    {
        Tile[] tiles = new Tile[8];
        tiles[0] = GetTileAt(x - 1, y - 1);
        tiles[1] = GetTileAt(x, y - 1);
        tiles[2] = GetTileAt(x + 1, y - 1);
        tiles[3] = GetTileAt(x - 1, y);
        tiles[4] = GetTileAt(x + 1, y);
        tiles[5] = GetTileAt(x - 1, y + 1);
        tiles[6] = GetTileAt(x, y - 1);
        tiles[7] = GetTileAt(x + 1, y + 1);
        int tileCount = 0;
        for(int i = 0; i < 8; i++)
        {
            if(tiles[i] != null) {
                tileCount++;
            }
        }
        Tile[] realTiles = new Tile[tileCount];
        int reali = 0;
        for (int i = 0; i < 8; i++)
        {
            if (tiles[i] != null) {
                realTiles[reali] = tiles[i];
                reali++;
            }
        }
        return realTiles;
    }

    public bool isInstalledObjectPrototypeRuleTile(string name)
    {
        if(installedObjectPrototypes.ContainsKey(name) == false)
        {
            Debug.LogError("Prototype not found: " + name);
            return false;
        }
        return installedObjectPrototypes[name].ruleTile;
    }

    public string getInstalledObjectPrototypeDragBuildPattern(string name)
    {
        if (installedObjectPrototypes.ContainsKey(name) == false)
        {
            Debug.LogError("Prototype not found: " + name);
            return "Single";
        }
        return installedObjectPrototypes[name].dragBuildPattern;
    }

    public int getInstalledObjectPrototypeStages(string name)
    {
        if (installedObjectPrototypes.ContainsKey(name) == false)
        {
            Debug.LogError("Prototype not found: " + name);
            return 0;
        }
        return installedObjectPrototypes[name].stages;
    }

    public void RegisterInstalledObjectCreatedCB(Action<InstalledObject> cb)
    {
        installedObjectCreatedCB += cb;
    }

    public void UnregisterInstalledObjectCreatedCB(Action<InstalledObject> cb)
    {
        installedObjectCreatedCB -= cb;
    }

    public void RegisterInstalledObjectRemovedCB(Action<InstalledObject> cb)
    {
        installedObjectRemovedCB += cb;
    }

    public void UnregisterInstalledObjectRemovedCB(Action<InstalledObject> cb)
    {
        installedObjectRemovedCB -= cb;
    }

    public void AddInstalledObjectPrototypes()
    {
        string[] defaultBlacklist = { "Water", "WaterShallow", "WaterDeep"};
        InstalledObject wallPrototype = InstalledObject.CreatePrototype("Wall", moveCost: 0f, ruleTile: true, dragBuildPattern: "Fill", blacklistedTiles: defaultBlacklist);
        installedObjectPrototypes.Add("Wall", wallPrototype);
        //ButtonMenuScript.Instance.GenerateButton("objects", "Wall", "Wall",WorldController.Instance.getInstalledObjectSpriteByName("Wall"));
        InstalledObject treePrototype = InstalledObject.CreatePrototype("Tree", moveCost: 0f, blacklistedTiles: defaultBlacklist);
        installedObjectPrototypes.Add("Tree", treePrototype);
        //ButtonMenuScript.Instance.GenerateButton("objects", "Tree", "Tree", WorldController.Instance.getInstalledObjectSpriteByName("Tree"));
        InstalledObject rocks1Prototype = InstalledObject.CreatePrototype("Rocks1", layer: "Background", blacklistedTiles: defaultBlacklist);
        installedObjectPrototypes.Add("Rocks1", rocks1Prototype);
        //ButtonMenuScript.Instance.GenerateButton("objects", "Rocks1", "Rocks1", WorldController.Instance.getInstalledObjectSpriteByName("Rocks1"));
        InstalledObject rocks2Prototype = InstalledObject.CreatePrototype("Rocks2", layer: "Background", blacklistedTiles: defaultBlacklist);
        installedObjectPrototypes.Add("Rocks2", rocks2Prototype);
        //ButtonMenuScript.Instance.GenerateButton("objects", "Rocks2", "Rocks2", WorldController.Instance.getInstalledObjectSpriteByName("Rocks2"));
        InstalledObject plantPrototype = InstalledObject.CreatePrototype("Plant", stages: 4, blacklistedTiles: defaultBlacklist);
        installedObjectPrototypes.Add("Plant", plantPrototype);
        //ButtonMenuScript.Instance.GenerateButton("objects", "Plant", "Plant", WorldController.Instance.getInstalledObjectSpriteByName("Plant"));
    }
}
