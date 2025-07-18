using Godot;
using System;

public partial class Cursor : Node2D
{
	[Signal] public delegate void AcceptPressedEventHandler(Vector2 cell);
	[Signal] public delegate void MovedEventHandler(Vector2 newCell);
	
	[Export] public Grid Grid { get; set; }
	[Export] public float UiCooldown { get; set; } = 0.1f;

	private Vector2 _cell = Vector2.Zero;
	public Vector2 Cell
	{
		get => _cell;
		set
		{
			var newCell = Grid.Clamp(value);
			if (newCell.IsEqualApprox(_cell))
				return;
			_cell = newCell;
			Position = Grid.CalculateMapPosition(_cell);
			EmitSignal(nameof(Moved), _cell);
			_timer.Start();
		}
	}

	private Timer _timer; 
	
	public override void _Ready()
	{
		Grid ??= ResourceLoader.Load<Grid>("res://GameBoard/Grid.tres");
		
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = UiCooldown;
		Position = Grid.CalculateMapPosition(Cell);
	}
	
	public override void _Process(double delta) {}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (@event is InputEventMouseMotion mouseMotionEvent)
			Cell = Grid.CalculateGridCoordinates(mouseMotionEvent.Position);	
		
		else if (Input.IsActionPressed("click") || @event.IsActionPressed("ui_accept"))
		{
			EmitSignal(nameof(AcceptPressed), Cell);
			GetViewport().SetInputAsHandled();
		}

		var shouldMove = @event.IsPressed();
		if(@event.IsEcho())
			shouldMove = shouldMove && _timer.IsStopped();

		if (!shouldMove) return;

		if      (@event.IsAction("ui_right")) Cell += Vector2.Right;
		else if (@event.IsAction("ui_up"))    Cell += Vector2.Up;
		else if (@event.IsAction("ui_left"))  Cell += Vector2.Left;
		else if (@event.IsAction("ui_down"))  Cell += Vector2.Down;
	}

	public override void _Draw() =>
		DrawRect(new Rect2(-Grid.CellSize / 2, Grid.CellSize), Colors.AliceBlue, false, 2.0f);
}
