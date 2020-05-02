using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private Vector3 lastMousePosition;

    public Camera MouseControlledCamera;

    private const float OrthographicZoomMin = 8f;
    private const float OrthographicZoomMax = 33f;

    private IGameController _game;

    public float ZoomSpeed = 2f;
    public GameObject cursorPrefab;

    private const float _defaultZ = -10f;
    private GameObject _cursor;

    void Awake()
    {
        _game = GetComponentInParent<GameController>() as IGameController;

        _cursor = Instantiate(cursorPrefab);
        SetCursorPosition(Vector3.zero);
    }

    private void SetCursorPosition(Vector3 position)
    {
        position.z = 0;

        position.x = Mathf.RoundToInt(position.x+.5f) - .5f;
        position.y = Mathf.RoundToInt(position.y + .5f) - .5f;

        _cursor.transform.position = position;
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
        var currentMousePosition = MouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
        var currentZoom = MouseControlledCamera.orthographicSize;

        //Cursor
        SetCursorPosition(currentMousePosition);

        //Mouse 0 : left, Mouse 1 : right, Mouse 2 : middle
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
}