using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class UnitOverlay : TileMapLayer
{
	public void DrawOverlay(IEnumerable<Vector2> cells)
	{
		Clear();
		var arr = new Array<Vector2I>(cells.Select(x => (Vector2I)x));
		SetCellsTerrainConnect(arr, 0, 0, false);
	}
}
