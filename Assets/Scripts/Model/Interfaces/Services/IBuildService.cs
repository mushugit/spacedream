﻿using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Model.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Model.Interfaces.Services
{
    public interface IBuildService
    {
        bool Build(Point coord, TileContentType content, Size footprint, ITile mainContentTile = null);
        void Initialize(IBuildableMapService buildableMapService);

        event EventHandler<TileTypeChangedEventArgs> TileTypeChanged;
        event EventHandler<TileContentChangedEventArgs> TileContentChanged;
    }
}
