extends Node


var pre_game: PackedScene = preload("res://Scenes/Game.tscn")
var game: Node

# Referência à cena Piece (serão instanciadas várias)
@onready var pre_piece: PackedScene = preload("res://Scenes/Piece.tscn")

var game_over: bool = false

func StartGame(mode: int) -> void:
	''' Delega a função de iniciar o jogo ao Game.tscn
	[mode] indica o tamanho do grid (3x3, 4x4, 5x5, 6x6)'''
	game = pre_game.instantiate()
	# Adicionar a cena Game à árvore
	get_tree().root.add_child(game)
	# Iniciar depois de adicionar à árvore para pegar [dentro de Game]
	# o tamanho do Grid [Control]
	game.Start(mode)


# O GameManager é quem fica responsável por instanciar uma Piece
# Método chamado em Game.gd
#TODO - apagar método [obsoleto] - Refatorado para Game.gd (instancia)
# e Piece (configura o Label com a numeração)
func InstantiatePiece(piece_size: Vector2, number: int) -> Piece:
	''' Instancia uma Piece e atribui um tamanho
	à Piece (Control) e um número ao seu Node Label '''

	var piece: Piece = pre_piece.instantiate()
	# Diminuir um pouco o tamanho para servir como margem externa
	piece_size.x -= 10
	piece_size.y -= 10
	piece.size = piece_size
	var label: Label = piece.get_node("Label")
	label.text = str(number)
	# Mesmo tamanho de Piece aplicado ao tamanho da fonte do label
	label.set("theme_override_font_sizes/font_size", piece_size.x - 50)

	return piece


func PlayAgain() -> void:
	''' Reiniciar o jogo com o mesmo modo escolhido
	anteriormente '''
	# EndGame.gd é quem chama este método
	game.ReEnableGameButtons()
	game._On_ResetButton_Pressed()

func ExitToLobby() -> void:
	''' Vai para a StartScreen '''
	# Em EndGame.gd, o jogador opta por escolher outro modo de jogo
	game.queue_free()
