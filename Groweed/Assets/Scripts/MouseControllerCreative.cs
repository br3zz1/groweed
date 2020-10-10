using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseControllerCreative : MonoBehaviour
{
    public static MouseControllerCreative Instance { get; protected set; }

    public GameObject tileCursor;
    public GameObject tileCursorBL;
    public GameObject tileCursorBR;
    public GameObject tileCursorTL;
    public GameObject tileCursorTR;
    public GameObject toolSelected;

    public Sprite bulldoze;

    public float minView = 5;
    public float maxView = 10;
    public float zoomSensitivity = 10;

    Vector3 lastFramePosition;
    Vector3 dragStart;
    bool dragStarted = false;
    string dragBuildPattern = "Fill";

    private string tool = "Select";

    private string tileType = "Dirt";
    private string objectType = "Wall";

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Why are there more than one MouseControllerCreative instances?");
        }
        Instance = this;
        SetCursorsActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        

        // ----------------------------------- Tile cursor snap to grid
        Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
        if( tileUnderMouse != null)
        {
            tileCursor.SetActive(true);
            Vector3 cursorPos = new Vector3(tileUnderMouse.x, tileUnderMouse.y, 0);
            tileCursor.transform.position = cursorPos;
        } 
        else {
            tileCursor.SetActive(false);
        }

        // ---------------------------------- Camera Panning --DISABLED FOR PLAYER MOVEMENT--
        /*if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 diff = lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);
        }*/


        if(tool != "Select")
        {
            // ---------------------------------- Select Area Thingy
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                dragStarted = true;
                dragStart = currFramePosition;
            }
            if (Input.GetMouseButton(0) && dragStarted && dragBuildPattern != "Single")
            {
                int startX = Mathf.FloorToInt(dragStart.x);
                int startY = Mathf.FloorToInt(dragStart.y);
                int endX = Mathf.FloorToInt(currFramePosition.x);
                int endY = Mathf.FloorToInt(currFramePosition.y);
                if (startX != endX || startY != endY)
                {
                    if (endX < startX)
                    {
                        int tmp = endX;
                        endX = startX;
                        startX = tmp;
                    }
                    if (endY < startY)
                    {
                        int tmp = endY;
                        endY = startY;
                        startY = tmp;
                    }
                    tileCursorBL.transform.position = new Vector3(startX, startY, 0);
                    tileCursorBR.transform.position = new Vector3(endX, startY, 0);
                    tileCursorTL.transform.position = new Vector3(startX, endY, 0);
                    tileCursorTR.transform.position = new Vector3(endX, endY, 0);
                    SetCursorsActive(true);
                }
                else
                {
                    SetCursorsActive(false);
                }
            }
            if (Input.GetMouseButtonUp(0) && dragStarted)
            {
                if (dragBuildPattern == "Single")
                {
                    Tile t = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
                    WorldController.Instance.world.PlaceInstalledObject(objectType, t);
                    dragStarted = false;
                }
                else
                {
                    SetCursorsActive(false);
                    int startX = Mathf.FloorToInt(dragStart.x);
                    int startY = Mathf.FloorToInt(dragStart.y);
                    int endX = Mathf.FloorToInt(currFramePosition.x);
                    int endY = Mathf.FloorToInt(currFramePosition.y);
                    if (endX < startX)
                    {
                        int tmp = endX;
                        endX = startX;
                        startX = tmp;
                    }
                    if (endY < startY)
                    {
                        int tmp = endY;
                        endY = startY;
                        startY = tmp;
                    }

                    for (int x = startX; x <= endX; x++)
                    {
                        for (int y = startY; y <= endY; y++)
                        {
                            Tile t = WorldController.Instance.world.GetTileAt(x, y);
                            if (t != null)
                            {
                                if (tool == "BuildTile")
                                {
                                    t.Type = tileType;
                                }
                                else if (tool == "BuildInstalled")
                                {
                                    if (objectType == "Remove")
                                    {
                                        WorldController.Instance.world.RemoveInstalledObject(t);
                                    }
                                    else
                                    {
                                        if (dragBuildPattern == "Fill_Hollow")
                                        {
                                            if (x == startX || x == endX || y == startY || y == endY)
                                            {
                                                if (t.Type != "Water")
                                                {
                                                    WorldController.Instance.world.PlaceInstalledObject(objectType, t);
                                                }
                                            }

                                        }
                                        else if (dragBuildPattern == "Fill")
                                        {
                                            if (t.Type != "Water")
                                            {
                                                WorldController.Instance.world.PlaceInstalledObject(objectType, t);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    dragStarted = false;
                }
            }
        }
        

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // CAMERA ZOOM

        float scale = Camera.main.orthographicSize;
        scale -= Input.mouseScrollDelta.y * zoomSensitivity;
        scale = Mathf.Clamp(scale, minView, maxView);
        Camera.main.orthographicSize = scale;

        // Selected Tool Position and Scaling

        float toolSelectedScale = scale / 10f;
        
        Vector3 toolIconPos = new Vector3(currFramePosition.x + toolSelectedScale*0.3f, currFramePosition.y - toolSelectedScale*1.7f, 0);
        toolSelected.transform.position = toolIconPos;
        
        toolSelected.transform.localScale = new Vector3(toolSelectedScale, toolSelectedScale, toolSelectedScale);
        

    }

    void SetCursorsActive(bool ac)
    {
        tileCursorBL.SetActive(ac);
        tileCursorBR.SetActive(ac);
        tileCursorTL.SetActive(ac);
        tileCursorTR.SetActive(ac);
        tileCursor.SetActive(!ac);
    }

    public void SetTerraformTile(string tile)
    {
        tool = "BuildTile";
        tileType = tile;
        dragBuildPattern = "Fill";
        
        toolSelected.GetComponent<SpriteRenderer>().sprite = WorldController.Instance.getTileSpriteByName(tile);
    }

    public void SetInstalledObject(string obj)
    {
        tool = "BuildInstalled";
        objectType = obj;
        if(obj == "Remove")
        {
            toolSelected.GetComponent<SpriteRenderer>().sprite = bulldoze;
            dragBuildPattern = "Fill";
            return;
        }
        dragBuildPattern = WorldController.Instance.world.getInstalledObjectPrototypeDragBuildPattern(objectType);
        toolSelected.GetComponent<SpriteRenderer>().sprite = WorldController.Instance.getInstalledObjectSpriteByName(obj);
    }

    public void SetSelect()
    {
        tool = "Select";
        toolSelected.GetComponent<SpriteRenderer>().sprite = null;
    }
}
