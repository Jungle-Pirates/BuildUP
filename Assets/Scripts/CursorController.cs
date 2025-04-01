using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private Vector2Int cursorCoordinate = Vector2Int.zero;

    [Header("Cursor Sprites")]
    [SerializeField] private GameObject ableSprite;
    [SerializeField] private GameObject unableSprite;

    private bool isBuildMode = false;

    private void Update()
    {
        //임시로 'B'를 누르면 건설모드로 들어가도록
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuildMode = !isBuildMode;
        }

        if (isBuildMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                BuildManager.Instance.CmdBuildRoom(cursorCoordinate);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                BuildManager.Instance.CmdDeleteRoom(cursorCoordinate);
            }

            DrawCursor();
        }
        else if (!isBuildMode && (ableSprite.activeSelf || unableSprite.activeSelf))
        {
            SetCursorSprite(false);
        }
    }

    public void SetCursorSize(Vector2 size)
    {
        transform.localScale = size;
    }

    private void DrawCursor()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (cursorCoordinate != BuildManager.Instance.ToBasisIntCoordinates(mousePos))
        {
            cursorCoordinate = BuildManager.Instance.ToBasisIntCoordinates(mousePos);
            SetCursorSprite(true);
        }

        transform.position = BuildManager.Instance.FromBasisIntCoordinates(cursorCoordinate);
    }

    private void SetCursorSprite(bool willDrawSprite)
    {
        if (willDrawSprite)
        {
            if (BuildManager.Instance.CanBuildRoom(cursorCoordinate) && (!ableSprite.activeSelf || unableSprite.activeSelf))
            {
                ableSprite.SetActive(true);
                unableSprite.SetActive(false);
            }
            else if (!BuildManager.Instance.CanBuildRoom(cursorCoordinate) && (!unableSprite.activeSelf || ableSprite.activeSelf))
            {
                ableSprite.SetActive(false);
                unableSprite.SetActive(true);
            }
        }
        else
        {
            ableSprite.SetActive(false);
            unableSprite.SetActive(false);
        }
    }
}
