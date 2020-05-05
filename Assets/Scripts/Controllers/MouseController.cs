﻿using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Tile;
using Color = UnityEngine.Color;

public class MouseController : MonoBehaviour
{
    private Vector3 lastMousePosition;

    private Vector3 dragStartMousePosition;

    public Camera MouseControlledCamera;

    private const float OrthographicZoomMin = 8f;
    private const float OrthographicZoomMax = 33f;

    private IGameController _game;

    public float ZoomSpeed = 2f;
    public GameObject cursorPrefab;
    public Transform boxSelectorParent;

    private const float _defaultZ = -10f;

    private List<GameObject> dragCursors;

    private readonly Color controlColor = new Color(0.7764706f, 0.2117647f, 0.2117647f);

    void Awake()
    {
        _game = GetComponentInParent<GameController>() as IGameController;

        SetCursorPosition(Vector3.zero);

        dragCursors = new List<GameObject>();
        dragStartMousePosition = Vector3.zero;
    }

    private void SetCursorPosition(Vector3 position)
    {
        position.z = 0;

        position.x = Mathf.RoundToInt(position.x + .5f) - .5f;
        position.y = Mathf.RoundToInt(position.y + .5f) - .5f;
    }

    // Start is called before the first frame update
    void Start()
    {
        var worldCenter = _game.WorldCenter;
        MouseControlledCamera.transform.position = new Vector3(worldCenter.X, worldCenter.Y, _defaultZ);
    }

    // Update is called once per frame
    void Update()
    {

        //Mouse 0 : left, Mouse 1 : right, Mouse 2 : middle

        var currentMousePosition = MouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
        var currentZoom = MouseControlledCamera.orthographicSize;

        //Cursor
        SetCursorPosition(currentMousePosition);

        var control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (Input.GetMouseButtonDown(0))
        {
            //Start drag
            if (dragStartMousePosition == Vector3.zero)
            {
                dragStartMousePosition = MouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
                DrawCursorDrag(dragStartMousePosition, dragStartMousePosition, control);
            }
        }

        //Continue drag
        if (Input.GetMouseButton(0) && dragStartMousePosition != Vector3.zero)
        {
            var dragEndMousePosition = MouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
            DrawCursorDrag(dragStartMousePosition, dragEndMousePosition, control);
        }

        //End drag
        if (Input.GetMouseButtonUp(0) && dragStartMousePosition != Vector3.zero)
        {
            var dragEndMousePosition = MouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
            HideCursorDrag();
            DoCursorDrag(dragStartMousePosition, dragEndMousePosition, control);

            dragStartMousePosition = Vector3.zero;
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            var mouseMovement = lastMousePosition - currentMousePosition;
            //TODO: Delegate camera handling to Camera controller
            MouseControlledCamera.transform.Translate(mouseMovement);
        }

        var scrollZoomDelta = Input.mouseScrollDelta.y;

        MouseControlledCamera.orthographicSize = Mathf.Clamp(currentZoom + (-scrollZoomDelta * ZoomSpeed), OrthographicZoomMin, OrthographicZoomMax);

        lastMousePosition = MouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void HideCursorDrag()
    {
        foreach (var go in dragCursors)
        {
            Destroy(go);
        }

        dragCursors.Clear();
    }

    private void DoCursorDrag(Vector3 start, Vector3 end, bool control)
    {
        var startX = Mathf.RoundToInt(start.x - .5f);
        var endX = Mathf.RoundToInt(end.x - .5f);
        if (startX > endX)
        {
            var temp = startX;
            startX = endX;
            endX = temp;
        }
        var startY = Mathf.RoundToInt(start.y - .5f);
        var endY = Mathf.RoundToInt(end.y - .5f);
        if (startY > endY)
        {
            var temp = startY;
            startY = endY;
            endY = temp;
        }

        if (!control)
        {
            for (var x = startX; x <= endX; x++)
            {
                _game.Build(new Point(x, startY), TileContentType.Wall, Size.Empty);
                _game.Build(new Point(x, endY), TileContentType.Wall, Size.Empty);
            }
            for (var y = startY; y <= endY; y++)
            {
                _game.Build(new Point(startX, y), TileContentType.Wall, Size.Empty);
                _game.Build(new Point(endX, y), TileContentType.Wall, Size.Empty);
            }
            for (var x = startX + 1; x <= endX - 1; x++)
            {
                for (var y = startY + 1; y <= endY - 1; y++)
                {
                    _game.Build(new Point(x, y), TileContentType.None, Size.Empty);
                }
            }
        }
        else
        {
            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    _game.Destroy(new Point(x, y), TileContentType.None, Size.Empty);
                }
            }
        }
    }

    private void DrawCursorDrag(Vector3 start, Vector3 end, bool control)
    {
        HideCursorDrag();

        var startX = Mathf.RoundToInt(start.x + .5f) - .5f;
        var endX = Mathf.RoundToInt(end.x + .5f) - .5f;
        if (startX > endX)
        {
            var temp = startX;
            startX = endX;
            endX = temp;
        }
        var startY = Mathf.RoundToInt(start.y + .5f) - .5f;
        var endY = Mathf.RoundToInt(end.y + .5f) - .5f;
        if (startY > endY)
        {
            var temp = startY;
            startY = endY;
            endY = temp;
        }

        var color = Color.white;
        if (control)
        {
            color = controlColor;
        }
        for (var x = startX; x <= endX; x++)
        {
            DrawElementOfDrag(x, startY, color);
            DrawElementOfDrag(x, endY, color);
        }
        for (var y = startY; y <= endY; y++)
        {
            DrawElementOfDrag(startX, y, color);
            DrawElementOfDrag(endX, y, color);
        }
    }

    private void DrawElementOfDrag(float x, float y, Color color)
    {
        var position = new Vector3(x, y, _defaultZ + 1);
        var go = Instantiate(cursorPrefab, position, Quaternion.identity);
        go.name = $"Cursor_{x}_{y}";
        if (color != Color.white)
        {
            var renderer = go.GetComponent<SpriteRenderer>();
            renderer.color = color;
        }
        go.transform.parent = boxSelectorParent;
        dragCursors.Add(go);
    }
}