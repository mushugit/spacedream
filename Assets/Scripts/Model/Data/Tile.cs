﻿using Assets.Scripts.Model.Interfaces.Data;
using System;
using System.Drawing;

public class Tile : IBuildableTile
{
    #region Events
    public class TileTypeChangedEventArgs : EventArgs
    {
        public Point TileCoord { get; set; }
        public TileType OldTileType { get; set; }
        public TileType NewTileType { get; set; }
    }
    public class TileContentChangedEventArgs : EventArgs
    {
        public Point TileCoord { get; set; }
        public TileContentType OldContent { get; set; }
        public TileContentType NewContent { get; set; }
    }

    //TODO unused? 
    public event EventHandler<TileTypeChangedEventArgs> TileTypeChanged;
    public event EventHandler<TileContentChangedEventArgs> TileContentChanged;
    #endregion

    public enum TileType { Space, Floor, FloorTemplate }
    public enum TileContentType { None, Wall, WallTemplate, Door, DoorTemplate }


    public ITile MainContentTile { get; private set; }

    private TileType _type;
    private TileContentType _content;

    public Tile(TileType type, ITile mainContentTile)
    {
        Type = type;
        MainContentTile = mainContentTile;
    }

    public TileType Type
    {
        get
        {
            return _type;
        }
        set
        {
            if (_type != value)
            {
                var oldValue = _type;
                _type = value;
                OnTileTypeChanged(new TileTypeChangedEventArgs { NewTileType = value, OldTileType = oldValue, TileCoord = Point.Empty });
            }
        }
    }

    public TileContentType Content
    {
        get
        {
            return _content;
        }
        private set
        {
            if (_content != value)
            {
                var oldValue = _content;
                _content = value;
                OnTileContentChanged(new TileContentChangedEventArgs { NewContent = value, OldContent = oldValue, TileCoord = Point.Empty });
            }
        }
    }


    protected virtual void OnTileTypeChanged(TileTypeChangedEventArgs e)
    {
        TileTypeChanged?.Invoke(this, e);
    }

    protected virtual void OnTileContentChanged(TileContentChangedEventArgs e)
    {
        TileContentChanged?.Invoke(this, e);
    }

    public void Build(TileContentType content, ITile mainContentTile)
    {
        Type = (Type == TileType.Space) ? TileType.FloorTemplate : TileType.Floor;
        Content = content;
        MainContentTile = mainContentTile;
    }

    public void DestroyAll()
    {
        Type = TileType.Space;
        DestroyContent();
    }

    public void DestroyContent()
    {
        Content = TileContentType.None;
    }
}