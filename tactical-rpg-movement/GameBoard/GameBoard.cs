using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class GameBoard : Node2D
{
	[Export] public Grid Grid {get; set;}
	
	private Unit _activeUnit;
	private UnitPath _unitPath;
	private UnitOverlay _unitOverlay;
	private Array<Vector2> _walkableCell = [];
	
	public static readonly Vector2[] Directions = 
	[
		Vector2.Left,
		Vector2.Up,
		Vector2.Right,
		Vector2.Down,
	];
	
	private Godot.Collections.Dictionary<Vector2, Unit> _units = new();
	
	public bool IsOccupied(Vector2 pos) => _units.ContainsKey(pos);

	public IEnumerable<Vector2> GetWalkableCells(Unit unit)
		=> FloodFill(unit.Cell, unit.MoveRange);

	public override void _Ready()
	{
		_unitPath = GetNodeOrNull<UnitPath>("UnitPath");
		_unitOverlay = GetNodeOrNull<UnitOverlay>("UnitOverlay");
		
		Reinitialize();
	}
	
	private void SelectUnit(Vector2 cell)
	{
		if(!_units.TryGetValue(cell, out var unit)) return;
		
		_activeUnit = unit;
		_activeUnit.IsSelected = true;
		_walkableCell = (Array<Vector2>)GetWalkableCells(_activeUnit);
		_unitOverlay.DrawOverlay(_walkableCell);
		_unitPath.Initialize(_walkableCell);
	}

	private void DeselectActiveUnit()
	{
		_activeUnit.IsSelected = false;
		_unitOverlay.Clear();
		_unitPath.Stop();
	}

	private void ClearActiveUnit()
	{
		_activeUnit = null;
		_walkableCell.Clear();
	}

	private async void MoveActiveUnit(Vector2 newCell)
	{
		if(IsOccupied(newCell) || ! _walkableCell.Contains(newCell))
			return;
		
		_units.Remove(_activeUnit.Cell);
		DeselectActiveUnit();
		_activeUnit.WalkAlong(_unitPath.CurrentPath);
		await ToSignal(_activeUnit, nameof(Unit.SignalName.WalkFinished));

		_activeUnit.Cell = newCell;
		_units[newCell] = _activeUnit;
		ClearActiveUnit();
	}

	public void OnCursorAcceptPressed(Vector2 cell)
	{
		if(_activeUnit is null)
			SelectUnit(cell);
		else if(_activeUnit is { IsSelected: true })
			MoveActiveUnit(cell);
	}

	public void OnCursorMoved(Vector2 newCell)
	{
		if(_activeUnit is not null && _activeUnit.IsSelected)
			_unitPath.Draw(_activeUnit.Cell, newCell);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (_activeUnit is not null && @event.IsActionPressed("ui_cancel"))
		{
			DeselectActiveUnit();
			ClearActiveUnit();
		}
	}

	public void Reinitialize()
	{
		_units.Clear();
		foreach (var child in GetChildren())
			if (child is Unit unit) 
				_units[unit.Cell] = unit;
	}
	
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
