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
func Move(new_position: Vector2) -> void:
	position = new_position
