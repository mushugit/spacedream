using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Tile;
using Color = UnityEngine.Color;

public class MouseController : MonoBehaviour
{
    public enum MouseButton
    {
        MouseLeft = 0,
        MouseRight = 1,
        MouseMiddle = 2
    }


    private Vector3 lastMousePosition;

    private Vector3 dragStartMousePosition;

    [SerializeField]
    private Camera mouseControlledCamera = default;

    private const float OrthographicZoomMin = 8f;
    private const float OrthographicZoomMax = 33f;

    private IGameController _game;

    [Header("Mouse Button config")]
    [SerializeField]
    private MouseButton dragDropButton = MouseButton.MouseLeft;
    [SerializeField]
    private MouseButton moveCameraButton = MouseButton.MouseRight;
    [SerializeField]
    private MouseButton moveCameraButtonAlt = MouseButton.MouseMiddle;
    [SerializeField]
    private MouseButton moveCharacterButton = MouseButton.MouseLeft;

    private int DragDropButton { get { return (int)dragDropButton; } }
    private int MoveCameraButton { get { return (int)moveCameraButton; } }
    private int MoveCameraButtonAlt { get { return (int)moveCameraButtonAlt; } }
    private int MoveCharacterButton { get { return (int)moveCharacterButton; } }

    [Header("Keyboard config")]
    [SerializeField]
    private KeyCode destroyModifier = KeyCode.LeftControl;
    [SerializeField]
    private KeyCode destroyModifierAlt = KeyCode.RightControl;
    [SerializeField]
    private KeyCode moveCharacterModifier = KeyCode.LeftShift;
    [SerializeField]
    private KeyCode moveCharacterModifierAlt = KeyCode.RightShift;

    [Header("Other configuration")]
    [SerializeField]
    private float zoomSpeed = 2f;
    [SerializeField]
    private GameObject cursorPrefab = default;
    [SerializeField]
    private Transform boxSelectorParent = default;

    private const float _defaultZ = -10f;
    private readonly Vector2 _tileCenter = new Vector2(.5f, .5f);

    private List<GameObject> dragCursors;

    private readonly Color destroyDragDrogTint = new Color(0.7764706f, 0.2117647f, 0.2117647f);

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
        mouseControlledCamera.transform.position = new Vector3(worldCenter.X, worldCenter.Y, _defaultZ);
    }

    private bool DestroyModifier()
    {
        return Input.GetKey(destroyModifier) || Input.GetKey(destroyModifierAlt);
    }

    private bool MoveCharacterModifier()
    {
        return Input.GetKey(moveCharacterModifier) || Input.GetKey(moveCharacterModifierAlt);
    }

    // Update is called once per frame
    void Update()
    {
        var currentMousePosition = mouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
        var currentZoom = mouseControlledCamera.orthographicSize;

        //Cursor
        SetCursorPosition(currentMousePosition);

        var destroyModifier = DestroyModifier();
        var moveCharacterModifier = MoveCharacterModifier(); ;

        if (Input.GetMouseButtonDown(DragDropButton))
        {
            if (!moveCharacterModifier)
            {
                //Start drag
                if (dragStartMousePosition == Vector3.zero)
                {
                    dragStartMousePosition = mouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
                    DrawCursorDrag(dragStartMousePosition, dragStartMousePosition, destroyModifier);
                }
            }
        }

        if (Input.GetMouseButtonDown(MoveCharacterButton))
        {
            if (moveCharacterModifier)
            {
                //Order move
                Vector2 currentMousePosition2D = currentMousePosition;
                currentMousePosition2D -= _tileCenter;
                _game.CharacterController.ForceMove(currentMousePosition2D.ToPoint());
            }
        }

        //Continue drag
        if (Input.GetMouseButton(DragDropButton) && dragStartMousePosition != Vector3.zero)
        {
            if (!moveCharacterModifier)
            {
                var dragEndMousePosition = mouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
                DrawCursorDrag(dragStartMousePosition, dragEndMousePosition, destroyModifier);
            }
        }

        //End drag
        if (Input.GetMouseButtonUp(DragDropButton) && dragStartMousePosition != Vector3.zero)
        {
            if (!moveCharacterModifier)
            {
                var dragEndMousePosition = mouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
                HideCursorDrag();
                DoCursorDrag(dragStartMousePosition, dragEndMousePosition, destroyModifier);

                dragStartMousePosition = Vector3.zero;
            }
        }

        if (Input.GetMouseButton(MoveCameraButton) || Input.GetMouseButton(MoveCameraButtonAlt))
        {
            var mouseMovement = lastMousePosition - currentMousePosition;
            //TODO: Delegate camera handling to Camera controller
            mouseControlledCamera.transform.Translate(mouseMovement);
        }

        var scrollZoomDelta = Input.mouseScrollDelta.y;

        mouseControlledCamera.orthographicSize = Mathf.Clamp(currentZoom + (-scrollZoomDelta * zoomSpeed), OrthographicZoomMin, OrthographicZoomMax);

        lastMousePosition = mouseControlledCamera.ScreenToWorldPoint(Input.mousePosition);
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
                _game.CreateBuildJob(new Point(x, startY), TileContentType.Wall, TileContentType.WallTemplate, Size.Empty);
                _game.CreateBuildJob(new Point(x, endY), TileContentType.Wall, TileContentType.WallTemplate, Size.Empty);
            }
            for (var y = startY; y <= endY; y++)
            {
                _game.CreateBuildJob(new Point(startX, y), TileContentType.Wall, TileContentType.WallTemplate, Size.Empty);
                _game.CreateBuildJob(new Point(endX, y), TileContentType.Wall, TileContentType.WallTemplate, Size.Empty);
            }
            for (var x = startX + 1; x <= endX - 1; x++)
            {
                for (var y = startY + 1; y <= endY - 1; y++)
                {
                    _game.CreateBuildJob(new Point(x, y), TileContentType.None, TileContentType.None, Size.Empty);
                }
            }
        }
        else
        {
            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    _game.CreateDestroyJob(new Point(x, y), TileContentType.None, Size.Empty);
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
            color = destroyDragDrogTint;
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