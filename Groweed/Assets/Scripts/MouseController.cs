using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public static MouseController Instance { get; protected set; }

    public GameObject tileCursor;
    public GameObject toolSelected;

    public ItemStack itemStackInHand;

    // CAMERA ZOOM SETTINGS

    public float minView = 5;
    public float maxView = 10;
    public float zoomSensitivity = 10;

    // Player reach
    public float reach = 5;

    public Tile tileUnderMouse;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Why are there more than one MouseController instances?");
        }
        Instance = this;
    }

    // ---------------- UPDATE
    void Update()
    {

        // Get cursor position (max distanced)
        Vector3 reachPosition = getCurrFramePos();
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Get tile under cursor and render selector

        tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(reachPosition);
        if (tileUnderMouse != null)
        {
            tileCursor.SetActive(true);
            Vector3 cursorPos = new Vector3(tileUnderMouse.x, tileUnderMouse.y, 0);
            tileCursor.transform.position = cursorPos;
        }
        else
        {
            tileCursor.SetActive(false);
        }

        // Handle Input

        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if(itemStackInHand != null)
            {
                if(LooseObject.PlaceLooseObject(tileUnderMouse,itemStackInHand))
                {
                    itemStackInHand = null;
                } else
                {
                    if (tileUnderMouse.installedObject != null)
                    {
                        InstalledObject obj = tileUnderMouse.installedObject;
                        if (obj.inventory != null)
                        {
                            ButtonMenuScript.Instance.openObjectInventory(obj);
                        }
                    }
                }
            } else
            {
                if(tileUnderMouse.looseObject != null)
                {
                    itemStackInHand = tileUnderMouse.looseObject.stack;
                    LooseObject.RemoveLooseObject(tileUnderMouse.looseObject);
                } else
                {
                    ButtonMenuScript.Instance.closeObjectInventory();
                }
                if(tileUnderMouse.installedObject != null)
                {
                    InstalledObject obj = tileUnderMouse.installedObject;
                    if(obj.inventory != null)
                    {
                        ButtonMenuScript.Instance.openObjectInventory(obj);
                    }
                }
            }
        }

        // CAMERA ZOOM

        float scale = ZoomCamera();

        // Selected Tool Position and Scaling

        ToolPos(scale, cursorPosition);
        
    }
    // ------------------ UPDATE END


    public float ZoomCamera()
    {
        float scale = Camera.main.orthographicSize;
        scale -= Input.mouseScrollDelta.y * zoomSensitivity;
        scale = Mathf.Clamp(scale, minView, maxView);
        Camera.main.orthographicSize = scale;
        return scale;
    }



    public void ToolPos(float scale, Vector3 cfp)
    {
        float toolSelectedScale = scale / 10f;

        Vector3 toolIconPos = new Vector3(cfp.x + toolSelectedScale * 0.3f, cfp.y - toolSelectedScale * 1.7f, 0);
        toolSelected.transform.position = toolIconPos;

        toolSelected.transform.localScale = new Vector3(toolSelectedScale, toolSelectedScale, toolSelectedScale);
        if (itemStackInHand == null)
        {
            toolSelected.GetComponent<SpriteRenderer>().sprite = null;
            return;
        }
        toolSelected.GetComponent<SpriteRenderer>().sprite = itemStackInHand.item.sprite;
    }



    public Vector3 getCurrFramePos()
    {
        Vector3 currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cfp = new Vector2(currFramePosition.x, currFramePosition.y);
        Vector3 mainCameraPosition = Camera.main.transform.position;
        Vector2 mcp = new Vector2(mainCameraPosition.x, mainCameraPosition.y);
        if (Vector2.Distance(cfp, mcp) > 5)
        {
            Vector2 tmp = Vector2.MoveTowards(mcp, cfp, reach);
            currFramePosition = new Vector3(tmp.x, tmp.y, 0);
        }
        return currFramePosition;
    }

    public void SetItemStackInHand(ItemStack stack)
    {
        itemStackInHand = stack;
    }

}
