extends ColorRect

@onready var play_again_button: Button = get_node("PlayAgainButton")

func _ready():
	''' Assim que esta cena entra na árvore, pausar o jogo
	pois o jogo finalizou '''
	get_tree().paused = true

func _On_PlayAgainButton_Pressed() -> void:
	'''
	Delega a responsabilidade de jogar novamente ao GameManager,
	que, por sua vez, vai chamar um método de Game.gd
	Para não ter que, neste arquivo [EndGame.gd], ter uma
	referência ao Game.tscn
	'''
	GameManager.PlayAgain()
	# Esta cena pode sumir
	queue_free()

func _On_ExitButton_Pressed() -> void:
	''' Tira o jogo de pause e volta à StarScreen '''
	# Sair para a seleção de modo de jogo
	# Tirar o jogo de pause :: definido no _ready
	get_tree().paused = false
	# O GameManager é quem faz esta articulação
	GameManager.ExitToLobby()
	queue_free()
