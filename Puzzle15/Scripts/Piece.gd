extends Button
class_name Piece

"""
var number_label: Label
var number: int


func _init(requested_size: Vector2, _number: int):
	'''
	Define um tamanho proporcional ao objeto Piece
	Este tamanho varia de acordo com o modo de jogo selecionado
	Quanto maior a quantidade de peças no tabuleiro, menor o tamanho das peças
	'''
	size = requested_size
	number = _number


func _ready():
	# Quanto entrar na árvore, exibe-se o número no Label
	# Útil quando exibir as peças na tela
	number_label = get_node("Label")
	number_label.text = str(number)
"""
@export var number_label: Label

var number_str: String:
	get:
		return number_label.text
	set(value):
		number_label.text = value
		# Mesmo tamanho de Piece aplicado ao tamanho da fonte do label
		number_label.set("theme_override_font_sizes/font_size", piece_size.x - 50)

var piece_size: Vector2:
	get:
		# size é uma propriedade do próprio Button
		return size
	set(value):
		# Diminuir um pouco o tamanho para servir como margem externa
		size.x = value.x - 10
		size.y = value.y - 10

#func _init():
#	# Godot não tem Awake
#	call_deferred("InitProperties")
	
func InitProperties():
	number_label = get_node("NumberLabel")

func Move(new_position: Vector2) -> void:
	position = new_position
