extends Control

signal touch

func _input(event):
	# Envia um 'signal' com a posição onde
	# grid (Control) foi clicado
	if (!event.is_action_pressed("touch")): return

	var pos: Vector2 = get_local_mouse_position()
	if (pos.x < 0 || pos.x > self.size.x): return
	if (pos.y < 0 || pos.y > self.size.y): return

	# Só executa os próximos comandos se for o evento 'touch'
	# e estiver dentro do grid (Control)

	# Inverter para não dar erro ao acessar o 'grid_data' em Game.gd
	touch.emit(InvertInputPosition(pos))


func InvertInputPosition(input_pos: Vector2) -> Vector2:
	''' Troca as posições de X e Y e retorna um Vector2 '''
	var inverted_pos: Vector2 = Vector2()
	inverted_pos.x = input_pos.y
	inverted_pos.y = input_pos.x

	return inverted_pos
