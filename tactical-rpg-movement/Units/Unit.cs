using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

#if TOOLS
[Tool]
#endif
public partial class Unit : Path2D
{
	[Signal] public delegate void WalkFinishedEventHandler();
	
	[Export] public Grid Grid { get; set; }
	[Export] public uint MoveRange { get; set; } = 6;
	[Export] public float MoveSpeed { get; set; } = 600.0f;
	
	private Texture2D _skin;
	[Export] public Texture2D Skin
	{
		get => _skin;
		set
		{
			_skin = value;
			SafeUpdateSpriteTexture();
		}
	}

	private Vector2 _skinOffset = Vector2.Zero;
	[Export] public Vector2 SkinOffset
	{
		get => _skinOffset;
		set
		{
			_skinOffset = value;
			SafeUpdateSpriteOffset();
		}
	}
	
	private Vector2 _cell = Vector2.Zero;
	public Vector2 Cell
	{
		get => _cell;
		set => _cell = Grid?.Clamp(value) ?? value;
	}
	
	private bool _isSelected = false;
	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			_isSelected = value;
			SafeUpdateAnimationState();
		}
	}
	
	private bool _isWalking = false;
	public bool IsWalking
	{
		get => _isWalking;
		set
		{
			_isWalking = value;
			SetProcess(_isWalking);
		}
	}
	
	private PathFollow2D _pathFollow;
	
	public override void _Ready()
	{
		_pathFollow = GetNodeOrNull<PathFollow2D>("PathFollow2D");

		if (Engine.IsEditorHint())
		{
			SafeUpdateSpriteTexture();
			SafeUpdateSpriteOffset();
			return;
		}
		
		Grid ??= ResourceLoader.Load<Grid>("res://GameBoard/Grid.tres");
		
		SetProcess(false);
		Cell = Grid.CalculateGridCoordinates(Position);
		Position = Grid.CalculateMapPosition(Cell);
		
		if (!Engine.IsEditorHint())
			Curve = new Curve2D();
	}
	
	public override void _Process(double delta)
	{
		_pathFollow.Progress += MoveSpeed * (float)delta;
	
		if (_pathFollow.ProgressRatio >= 1.0f)
		{
			IsWalking = false;
			_pathFollow.Progress = 0.0f;
			Position = Grid.CalculateMapPosition(Cell);
			Curve.ClearPoints();
			EmitSignal(nameof(WalkFinished));
		}
	}
	
	public void WalkAlong(Array<Vector2> path)
	{
		if (path == null || path.Count == 0)
			return;
		
		Curve.ClearPoints();
		
		foreach (Vector2 point in path)
		{
			var mapPosition = Grid.CalculateMapPosition(point);
			var localPosition = mapPosition - Position;
			Curve.AddPoint(localPosition);
		}
		
		Cell = path[^1];
		IsWalking = true;
	}
}
