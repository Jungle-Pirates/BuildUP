using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private Vector2Int cursorCoordinate = Vector2Int.zero;

    [Header("Cursor Sprites")]
    [SerializeField] private GameObject buildableSprite;
    [SerializeField] private GameObject placableSprite;
    [SerializeField] private GameObject deletionSprite;
    [SerializeField] private GameObject unableSprite;

    private void Update()
    {
        if (BuildManager.Instance.IsBuildMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                BuildManager.Instance.BuildOrUpgradeRoom(cursorCoordinate);
            }

            DrawCursor();
        }
        if (BuildManager.Instance.IsPlaceMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 일단 사다리만
                BuildManager.Instance.CmdBuildNonRoom(cursorCoordinate, NonRoomType.ladder);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                BuildManager.Instance.CmdBuildNonRoom(cursorCoordinate, NonRoomType.pipe);
            }

            DrawCursor();
        }
        if (BuildManager.Instance.IsDestructionMode)
        {
            if (Input.GetMouseButtonDown(1))
            {
                BuildManager.Instance.CmdDeleteRoom(cursorCoordinate);
            }

            DrawCursor();
        }
        else if (!BuildManager.Instance.IsBuildMode && !BuildManager.Instance.IsPlaceMode && (buildableSprite.activeSelf || unableSprite.activeSelf || placableSprite.activeSelf))
        {
            // 모드가 아닐 때 커서 비활성화
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
            if (BuildManager.Instance.IsBuildMode)
            {
                placableSprite.SetActive(false);
                deletionSprite.SetActive(false);
                if (BuildManager.Instance.CanBuildRoom(cursorCoordinate))
                {
                    buildableSprite.SetActive(true);
                    unableSprite.SetActive(false);
                }
                else
                {
                    buildableSprite.SetActive(false);
                    unableSprite.SetActive(true);
                }
            }
            else if (BuildManager.Instance.IsPlaceMode)
            {
                buildableSprite.SetActive(false);
                deletionSprite.SetActive(false);
                if (BuildManager.Instance.CanBuildLadder(cursorCoordinate))
                {
                    placableSprite.SetActive(true);
                    unableSprite.SetActive(false);
                }
                else
                {
                    placableSprite.SetActive(false);
                    unableSprite.SetActive(true);
                }
            }
            else if (BuildManager.Instance.IsDestructionMode)
            {
                buildableSprite.SetActive(false);
                placableSprite.SetActive(false);
                if (BuildManager.Instance.CheckRoomExistance(cursorCoordinate))
                {
                    deletionSprite.SetActive(true);
                    unableSprite.SetActive(false);
                }
                else
                {
                    deletionSprite.SetActive(false);
                    unableSprite.SetActive(true);
                }
            }
        }
        else
        {
            buildableSprite.SetActive(false);
            placableSprite.SetActive(false);
            unableSprite.SetActive(false);
            deletionSprite.SetActive(false);
        }
    }
}
