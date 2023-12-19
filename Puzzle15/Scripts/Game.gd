extends TextureRect
class_name Game

#TODO
@onready var pre_piece: PackedScene = preload("res://Scenes/Piece.tscn")
#

@onready var time_label: Label = get_node("HBoxContainer/TimeContainer/TimeLabel")
@onready var moves_label: Label = get_node("HBoxContainer/MovesContainer/MovesLabel")
@onready var grid: Control = get_node("Grid")

var game_solution: Array
var grid_data: Array  # Matriz

var elapsed_time: float
var moves: int

# 3x3 4x4 5x5 6x6
var game_mode: int

# Última posição de 'null'
# Para não ficar voltando a peça a esta posição imediatamente
# na hora do Shuffle()
# A última posição do 'null' ao iniciar o 'grid_data' é (2, 2)
var last_null_position: Vector2 = Vector2(2, 2)


func _ready():
	# Fica de olho para quando o 'grid' for clicado
	grid.connect("touch", GridTouch)
	# Iniciar o Grid limpo
	Reset()

func _process(delta):
	elapsed_time += delta
	UpdateHUD()


func Start(mode: int) -> void:
	''' Inicializa o jogo definindo o tamanho de [grid, grid_data] '''
	# Armazenar o modo de jogo para usar num Reset()
	game_mode = mode
	InitGrid(mode)
	# Randomizar as posições das peças
	Shuffle(game_mode * 30)
	# Mostrar as peças na tela Grid (Control)
	PopulateGrid()


func CheckColorPosition() -> void:
	#return
	for x in game_mode:
		for y in game_mode:
			
			var piece: Piece = grid_data[x][y]
			var piece_solution: Piece = game_solution[x][y]
			
			if (piece == null && piece_solution == null):
				# posição (2, 2) com os mesmos valores (null)
				continue
			elif (piece == null):
				# somente o valor [Piece] em grid_data é null
				continue
			elif (piece_solution == null):
				# somente o valor em game_solution é null
				piece.ChangeColor("orange")
			elif (piece.number == piece_solution.number):
				piece.ChangeColor("blue")
			elif (piece.number != piece_solution.number):
				piece.ChangeColor("orange")


func InitGrid(_size: int) -> void:
	'''
	Define o tamanho da matriz do grid
	[de acordo com o modo do jogo, 3x3 4x4 ...]
	'_size' é a quantidade de colunas e linhas
	'''
	MakeGrid(_size)

	# Usado para definir o número de cada 'Piece'
	var number: int = 1
	# Popular o grid_data com os 'Piece'
	for x in _size:
		for y in _size:
			# O último índice da última linha continuará 'null'
			# [2][2] _size - 1 sendo 2
			if (x == _size - 1 && x == y):
				break
			# A largura e altura e cada Piece é definido ao dividir
			# o tamanho do Grid [Control] pelo número de colunas
			var piece_size: Vector2 = grid.size / _size
			#TODO apagar - instanciar pelo GameManager não é mais necessário
			#var piece: Piece = GameManager.InstantiatePiece(piece_size, number)
			var piece: Piece = pre_piece.instantiate()
			piece.piece_size = piece_size
			piece.number = number
			grid_data[x][y] = piece

			number += 1

	# A sequência incial será a solução do jogo
	game_solution = grid_data.duplicate(true)


func MakeGrid(_size: int) -> void:
	''' Inializa o 'grid_data' com valores 'null' '''
	# em _ready > Reset > ClearGrid   [ao adicionar este Node à árvore]
	# 'grida_data' já será um array vazio []
	# grid_data = []
	for x in _size:
		grid_data.append([])
		for y in _size:
			grid_data[x].append(null)


func Shuffle(_times: int = 1) -> void:
	''' Randomiza as posições das peças no 'grid_data'
	movendo-as '''
	#[x] Remover 'documentação'
	# Há 2 formas de mover, o usuário selecionando uma peça
	## checar se é possível
	# Para randomizar, seleciona 1 das peças adjacentes ao espaço vazio
	# Shuffle localiza a posição vazia e chama Move várias vezes
	# em uma das peças adjacentes [aleatoriamente] e a move
	# CanMakeMove -> [true] -> Move
	# Move -> grid_data
	# Move -> Piece : representação visual [GridToData]
	#[x]

	for i in range(_times):
		var null_position: Vector2 = FindNullPosition()
		var valid_positions: Array[Vector2] = GetGridAdjacentPositions(null_position)
		# Remover a última posição do 'null' para a peça não
		# voltar imediatamente para esta posição
		valid_positions.erase(last_null_position)
		# Atualizar a posição mais recente do 'null' em 'grid_data'
		last_null_position = null_position
		var index: int = randi_range(0, valid_positions.size() - 1)
		var piece_pos: Vector2 = valid_positions[index]
		Swipe(piece_pos, null_position)


func FindNullPosition() -> Vector2:
	# Encontra a posição 'null' em grid_data
	# Retorna a posição encontrada
	var null_pos: Vector2
	for x in game_mode:
		for y in game_mode:
			if (grid_data[x][y] == null):
				null_pos = Vector2(x, y)
				break

	return null_pos


func GetGridAdjacentPositions(null_pos: Vector2) -> Array[Vector2]:
	''' Encontra as posições adjacentes válidas à posição 'null'
	em grid_data
	Retorna um Array de Vector2 representando estas posições '''

	var adjacents: Array[Vector2] = []

	# São 4 posíveis posições para mover
	# LEFT, na verdade, representa a posição acima para este propósito
	var up: Vector2 = null_pos + Vector2.LEFT
	# RIGHT == down
	var down: Vector2 = null_pos + Vector2.RIGHT
	# UP == right
	var right: Vector2 = null_pos + Vector2.UP
	# DOWN == left
	var left: Vector2 = null_pos + Vector2.DOWN

	var positions: Array[Vector2] = []
	positions.append(up)
	positions.append(down)
	positions.append(left)
	positions.append(right)

	for pos in positions:
		# Após as somas, as posições válidas não podem ser
		# menores que zero ou maiores que o 'game_mode' - 1
		if (pos.x < 0 || pos.y < 0 || pos.x == game_mode || pos.y == game_mode):
			continue

		# Também checar qual das posições válidas foi a última posição
		# de 'null' para não mover a peça de volta imediatamente
		# Logo, esta posição não é adicionada às posições adjacentes válidas
		#BUG tornar este if possível faz com que algumas peças não se movam ao clicar
		#if (pos == last_null_position):
		#TODO posso colocar essa checagem no Shuffle, já que é o único lugar
		# onde este last_null_position não deve ser considerado
		#	continue

		# Posição válida
		adjacents.append(pos)

	return adjacents
	

func Swipe(piece_pos: Vector2, null_pos: Vector2) -> void:
	''' Troca as posições de uma Piece com a posição 'null'
	em grid_data '''
	
	grid_data[null_pos.x][null_pos.y] = grid_data[piece_pos.x][piece_pos.y]
	grid_data[piece_pos.x][piece_pos.y] = null


func PopulateGrid() -> void:
	''' Exibe as peças no Grid (Control)'''
	for x in game_mode:
		for y in game_mode:
			var piece: Piece = grid_data[x][y]
			if (piece == null):
				continue
			grid.add_child(piece)
			piece.position = GridToPixel(x, y)
	
	CheckColorPosition()


func GridToPixel(x: int, y: int) -> Vector2:
	''' Converte uma posição em 'grid_data'
	para uma posição em pixel no 'grid' '''

	var pos: Vector2 = Vector2()
	# Ex.: (1, 2) 1 * 600 / 3  ::  2 * 600 / 3
	# Os valores de X e Y devem ser invertidos ao converter
	# para pixel pois o X da tela é na horizontal e o Y é na vertical
	# Em 'grid_data', X é vertical e Y horizontal
	pos.y = x * grid.size.x / game_mode
	pos.x = y * grid.size.y / game_mode

	return pos


func PixelToGrid(pixel_pos: Vector2) -> Vector2:
	''' Converte uma posição em pixel do 'grid'
	para uma posição Vector2i(x,y) que será usada para
	acessar o 'grid_data' '''
	# A altura e largura do 'grid' é a mesma, então só precisa de um valor
	# Ex.: 600 / 3 == 200
	var width: float = grid.size.x /  game_mode

	# 503 / 200 == 2
	var x: float = floor(pixel_pos.x / width)
	# 304 / 200 == 1
	var y: float = floor(pixel_pos.y / width)

	var grid_pos: Vector2i = Vector2(x, y)
	return grid_pos


func GridTouch(input_pos: Vector2) -> void:
	''' Responde a ações do jogador para mover as peças no 'grid' '''
	var grid_pos: Vector2 = PixelToGrid(input_pos)

	# Se a posição tiver o valor 'null' não continua a execução,
	# tem que ter uma peça
	if (!CanMakeMove(grid_pos)): return
	
	# Passou pelo if acima, pode mover a peça [Piece]
	# Mover a peça no 'grid_data' e no grid (Control)
	#TODO isso [null] já é checado dentro de CanMakeMove
	# achar um jeito de não repetir
	var null_pos: Vector2 = FindNullPosition()
	var piece: Piece = grid_data[grid_pos.x][grid_pos.y]

	Swipe(grid_pos, null_pos)
	# Após feito o Swipe no grid_data
	# 'null_pos' [em pixel] é para onde deve-se mover a peça no grid (Control)
	piece.Move(GridToPixel(floori(null_pos.x), floori(null_pos.y)))
	# print(piece.number)
	CheckColorPosition()
	
	# Contabilizar 1 movimento
	moves += 1
	if (DidMoveWin()):
		#HACK
		# Às vezes não é chamado no _process assim que o jogo finaliza
		UpdateHUD()
		GameOver()


func GameOver() -> void:
	''' Mostra uma tela de jogo concluído '''
	var end_game_packed: PackedScene = preload("res://Scenes/EndGame.tscn")
	var end_game: Node = end_game_packed.instantiate()
	var label: Label = end_game.get_node("Label")
	label.text = "You win in %.0f seconds with %d moves" %[elapsed_time, moves]
	GameManager.game_over = true
	DisableGameButtons()
	add_child(end_game)

func DisableGameButtons() -> void:
	''' Desativa os botões Pause, Reset, Close 
	quando a tela de GameOver for exibida '''
#	var c = get_node("CloseButton")
#	c.process_mode = Node.PROCESS_MODE_DISABLED
	var p = get_node("BottomContainer/PlayPauseButton")
	p.process_mode = Node.PROCESS_MODE_PAUSABLE
	var r = get_node("BottomContainer/ResetButton")
	r.process_mode = Node.PROCESS_MODE_PAUSABLE

func ReEnableGameButtons() -> void:
	''' Reativa os botões ao clicar em PlayAgain
	na tela de EndGame '''
	var p = get_node("BottomContainer/PlayPauseButton")
	p.process_mode = Node.PROCESS_MODE_ALWAYS
	var r = get_node("BottomContainer/ResetButton")
	r.process_mode = Node.PROCESS_MODE_ALWAYS

func DidMoveWin() -> bool:
	''' Checa se o último movimento feito
	finaliza o jogo '''
	# O jogo estará completo se o 'grid_data'
	# for igual ao 'game_solution'
	return grid_data == game_solution


func CanMakeMove(grid_pos: Vector2) -> bool:
	''' Retorna true se o movimento em grid_data[x][y] pode ser
	realizado '''
	# Se a posição estiver vazia, não pode realizar o movimento
	# pois não clicou numa peça válida
	if (IsPositionEmpty(grid_pos.x, grid_pos.y)): return false
	
	#TODO
	#[ ] Mover mais de uma peça
	# Checar se há algum espaço livre na linha ou coluna
	var null_pos: Vector2 = FindNullPosition()
	var piece_positions: Array[Vector2] = GetGridAdjacentPositions(null_pos)

	# De acordo com as posições adjacentes à posição de 'null'
	# se uma delas [piece_positions] for igual a 'grid_pos'
	# a movimentação pode acontecer
	# Se não, retorna false
	if !(grid_pos in piece_positions): return false

	# A condição do if acima é falsa e o movimento é possível
	return true


func IsPositionEmpty(x: float, y: float) -> bool:
	''' Retorna true se a posição [x][y] estiver vazia (null) '''
	return grid_data[x][y] == null


func UpdateHUD() -> void:
	'''
	Atualiza as informações no HUD
	UpdateHUD é chamado a cada frame  :: _process ::
	'''
	time_label.text = "%.0f" %[elapsed_time]
	moves_label.text = "%d" %[moves]


func Play() -> void:
	''' Play no jogo '''
	get_tree().paused = false


func Pause() -> void:
	''' Pause no jogo '''
	get_tree().paused = true
	

func Reset() -> void:
	''' Reseta o jogo ao seu estado inicial '''
	elapsed_time = 0
	moves = 0
	ClearGrid()
	Start(game_mode)


func ClearGrid() -> void:
	''' Remove os itens do grid_data e do grid (Control) '''
	# Limpar o 'grid_data'
	grid_data = []
	# Limpar 'grid'
	get_tree().call_group("pieces", "queue_free")



''' Métodos dos botões [Nós filhos] '''
func _On_ResetButton_Pressed() -> void:
	""" Tira de pause [se estiver pausado]
	e reseta o jogo no modo atual """
	# Assegurar que não haja pause
	if (get_tree().paused):
		_On_PlayPauseButton_Pressed()
	# Resetar a partida no modo atual ex. 3x3
	Reset()

func _On_CloseButton_Pressed() -> void:
	""" Fecha o jogo atual e volta à StartScreen """
	# Tirar do pause, se estiver
	Play()
	# Fechar a cena do jogo [Game.tscn]
	queue_free()


func _On_PlayPauseButton_Pressed() -> void:
	""" Pausa ou dá play no jogo ao clicar
	Se o jogo estiver em pause, tira de pause
	Se o jogo estiver em play, fica em pause
	E muda o icon de acordo com o estado """
	var paused: bool = get_tree().paused

	if (paused):
		Play()
	else:
		Pause()

	ChangePlayPauseIcon(paused)


func ChangePlayPauseIcon(paused: bool) -> void:
	""" Muda a imagem do botão 'PlayPauseButton' """
	var image: Resource

	if (paused):
		image = load("res://Assets/Images/pause_button.png")
	else:
		image = load("res://Assets/Images/play_button.png")

	var button: Button = get_node("BottomContainer/PlayPauseButton")
	button.icon = image
