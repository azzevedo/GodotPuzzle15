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

# var piece_colors = { "orange": "#ffbc00", "blue" : "#00a3b0" }
var piece_colors = { "orange": "#ffbc00", "blue" : "#4effff" }
# piece.ChangeColor("orange")

var number: int:
	get:
		return number
	set(value):
		number_label.text = str(value)
		# Mesmo tamanho de Piece aplicado ao tamanho da fonte do label
		number_label.set("theme_override_font_sizes/font_size", piece_size.x - 50)
		number = value

var piece_size: Vector2:
	get:
		# size é uma propriedade do próprio Button
		return size
	set(value):
		# Diminuir um pouco o tamanho para servir como margem externa
		size.x = value.x - 10
		size.y = value.y - 10


func Move(new_position: Vector2) -> void:
	var tween: Tween = create_tween()
	tween.tween_property(
		self, "position",
		new_position, 0.2
	).set_trans(Tween.TRANS_LINEAR).set_ease(Tween.EASE_IN_OUT)
	# position = new_position


func ChangeColor(color: String) -> void:
	var styleBox: StyleBoxFlat = StyleBoxFlat.new()
	styleBox.bg_color = Color.from_string(piece_colors[color], Color.REBECCA_PURPLE)
	set("theme_override_styles/normal", styleBox)
	
	#var style = get("theme_override_styles/normal")
	#style.set("bg_color", piece_colors[color])

