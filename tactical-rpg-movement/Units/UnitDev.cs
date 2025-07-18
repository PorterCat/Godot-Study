using Godot;

public partial class Unit
{
    private void SafeUpdateSpriteTexture()
    {
        var sprite = GetNodeOrNull<Sprite2D>("PathFollow2D/Sprite");
        if (Engine.IsEditorHint())
        {
            if (sprite != null && _skin != null)
            {
                sprite.Texture = _skin;
                QueueRedraw();
            }
        }
        else if (sprite != null)
            sprite.Texture = _skin;
    }
    
    private void SafeUpdateSpriteOffset()
    {
        var sprite = GetNodeOrNull<Sprite2D>("PathFollow2D/Sprite");
        if (Engine.IsEditorHint())
        {
            if (sprite != null)
            {
                sprite.Position = _skinOffset;
                QueueRedraw();
            }
        }
        else if (sprite != null)
            sprite.Position = _skinOffset;
    }
    
    private void SafeUpdateAnimationState()
    {
        var animPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (!Engine.IsEditorHint())
            animPlayer?.Play(_isSelected ? "selected" : "idle");
    }
}