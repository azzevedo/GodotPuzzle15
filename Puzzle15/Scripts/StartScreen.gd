extends Control


func _On_TextureButton3x3_Pressed() -> void:
	''' Inicia um jogo 3x3 '''
	InitiateGame(3)

func _On_TextureButton4x4_Pressed() -> void:
	InitiateGame(4)

func _On_TextureButton5x5_Pressed() -> void:
	InitiateGame(5)

func _On_TextureButton6x6_Pressed() -> void:
	InitiateGame(6)

func InitiateGame(mode: int) -> void:
	GameManager.StartGame(mode)
