using UnityEngine;
using System.Collections;
using Assets.Scripts.View.Interfaces;
using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.Controllers;
using System.Drawing;
using static Tile;

public class GameView : MonoBehaviour, IGameWaiter, IGameView
{
    private IGameController _game;

    public Sprite floorSprite;
    public Sprite wallSprite;
    public Sprite doorSprite;

    public Transform floorsParent;
    public Transform wallsParent;
    public Transform doorsParent;

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
        RenderMap(_game.GetMapSize());
    }

    // Update is called once per frame
    void Update()
    {

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
        GameObject gameObject = null;

        //TODO:Get sprite from type dynamically
        switch (tileData.Content)
        {
            case TileContentType.Wall:
                gameObject = Instantiate(wallSprite, coord, worldPosition);
                gameObject.transform.parent = wallsParent;
                break;
            case TileContentType.Door:
                gameObject = Instantiate(doorSprite, coord, worldPosition);
                gameObject.transform.parent = doorsParent;
                break;
            case TileContentType.None:
                switch (tileData.Type)
                {
                    case TileType.Floor:
                        gameObject = Instantiate(floorSprite, coord, worldPosition);
                        break;
                    case TileType.Space:
                        gameObject = Instantiate(null, coord, worldPosition);
                        break;
                    default:
                        break;
                }
                gameObject.transform.parent = floorsParent;
                break;
            default:
                break;
        }
    }

    private void OnTileTypeChanged(object sender, TileTypeChangedEventArgs e)
    {
       RenderTile(e.TileCoord);
    }

    private void OnTileContentChanged(object sender, TileContentChangedEventArgs e)
    {
        RenderTile(e.TileCoord);
    }

    private GameObject Instantiate(Sprite sprite, Point point, Vector2 position)
    {
        var gameObject = new GameObject($"Tile_{point.X}_{point.Y}");

        //Position
        gameObject.transform.position = position;

        //Sprite
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

        return gameObject;
    }
}
