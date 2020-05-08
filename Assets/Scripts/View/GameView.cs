using Assets.Scripts.Controllers;
using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.View.Interfaces;
using System.Collections;
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
    [SerializeField]
    public Sprite wallSprite = default;
    [SerializeField]
    public Sprite doorSprite = default;

    [Header("Template sprites")]
    [SerializeField]
    private Sprite floorTemplateSprite = default;
    [SerializeField]
    private Sprite wallTemplateSprite = default;
    [SerializeField]
    private Sprite doorTemplateSprite = default;

    [Header("Parent transform main")]
    [SerializeField]
    private Transform floorsParent = default;
    [SerializeField]
    private Transform wallsParent = default;
    [SerializeField]
    private Transform doorsParent = default;

    [Header("Parent transform templates")]
    [SerializeField]
    private Transform floorsTemplateParent = default;
    [SerializeField]
    private Transform wallsTemplateParent = default;
    [SerializeField]
    private Transform doorsTemplateParent = default;

    private GameObject[,] GameObjectsReferences;

    private readonly Color wallColor = new Color(0.2300641f, 622777f, 0.8867924f);

    void Awake()
    {
        _game = GetComponentInParent<GameController>() as IGameController;
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
                gameObject = Instantiate(wallSprite, coord, worldPosition, wallColor);
                gameObject.transform.parent = wallsParent;
                break;
            case TileContentType.WallTemplate:
                gameObject = Instantiate(wallTemplateSprite, coord, worldPosition, Color.white);
                gameObject.transform.parent = wallsTemplateParent;
                break;
            case TileContentType.Door:
                gameObject = Instantiate(doorSprite, coord, worldPosition, wallColor);
                gameObject.transform.parent = doorsParent;
                break;
            case TileContentType.DoorTemplate:
                gameObject = Instantiate(doorTemplateSprite, coord, worldPosition, Color.white);
                gameObject.transform.parent = doorsTemplateParent;
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

    private void OnTileContentChanged(object sender, TileContentChangedEventArgs e)
    {
        RemoveGameObjectAt(e.TileCoord);
        RenderTile(e.TileCoord);
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

        GameObjectsReferences[point.X, point.Y] = gameObject;

        return gameObject;
    }

    private void RemoveGameObjectAt(Point point)
    {
        var gameObject = GameObjectsReferences[point.X, point.Y];
        GameObjectsReferences[point.X, point.Y] = null;
        Destroy(gameObject);
    }
}
