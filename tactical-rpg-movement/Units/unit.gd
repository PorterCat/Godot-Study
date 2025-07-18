@tool
class_name Unit
extends Path2D

@export var skin: Texture:
	set(value):
		skin = value
		if not _sprite:
			await ready
		_sprite.texture = value

@onready var _sprite: Sprite2D = $PathFollow2D/Sprite

func _ready() -> void:
	pass

func _process(delta: float) -> void:
	pass
