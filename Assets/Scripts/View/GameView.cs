using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Tile;
using Color = UnityEngine.Color;

public class GameView : MonoBehaviour, IGameWaiter, IGameView
{
    private IGameController _game;

    [Header("Main sprites")]
    [SerializeField]
    public Sprite floorSprite = default;

    [Header("Template sprites")]
    [SerializeField]
    private Sprite floorTemplateSprite = default;
    [SerializeField]
    private Sprite wallTemplateSprite = default;

    [Header("Parent transform main")]
    [SerializeField]
    private Transform floorsParent = default;

    [Header("Parent transform templates")]
    [SerializeField]
    private Transform floorsTemplateParent = default;
    [SerializeField]
    private Transform wallsTemplateParent = default;

    private GameObject[,] GameObjectsReferences;

    private IWallView _wallView;

    void Awake()
    {
        _game = GetComponentInParent<GameController>() as IGameController;
    }

    public void RegisterWallView(IWallView wallView)
    {
        _wallView = wallView;
    }

    // Start is called before the first frame update
    void Start()
    {
        WaitForGame(_game);
    }

    public void WaitForGame(IGameController game)
    {
        if (game == null)
        {
            Debug.LogError($"Game not initialized");
            Application.Quit();
        }
        else
        {
            StartCoroutine(WaitForGamePlaying());
        }
    }

    public IEnumerator WaitForGamePlaying()
    {
        yield return new WaitUntil(() => _game.Playing);

        var mapSize = _game.GetMapSize();
        GameObjectsReferences = new GameObject[mapSize.Width, mapSize.Height];
        RenderMap(mapSize);
    }

    private void RenderMap(Size mapSize)
    {
        var stopwatch = new System.Diagnostics.Stopwatch();

        Debug.Log($"Rendering map of size {mapSize}");

        stopwatch.Start();

        //Render content
        for (var x = 0; x < mapSize.Width; x++)
        {
            for (var y = 0; y < mapSize.Height; y++)
            {
                var coord = new Point(x, y);
                RenderTile(coord);
            }
        }

        // Register callbacks
        _game.SubscribeAllTileTypeChanged(OnTileTypeChanged);
        _game.SubscribeAllTileContentChanged(OnTileContentChanged);
        stopwatch.Stop();

        Debug.Log($"Map rendered in {stopwatch.Elapsed}");
    }

    public void SubscribeSpecificContentTypeChanged(TileContentType contentType, EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler)
    {
        _game.SubscribeSpecificTileContentChanged(contentType, tileContentChangedEventHandler);
    }

    public ITileView GetTileAt(Point coord)
    {
        return _game.GetTileAt(coord);
    }

    public List<(Point coord, ITileView tile)> GetNeighbours(Point coord)
    {
        return _game.GetNeighbours(coord);
    }

    private void RenderTile(Point coord)
    {
        var tileData = _game.GetTileAt(coord);
        //Create content
        var worldPosition = new Vector3(coord.X + 0.5f, coord.Y + .5f, 1f);
        GameObject gameObject;

        //TODO:Get sprite from type dynamically
        switch (tileData.Content)
        {
            case TileContentType.Wall:
                //Only for initial render
                _wallView.RenderTile(coord);
                break;
            case TileContentType.WallTemplate:
                gameObject = Instantiate(wallTemplateSprite, coord, worldPosition, Color.white);
                gameObject.transform.parent = wallsTemplateParent;
                break;
            case TileContentType.None:
                switch (tileData.Type)
                {
                    case TileType.Floor:
                        gameObject = Instantiate(floorSprite, coord, worldPosition, Color.white);
                        gameObject.transform.parent = floorsParent;
                        break;
                    case TileType.FloorTemplate:
                        gameObject = Instantiate(floorTemplateSprite, coord, worldPosition, Color.white);
                        gameObject.transform.parent = floorsTemplateParent;
                        break;
                    case TileType.Space:
                        gameObject = Instantiate(null, coord, worldPosition, Color.white);
                        gameObject.transform.parent = floorsParent;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    private void OnTileTypeChanged(object sender, TileTypeChangedEventArgs e)
    {
        RemoveGameObjectAt(e.TileCoord);
        RenderTile(e.TileCoord);
    }

    public void OnTileContentChanged(object sender, TileContentChangedEventArgs e)
    {
        if (e.OldContent != TileContentType.Wall && e.NewContent != TileContentType.Wall)
        {
            RemoveGameObjectAt(e.TileCoord);
            RenderTile(e.TileCoord);
        }
    }

    //TODO move to helper
    private GameObject Instantiate(Sprite sprite, Point point, Vector2 position, Color color)
    {
        var gameObject = new GameObject($"Tile_{point.X}_{point.Y}");

        //Position
        gameObject.transform.position = position;

        //Sprite
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        if (color != Color.white)
        {
            spriteRenderer.color = color;
        }

        AddGameobjectReference(point, gameObject);
        return gameObject;
    }

    public void AddGameobjectReference(Point coord, GameObject gameobject)
    {
        GameObjectsReferences[coord.X, coord.Y] = gameobject;
    }

    public void RemoveGameObjectAt(Point point)
    {
        var gameObject = GameObjectsReferences[point.X, point.Y];
        GameObjectsReferences[point.X, point.Y] = null;
        Destroy(gameObject);
    }
}
