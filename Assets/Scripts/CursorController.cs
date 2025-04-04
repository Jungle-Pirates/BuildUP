using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private Vector2Int cursorCoordinate = Vector2Int.zero;

    [Header("Cursor Sprites")]
    [SerializeField] private GameObject ableSprite;
    [SerializeField] private GameObject placableSprite;
    [SerializeField] private GameObject unableSprite;

    private void Update()
    {
        if (BuildManager.Instance.IsBuildMode)
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
        else if (!BuildManager.Instance.IsBuildMode && !BuildManager.Instance.IsPlaceMode && (ableSprite.activeSelf || unableSprite.activeSelf || placableSprite.activeSelf))
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
                if (BuildManager.Instance.CanBuildRoom(cursorCoordinate) && (!ableSprite.activeSelf || unableSprite.activeSelf))
                {
                    ableSprite.SetActive(true);
                    placableSprite.SetActive(false);
                    unableSprite.SetActive(false);
                }
                else if (!BuildManager.Instance.CanBuildRoom(cursorCoordinate) && (!unableSprite.activeSelf || ableSprite.activeSelf))
                {
                    ableSprite.SetActive(false);
                    placableSprite.SetActive(false);
                    unableSprite.SetActive(true);
                }
            }
            else if (BuildManager.Instance.IsPlaceMode)
            {
                if (BuildManager.Instance.CanBuildLadder(cursorCoordinate) && (!ableSprite.activeSelf || unableSprite.activeSelf))
                {
                    placableSprite.SetActive(true);
                    ableSprite.SetActive(false);
                    unableSprite.SetActive(false);
                }
                else if (!BuildManager.Instance.CanBuildLadder(cursorCoordinate) && (!unableSprite.activeSelf || ableSprite.activeSelf))
                {
                    placableSprite.SetActive(false);
                    ableSprite.SetActive(false);
                    unableSprite.SetActive(true);
                }
            }
        }
        else
        {
            ableSprite.SetActive(false);
            placableSprite.SetActive(false);
            unableSprite.SetActive(false);
        }
    }
}
