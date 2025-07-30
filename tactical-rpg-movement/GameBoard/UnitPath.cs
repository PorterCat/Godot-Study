using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Vector2 = Godot.Vector2;

public partial class UnitPath : TileMapLayer
{
	[Export] public Grid Grid {get; set;}
	
	private PathFinder _pathFinder;
	public Array<Vector2> CurrentPath = [];
	
	public void Initialize(IEnumerable<Vector2> walkableCells) =>
		_pathFinder = new PathFinder(Grid, walkableCells);

	public new void Draw(Vector2 cellStart, Vector2 cellEnd)
	{
		Clear();
		CurrentPath = new Array<Vector2>(_pathFinder.CalculatePointPath(cellStart, cellEnd));

		var path = new Vector2I[CurrentPath.Count];
		for (int i = 0; i < CurrentPath.Count; ++i)
			path[i] = (Vector2I)CurrentPath[i];
		
		SetCellsTerrainConnect(new Array<Vector2I>(path), 0, 0);
	}

	public void Stop()
	{
		_pathFinder = null;
		Clear();
	}
	
	public override void _Ready()
	{
	}
}
