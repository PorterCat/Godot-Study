using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class PathFinder : RefCounted
{
	public static readonly Vector2[] Directions = 
	[
		Vector2.Left,
		Vector2.Up,
		Vector2.Right,
		Vector2.Down,
	];
	private Grid _grid;
	private AStar2D _astar = new();
	
	public PathFinder(Grid grid, IEnumerable<Vector2> walkableCells)
	{
		_grid = grid;
		var cellMappings = new Godot.Collections.Dictionary<Vector2, int>();
		
		foreach (Vector2 cell in walkableCells)
			cellMappings[cell] = _grid.AsIndex(cell);

		AddAndConnectPoints(cellMappings);
	}

	public Vector2[] CalculatePointPath(Vector2 start, Vector2 end)
	{
		var startIndex = _grid.AsIndex(start);
		var endIndex = _grid.AsIndex(end);
		
		if(_astar.HasPoint(startIndex) && _astar.HasPoint(endIndex))
			return _astar.GetPointPath(startIndex, endIndex);
		return [];
	}
	
	private void AddAndConnectPoints(Godot.Collections.Dictionary<Vector2, int> cellMappings)
	{
		foreach (var cell in cellMappings)
			_astar.AddPoint(cellMappings[cell.Key], cell.Key);

		foreach (var cell in cellMappings)
			foreach (var neighborIndex in FindNeighborIndices(cell.Key, cellMappings))
				_astar.ConnectPoints(cellMappings[cell.Key], neighborIndex);
	}
	
	private IEnumerable<int> FindNeighborIndices(Vector2 cell, Godot.Collections.Dictionary<Vector2, int> cellMappings)
	{
		foreach (var direction in Directions)
		{
			Vector2 neighbor = cell + direction;
			if (cellMappings.TryGetValue(neighbor, out var neighborIndex))
				yield return neighborIndex;
		}
	}
}
