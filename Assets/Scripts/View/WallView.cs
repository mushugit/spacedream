using Assets.Scripts.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Tile;
using Color = UnityEngine.Color;

public class WallView : MonoBehaviour, IWallView
{
    [SerializeField]
    private Transform wallsParent = default;

    private IGameView _gameView;

    private readonly Color wallColor = new Color(0.2300641f, 622777f, 0.8867924f);

    private Dictionary<string, Sprite> wallSprites;

    private void Awake()
    {
        _gameView = GetComponentInParent<GameView>();
        _gameView.RegisterWallView(this);

        LoadSprites();
    }

    private void Start()
    {
        _gameView.SubscribeSpecificContentTypeChanged(TileContentType.Wall, OnTileContentChanged);
    }

    private void LoadSprites()
    {
        //TODO move to resources manager
        var rawWallSprites = Resources.LoadAll<Sprite>("Tiles/Walls");
        wallSprites = new Dictionary<string, Sprite>(rawWallSprites.Length);

        foreach (var sprite in rawWallSprites)
        {
            var splitedName = sprite.name.Split('_');
            if (splitedName.Length == 2)
            {
                wallSprites.Add(splitedName[1], sprite);
            }
        }
    }

    private void OnTileContentChanged(object sender, TileContentChangedEventArgs e)
    {
        if (e.NewContent != TileContentType.Wall)
        {
            _gameView.OnTileContentChanged(sender, e);
            var neighbours = _gameView.GetNeighbours(e.TileCoord);
            foreach (var neighbour in neighbours)
            {
                if (neighbour.tile.Content == TileContentType.Wall)
                {
                    _gameView.RemoveGameObjectAt(neighbour.coord);
                    RenderTile(neighbour.coord);
                }
            }
        }
        else
        {
            _gameView.RemoveGameObjectAt(e.TileCoord);
            RenderTile(e.TileCoord, true);
        }
    }

    public void RenderTile(Point coord, bool cascadeUpdateRender = false)
    {
        var neighbours = _gameView.GetNeighbours(coord);
        var wallDirectionFlag = 0;

        var pointToTheNorth = coord.GetPointToDirection(PointExtension.Direction.North);
        var pointToTheEast = coord.GetPointToDirection(PointExtension.Direction.East);
        var pointToTheSouth = coord.GetPointToDirection(PointExtension.Direction.South);
        var pointToTheWest = coord.GetPointToDirection(PointExtension.Direction.West);

        foreach (var neighbour in neighbours)
        {
            if (neighbour.tile.Content == TileContentType.Wall)
            {
                if (pointToTheNorth == neighbour.coord)
                {
                    wallDirectionFlag += 8;
                }
                if (pointToTheEast == neighbour.coord)
                {
                    wallDirectionFlag += 4;
                }
                if (pointToTheSouth == neighbour.coord)
                {
                    wallDirectionFlag += 2;
                }
                if (pointToTheWest == neighbour.coord)
                {
                    wallDirectionFlag += 1;
                }
                if (cascadeUpdateRender)
                {
                    _gameView.RemoveGameObjectAt(neighbour.coord);
                    RenderTile(neighbour.coord);
                }
            }
        }
        var binaryKey = Convert.ToString(wallDirectionFlag, 2).PadLeft(4, '0');
        Sprite sprite = null;
        if (wallSprites != null && wallSprites.ContainsKey(binaryKey))
        {
            sprite = wallSprites[binaryKey];
        }
        else
        {
            Debug.Log($"Error finding WALL sprite with key {binaryKey}");
        }

        //Create content
        var worldPosition = new Vector3(coord.X + 0.5f, coord.Y + .5f, 1f);
        var gameObject = Instantiate(sprite, coord, worldPosition, wallColor);
        gameObject.transform.parent = wallsParent;
    }

    //TODO Helper
    private GameObject Instantiate(Sprite sprite, Point point, Vector2 position, Color color)
    {
        var gameObject = new GameObject($"Wall_{point.X}_{point.Y}");

        //Position
        gameObject.transform.position = position;

        //Sprite
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        if (color != Color.white)
        {
            spriteRenderer.color = color;
        }

        _gameView.AddGameobjectReference(point, gameObject);

        return gameObject;
    }
}
