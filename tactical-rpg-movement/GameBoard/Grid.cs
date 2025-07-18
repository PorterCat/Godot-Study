using Godot;
using System;

[Tool]
public partial class Grid : Resource
{
	[Export] public Vector2 Size { get; set; } = new (20, 20);
	[Export] public Vector2 CellSize { get; set; } = new(80, 80);

	private Vector2 HalfCellSize => CellSize / 2;
	
	public Vector2 CalculateMapPosition(Vector2 gridPosition) =>
		gridPosition * CellSize + HalfCellSize;
	
	public Vector2 CalculateGridCoordinates(Vector2 mapPosition) => 
		(mapPosition / CellSize).Floor();
	
	public bool IsWithinBounds(Vector2 cellCoords) =>
		cellCoords.X >= 0 && cellCoords.X < Size.X  &&
		cellCoords.Y >= 0 && cellCoords.Y < Size.Y;

	public Vector2 Clamp(Vector2 gridPosition)
	{
		var grid = gridPosition;
		grid.X = Math.Clamp(grid.X, 0, Size.X - 1);
		grid.Y = Math.Clamp(grid.Y, 0, Size.Y - 1);
		return grid;
	}

	public int AsIndex(Vector2 cell) =>
		Convert.ToInt32(cell.X + Size.X * cell.Y);
}
