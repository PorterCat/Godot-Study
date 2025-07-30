using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using GodotOnReady.Attributes;

public partial class GameBoard : Node2D
{
	[Export] public Grid Grid {get; set;}
	
	[OnReadyGet("UnitOverlay")]
	private UnitOverlay _unitOverlay;
	
	[OnReadyGet("UnitPath")]
	private UnitPath _unitPath;
	
	private Unit _activeUnit;
	
	private Array<Vector2I> _walkableCell = [];
	
	public static readonly Vector2[] Directions = 
	[
		Vector2.Left,
		Vector2.Up,
		Vector2.Right,
		Vector2.Down,
	];
	
	private Godot.Collections.Dictionary<Vector2, Unit> _units = new();
	
	private bool IsOccupied(Vector2 pos) => _units.ContainsKey(pos);

	[OnReady] private void Reinitialize()
	{
		_units.Clear();
		foreach (var child in GetChildren())
		{
			var unit = child as Unit;
			if (unit is not null) 
				_units[unit.Cell] = unit;
		}
	}

	public IEnumerable<Vector2> GetWalkableCells(Unit unit)
		=> FloodFill(unit.Cell, unit.MoveRange);
	
	private IEnumerable<Vector2> FloodFill(Vector2 cell, uint maxDistance)
	{
		Array<Vector2> array = [];
		Stack<Vector2> stack = new();
		stack.Push(cell);

		while (stack.Count > 0)
		{
			var current = stack.Pop();
			
			if(!Grid.IsWithinBounds(current)) continue;
			if(array.Contains(current)) continue;
			
			Vector2 difference = (current - cell).Abs();
			var distance = Convert.ToInt32(difference.X + difference.Y);
			
			if(distance > maxDistance) continue;
			array.Add(current);

			foreach (var dir in Directions)
			{
				var coordinates = current + dir;
				if(IsOccupied(coordinates)) continue;
				if(array.Contains(coordinates)) continue;
				
				stack.Push(coordinates);
			}
		}
		
		return array;
	}
}
