using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class UnitOverlay : TileMapLayer
{
	public new void Draw(IEnumerable<Vector2> cells)
	{
		Clear();
		SetCellsTerrainConnect(new Array<Vector2I>(cells.Select(x => (Vector2I)x)), 0, 0);
	}
}
