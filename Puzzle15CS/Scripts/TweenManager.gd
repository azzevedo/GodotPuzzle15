extends Node
class_name GodotHack

func DoTween(object: Object, property: NodePath, to_value: Variant, duration: float) -> void:
	var tween: Tween = create_tween()
	tween.tween_property(
		object,
		property,
		to_value,
		duration
	).set_trans(Tween.TRANS_LINEAR).set_ease(Tween.EASE_IN_OUT)
