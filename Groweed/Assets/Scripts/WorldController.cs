﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }

    /*public Sprite grassSprite;
    public Sprite dirtSprite;
    public Sprite soilSprite;
    public Sprite sandSprite;
    public Sprite waterSprite;
    public Sprite wallSprite;*/

    Dictionary<string, Sprite> installedObjectSprites;
    Dictionary<string, Sprite> tileSprites;

    public World world { get; protected set; }

    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;
    Dictionary<LooseObject, GameObject> looseObjectGameObjectMap;

    public int width;
    public int height;

    public int randomTickSpeed = 10;

    public Inventory playerInv;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Why are there more than one WorldController instances?");
        }
        Instance = this;

        // Load sprites to dictionaries
        LoadSprites();

        // Instantiate player inventory
        playerInv = ButtonMenuScript.Instance.playerInv;
        playerInv.RegisterChangeCB(ButtonMenuScript.Instance.onInventoryChanged);
        playerInv.addItemStack(new ItemStack(Item.getItemByType("Stick"),1));

        // Generate world
        world = new World(width,height);
        world.RegisterInstalledObjectCreatedCB(onInstalledObjectCreated);
        world.RegisterInstalledObjectRemovedCB(onInstalledObjectRemoved);
        LooseObject.RegisterLooseObjectCreatedCB(onLooseObjectCreated);
        LooseObject.RegisterLooseObjectRemovedCB(onLooseObjectRemoved);

        foreach (KeyValuePair<string,InstalledObject> pair in world.installedObjectPrototypes)
        {
            ButtonMenuScript.Instance.GenerateButton("objects", pair.Key, pair.Key, getInstalledObjectSpriteByName(pair.Key));
        }
        

        // Object maps
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();
        looseObjectGameObjectMap = new Dictionary<LooseObject, GameObject>();

        for(int x = 0; x < world.Width; x++)
        {
            for(int y = 0; y < world.Height; y++)
            {
                Tile tile_data = world.GetTileAt(x,y);
                GameObject tile_go = new GameObject();

                tileGameObjectMap.Add( tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(x,y,0);
                tile_go.transform.SetParent(this.transform, true);
                
                tile_go.AddComponent<SpriteRenderer>();
                BoxCollider2D bc = tile_go.AddComponent<BoxCollider2D>();
                bc.offset = new Vector2(0.5f, 0.5f);
                bc.size = new Vector2(1f, 1f);
                onTileTypeChanged(tile_data);

                tile_data.RegisterTileTypeChangedCB(onTileTypeChanged);
            }
        }

        // Generate InstalledObjects (trees, etc.)
        world.generateTrees();
    }

    void LoadSprites()
    {
        installedObjectSprites = new Dictionary<string, Sprite>();
        tileSprites = new Dictionary<string, Sprite>();

        Sprite[] _tileSprites = Resources.LoadAll<Sprite>("Sprites/Tiles");
        Sprite[] _installedObjectSprites = Resources.LoadAll<Sprite>("Sprites/InstalledObjects");
        Sprite[] _itemSprites = Resources.LoadAll<Sprite>("Sprites/Items");

        foreach (Sprite s in _tileSprites)
        {
            tileSprites[s.name] = s;
            ButtonMenuScript.Instance.GenerateButton("terraform", s.name, s.name, s);
        }
        foreach (Sprite s in _installedObjectSprites)
        {
            installedObjectSprites[s.name] = s;
        }
        foreach (Sprite s in _itemSprites)
        {
            Item.CreateItem(s.name,s);
        }
    }

    void onTileTypeChanged(Tile tile_data)
    {
        GameObject tile_go = tileGameObjectMap[tile_data];
        if(tileSprites.ContainsKey(tile_data.Type) == false)
        {
            Debug.LogError("Couldn't find sprite for Tile Type: " + tile_data.Type);
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
            return;
        }
        tile_go.GetComponent<SpriteRenderer>().sprite = tileSprites[tile_data.Type];

        //Set Collider
        BoxCollider2D bc = tile_go.GetComponent<BoxCollider2D>();
        if (tile_data.Type == "WaterShallow")
        {
            bc.enabled = true;
            
        } else
        {
            bc.enabled = false;
        }
    }

    void onInstalledObjectCreated(InstalledObject obj)
    {
        GameObject obj_go = new GameObject();

        installedObjectGameObjectMap.Add(obj, obj_go);

        obj_go.name = obj.type + "_" + obj.tile.x + "_" + obj.tile.y;
        float z;
        if(obj.layer == "Background")
        {
            z = 1;
        } else
        {
            z = (float)obj.tile.y / height;
        }
        obj_go.transform.position = new Vector3(obj.tile.x, obj.tile.y, z);
        obj_go.transform.SetParent(transform, true);
        if(obj.movementCost == 0)
        {
            BoxCollider2D bc = obj_go.AddComponent<BoxCollider2D>();
            bc.offset = new Vector2(0.5f, 0.5f);
            bc.size = new Vector2(0.75f, 0.75f);
        }
        


        string spriteKey;
        if(obj.ruleTile)
        {
            spriteKey = obj.type + getRuleTileForInstalledObject(obj);
        } else if(obj.stages != 0)
        {
            spriteKey = obj.type + "_" + obj.Stage;
        } else
        {
            spriteKey = obj.type;
        }

        SpriteRenderer sr = obj_go.AddComponent<SpriteRenderer>();
        if (installedObjectSprites.ContainsKey(spriteKey) == false)
        {
            Debug.LogError("Couldn't find sprite for : " + spriteKey);
            sr.sprite = null;
            return;
        }
        sr.sprite = installedObjectSprites[spriteKey];
        sr.sortingLayerName = "InstalledObjects";

        obj.RegisterInstalledObjectChangedCB(onInstalledObjectChanged);
    }

    void onInstalledObjectRemoved(InstalledObject obj)
    {
        GameObject obj_go = installedObjectGameObjectMap[obj];
        installedObjectGameObjectMap.Remove(obj);
        Destroy(obj_go);
    }

    void onInstalledObjectChanged(InstalledObject obj)
    {
        // Get gameObject
        GameObject obj_go = installedObjectGameObjectMap[obj];
        // Update sprite
        string spriteKey;
        if (obj.ruleTile)
        {
            spriteKey = obj.type + getRuleTileForInstalledObject(obj);
        } else if (obj.stages != 0)
        {
            spriteKey = obj.type + "_" + obj.Stage;
        } else
        {
            spriteKey = obj.type;
        }

        SpriteRenderer sr = obj_go.GetComponent<SpriteRenderer>();
        if (installedObjectSprites.ContainsKey(spriteKey) == false)
        {
            Debug.LogError("Couldn't find sprite for : " + spriteKey);
            sr.sprite = null;
            return;
        }
        sr.sprite = installedObjectSprites[spriteKey];
    }

    void onLooseObjectCreated(LooseObject obj)
    {
        GameObject obj_go = new GameObject();

        looseObjectGameObjectMap.Add(obj, obj_go);

        obj_go.name = obj.stack.item.type + "_" + obj.tile.x + "_" + obj.tile.y;
        obj_go.transform.position = new Vector3(obj.tile.x, obj.tile.y, 1);
        obj_go.transform.SetParent(transform, true);
        SpriteRenderer sr = obj_go.AddComponent<SpriteRenderer>();
        sr.sprite = obj.stack.item.sprite;
        sr.sortingLayerName = "InstalledObjects";

        obj.RegisterLooseObjectChangedCB(onLooseObjectChanged);
    }

    void onLooseObjectChanged(LooseObject obj)
    {
        GameObject obj_go = looseObjectGameObjectMap[obj];

    }

    void onLooseObjectRemoved(LooseObject obj)
    {
        GameObject obj_go = looseObjectGameObjectMap[obj];
        looseObjectGameObjectMap.Remove(obj);
        Destroy(obj_go);
    }

    string getRuleTileForInstalledObject(InstalledObject obj)
    {
        string ruleTile = "_";
        int x = obj.tile.x;
        int y = obj.tile.y;
        Tile t;
        t = world.GetTileAt(x,y+1);
        if(t != null && t.installedObject != null && t.installedObject.type == obj.type)
        {
            ruleTile += "N";
        }
        t = world.GetTileAt(x + 1, y);
        if (t != null && t.installedObject != null && t.installedObject.type == obj.type)
        {
            ruleTile += "E";
        }
        t = world.GetTileAt(x, y - 1);
        if (t != null && t.installedObject != null && t.installedObject.type == obj.type)
        {
            ruleTile += "S";
        }
        t = world.GetTileAt(x - 1, y);
        if (t != null && t.installedObject != null && t.installedObject.type == obj.type)
        {
            ruleTile += "W";
        }
        if (ruleTile == "_") ruleTile += "0";
        return ruleTile;
    }
    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);
        return world.GetTileAt(x, y);
    }

    public Sprite getTileSpriteByName(string name)
    {
        if(tileSprites.ContainsKey(name) == false)
        {
            Debug.LogError("Cannot find tile sprite: " + name);
            return null;
        }
        return tileSprites[name];
    }

    public Sprite getInstalledObjectSpriteByName(string name)
    {
        if (installedObjectSprites.ContainsKey(name) == false && installedObjectSprites.ContainsKey(name + "_0") == false)
        {
            Debug.LogError("Cannot find isntalled object sprite: " + name);
            return null;
        }
        if (world.isInstalledObjectPrototypeRuleTile(name))
        {
            return installedObjectSprites[name + "_0"];
        }
        if (world.getInstalledObjectPrototypeStages(name) != 0)
        {
            return installedObjectSprites[name + "_0"];
        }
        return installedObjectSprites[name];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < randomTickSpeed; i++)
        {
            tileUpdate();
        }
    }

    void tileUpdate()
    {
        int randX = Random.Range(0, width);
        int randY = Random.Range(0, height);
        Tile tile_data = world.GetTileAt(randX, randY);
        if (tile_data.Type == "Dirt")
        {
            int grasschance = 0;
            foreach (Tile ntile_data in world.GetTileNeighbours(randX, randY))
            {
                if (ntile_data.Type == "Grass") grasschance++;
            }
            if (grasschance > Random.Range(0, 8)) tile_data.Type = "Grass";
        }
        if (tile_data.Type == "Grass")
        {
            int dirtchance = 0;
            foreach (Tile ntile_data in world.GetTileNeighbours(randX, randY))
            {
                if (ntile_data.Type == "Water") dirtchance++;
            }
            if (dirtchance > Random.Range(0, 8)) tile_data.Type = "Sand";
        }
        if(tile_data.installedObject != null)
        {
            InstalledObject obj = tile_data.installedObject;
            if(obj.type == "Plant")
            {
                if(obj.Stage < obj.stages - 1) obj.Stage++;
            }
        }
    }
}
