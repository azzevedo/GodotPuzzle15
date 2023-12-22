using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle;
public partial class Game : TextureRect
{
#region Variaveis
	[Export] Label elapsedTimeLabel, movesLabel;
	[Export] Control grid;
	[Export] PackedScene prePiece;

	Piece[,] gridData;
	List<int> gameSolution;

	double elapsedTime;
	int moves, gameMode; // 3x3, 4x4, 5x5, 6x6

	/*
	Última posição de 'null'
	Para não ficar voltando a peça a esta posição imediatamente
	na hora do Shuffle()
	A última posição do 'null' ao iniciar o 'gridData' é (2, 2)
	*/
	Vector2I lastNullPosition = new (2, 2);

#endregion

	public override void _Ready()
	{
		GameManager gm = GetNode<GameManager>("/root/GameManager");
		// Armazenar o modo de jogo para usar num Reset()
		gameMode = gm.GameMode;

		// Fica de olho para quando o 'grid' for clicado
		Grid g = (Grid)grid;
		g.Touch += GridTouch;
		// Iniciar o Grid limpo
		Reset();
	}

	public override void _Process(double delta)
	{
		elapsedTime += delta;
		UpdateHUD();
	}

	// Tirar de pause
	void Play() => GetTree().Paused = false;

	// Pôr em pause
	void Pause() => GetTree().Paused = true;

	/// <summary>
	/// Reseta o jogo ao seu estado inicial
	/// </summary>
	void Reset()
	{
		elapsedTime = 0;
		moves = 0;
		ClearGrid();
		StartGame();
	}

	/// <summary>
	/// Remove os itens do grid_data e do grid (Control)
	/// </summary>
	void ClearGrid()
	{
		// Limpar/Inicializar o array 'gridData'
		gridData = new Piece[gameMode, gameMode];
		// Limpar/Inicializar a lista 'gameSolution'
		gameSolution = new();
		// Limpar o 'grid'
		/* CallGroup não funciona no C# */
		//GetTree().CallGroup("Pieces", "QueueFree");
		//GetTree().CallGroup("Pieces", "Free");
		Godot.Collections.Array<Node> groupPieces = GetTree().GetNodesInGroup("Pieces");
		foreach (Node p in groupPieces)
		{
			p.QueueFree();
		}
	}

	/// <summary>
	/// Inicializa o jogo definindo o tamanho de [grid, gridData]
	/// </summary>
	/// <param name="mode"></param>
	void StartGame()
	{
		InitGrid();
		Shuffle(gameMode * 30);
		PopulateGrid();
	}

	/// <summary>
	/// Instancia e adiciona as 'Pieces' em 'gridData'
	/// Também define gameSolution
	/// </summary>
	void InitGrid()
	{
		int number = 1;

		for (int x = 0; x < gameMode; x++)
		{
			for (int y = 0; y < gameMode; y++)
			{
				/*
				O último índice da última linha continuará 'null'
				[2][2] _size - 1 sendo 2
				*/
				if (x == gameMode - 1 && x == y)
				{
					// Se for o último item de gridData (null)
					// gameSolution recebe um valor 0 para representá-lo
					gameSolution.Add(0);
					break;
				}

				/*
				A largura e altura e cada Piece é definido ao dividir
				o tamanho do Grid [Control] pelo número de colunas
				*/
				Vector2 pieceSize = grid.Size / gameMode;
				Piece piece = prePiece.Instantiate<Piece>();
				piece.PieceSize = pieceSize;
				piece.Number = number;

				gridData[x,y] = piece;

				// Também definir a solução do jogo
				// Uma simples lista em ordem crescente
				gameSolution.Add(number);

				number++;
			}
		}
	}

	/// <summary>
	/// Randomiza as posições das peças no 'gridData'
	/// movendo-as
	/// </summary>
	/// <param name="times"></param>
	void Shuffle(int times = 1)
	{
		for (int i = 0; i < times; i++)
		{
			Vector2I nullPosition = FindNullPosition();
			List<Vector2I> validPositions = GetGridAdjacentPositions(nullPosition);

			// Remover a última posição do 'null' para a peça não
			// voltar imediatamente para esta posição
			validPositions.Remove(nullPosition);
			// Atualizar a posição mais recente do 'null' em 'gridData'
			lastNullPosition = nullPosition;

			int index = GD.RandRange(0, validPositions.Count - 1);
			Vector2I piecePos = validPositions[index];
			Swipe(piecePos, nullPosition);
		}
	}

	/// <summary>
	/// Encontra a posição 'null' em gridData
	/// Retorna a posição encontrada
	/// </summary>
	/// <returns></returns>
	Vector2I FindNullPosition()
	{
		Vector2I nullPos = Vector2I.Zero;

		for (int x = 0; x < gameMode; x++)
		{
			for (int y = 0; y < gameMode; y++)
			{
				if (gridData[x,y] == null)
				{
					nullPos = new(x,y);
					break;
				}
			}
		}

		// Sempre tem que haver uma posição null
		// Se não houver, vai dar merda
		// Vai retornar "gridData[0,0]";
		return nullPos;
	}

	/// <summary>
	/// Encontra as posições adjacentes válidas à posição 'null'
	/// em gridData
	/// </summary>
	/// <param name="nullPos">A posição com o valor 'null'</param>
	/// <returns>
	/// Retorna um Array de Vector2 representando estas posições
	/// </returns>
	List<Vector2I> GetGridAdjacentPositions(Vector2I nullPos)
	{
		List<Vector2I> adjacents = new();

		/*
		São 4 posíveis posições para mover
		LEFT, na verdade, representa a posição acima para este propósito
		RIGHT == down
		UP == left
		DOWN == right
		*/
		Vector2I up = nullPos + Vector2I.Left;
		Vector2I down = nullPos + Vector2I.Right;
		Vector2I left = nullPos + Vector2I.Up;
		Vector2I right = nullPos + Vector2I.Down;

		List<Vector2I> positions = new() {up, down, left, right};

		foreach (Vector2I pos in positions)
		{
			/*
			Após as somas, as posições válidas não podem ser
			menores que zero ou maiores que o 'game_mode' - 1
			*/
			if (pos.X < 0 || pos.Y < 0 || pos.X == gameMode || pos.Y == gameMode)
				continue;

			// Adiciona a posição válida neste loop
			adjacents.Add(pos);
		}

		return adjacents;
	}

	/// <summary>
	/// Troca as posições de uma Piece com a posição 'null'
	/// em gridData
	/// </summary>
	/// <param name="piecePos"></param>
	/// <param name="nullPos"></param>
	void Swipe(Vector2I piecePos, Vector2I nullPos)
	{
		gridData[nullPos.X, nullPos.Y] = gridData[piecePos.X, piecePos.Y];
		gridData[piecePos.X, piecePos.Y] = null;
	}

	/// <summary>
	/// Exibe as peças no Grid (Control)
	/// </summary>
	void PopulateGrid()
	{
		for (int x = 0; x < gameMode; x++)
		{
			for (int y = 0; y < gameMode; y++)
			{
				Piece piece = gridData[x,y];
				if (piece == null)
					continue;

				grid.AddChild(piece);
				piece.Position = GridToPixel(x, y);
			}
		}

		// Mudar as cores das peças ao posicioná-las
		ChangePiecesColor();
	}

	/// <summary>
	/// Converte uma posição em 'gridData'
	/// para uma posição em pixel no 'grid'
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	Vector2 GridToPixel(int x, int y)
	{
		Vector2 pos = new()
		{
			/*
			Ex.: (1, 2) 1 * 600 / 3  ::  2 * 600 / 3
			Os valores de X e Y devem ser invertidos ao converter
			para pixel pois o X da tela é na horizontal e o Y é na vertical
			Em 'gridData', X é vertical e Y horizontal
			*/
			Y = x * grid.Size.X / gameMode,
			X = y * grid.Size.Y / gameMode
		};

		return pos;
	}

	/// <summary>
	/// Responde a ações do jogador para mover as peças no 'grid'
	/// </summary>
	/// <param name="inputPos"></param>
	void GridTouch(Vector2 inputPos)
	{
		Vector2I gridPos = PixelToGrid(inputPos);

		// Se a posição tiver o valor 'null' não continua a execução,
		// tem que ter uma peça
		if (! CanMakeMove(gridPos)) return;

		// Passou pelo if acima, pode mover a peça [Piece]
		Vector2I nullPos = FindNullPosition();

		// Mover a peça no grid (Control)
		// Esta linha tem que ser antes do Swipe,
		// senão o Swipe vai acessar um valor errado
		Piece piece = gridData[gridPos.X, gridPos.Y];
		// Mover a peça no 'gridData'
		Swipe(gridPos, nullPos);
		piece.Move(GridToPixel(nullPos.X, nullPos.Y));

		// Mudar de cor ao mover a peça (se estiver na posição certa)
		ChangePiecesColor();

		moves++;

		if (DidMoveWin())
		{
			/*
			HACK
			UpdateHUD às vezes não é chamado no _process assim que o jogo finaliza
			*/
			UpdateHUD();
			GameOver();
		}

	}

	/// <summary>
	/// Converte uma posição em pixel do 'grid'
	/// para uma posição Vector2I(x,y) que será usada para
	/// acessar o 'gridData'
	/// </summary>
	/// <param name="pixelPos"></param>
	/// <returns></returns>
	Vector2I PixelToGrid(Vector2 pixelPos)
	{
		/*
		A altura e largura do 'grid' é a mesma, então só precisa de um valor
		Ex.: 600 / 3 == 200
		*/
		float width = grid.Size.X / gameMode;

		// Ex. 503 / 200 == 2
		int x = Mathf.FloorToInt(pixelPos.X / width);
		// Ex. 304 / 200 == 1
		int y = Mathf.FloorToInt(pixelPos.Y / width);

		Vector2I gridPos = new(x, y);
		return gridPos;
	}

	/// <summary>
	/// Retorna true se o movimento em grid_data[x][y] pode ser
	/// realizado
	/// </summary>
	/// <param name="gridPos"></param>
	/// <returns></returns>
	bool CanMakeMove(Vector2I gridPos)
	{
		/*
		Se a posição estiver vazia, não pode realizar o movimento
		pois não clicou numa peça válida
		*/
		if (IsPositionEmpty(gridPos.X, gridPos.Y)) return false;

		/*
		TODO
		[ ] Mover mais de uma peça
		Checar se há algum espaço livre na mesma linha ou coluna
		*/
		Vector2I nullPos = FindNullPosition();
		List<Vector2I> piecePositions = GetGridAdjacentPositions(nullPos);

		/*
		De acordo com as posições adjacentes à posição de 'null'
		se uma delas [piecePositions] for igual a 'grid_pos'
		a movimentação pode acontecer
		Se não, retorna false
		(a posição clicada não é adjacente à 'null', não tem um espaço vazio para onde ir)
		*/
		if (! piecePositions.Contains(gridPos)) return false;

		// A condição do if acima é falsa e o movimento é possível
		return true;
	}

	/// <summary>
	/// Retorna true se a posição [x][y] estiver vazia (null)
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	bool IsPositionEmpty(int x, int y) => gridData[x,y] == null;

	/// <summary>
	/// Checa se o último movimento feito finaliza o jogo
	/// </summary>
	/// <returns></returns>
	bool DidMoveWin()
	{	
		// Temporária para representar os valores de 'gridData'
		List<int> tempData = new();

		for (int x = 0; x < gameMode; x++)
		{
			for (int y = 0; y < gameMode; y++)
			{
				if (gridData[x,y] == null)
					// Adicionar um valor 0 em 'tempData' para representar o 'null'
					tempData.Add(0);
				else
					tempData.Add(gridData[x,y].Number);
			}
		}

		/*
		O jogo estará completo se o 'tempData'
		for igual ao 'gameSolution'

		string tempDataString = string.Join("", tempData);
		string solutionString = string.Join("", gameSolution);
		*/

		// GD.Print(tempDataString == solutionString);
		// Decidi comparar as sequências sem converter para strings
		return tempData.SequenceEqual(gameSolution);
		// return tempDataString.Equals(solutionString);
	}

	/// <summary>
	/// Mudar a cor da peça se sua posição no 'grid' está correta
	/// </summary>
	void ChangePiecesColor()
	{
		int index = -1;

		for (int x = 0; x < gameMode; x++)
		{
			for (int y = 0; y < gameMode; y++)
			{
				// 'index' precisa incrementar o valor antes de checar o 'null'
				index++;

				Piece piece = gridData[x,y];

				if (piece == null)
					continue;
				else if (piece.Number == gameSolution[index])
					// O número da peça corresponde ao valor na posição correta
					// em gameSolution
					piece.ChangeColor(Colors.Blue);
				else
					piece.ChangeColor(Colors.Orange);
			}
		}
	}

	/// <summary>
	/// Mostra uma tela de jogo concluído
	/// </summary>
	async void GameOver()
	{
		// HACK
		// [x] Task.Delay funciona, não precisa depender de criar um timer
		// GetNode("/root/TweenManager").Call("Sleep", 0.2f);
		// await ToSignal(GetTree().CreateTimer(0.3), "Timeout");
		await Task.Delay(400);

		PackedScene endGamePacked = (PackedScene)ResourceLoader.Load("res://Scenes/EndGame.tscn");
		Node endGame = endGamePacked.Instantiate();
		Label infoLabel = endGame.GetNode<Label>("InfoLabel");
		infoLabel.Text = $"You win in {elapsedTime:F0} seconds with {moves} moves";
		// GameManager gm = GetNode<GameManager>("/root/GameManager");
		DisableGameButtons();
		AddChild(endGame);
	}

	/// <summary>
	/// Desativa os botões Pause, Reset, Close 
	/// quando a tela de GameOver for exibida
	/// </summary>
	void DisableGameButtons()
	{
		Node p = GetNode("BottomContainer/PlayPauseButton");
		p.ProcessMode = ProcessModeEnum.Pausable;

		Node r = GetNode("BottomContainer/ResetButton");
		r.ProcessMode = ProcessModeEnum.Pausable;
	}

	/// <summary>
	/// Reativa os botões ao clicar em PlayAgain
	/// na tela de EndGame
	/// </summary>
	public void ReEnableGameButtons()
	{
		Node p = GetNode("BottomContainer/PlayPauseButton");
		p.ProcessMode = ProcessModeEnum.Always;

		Node r = GetNode("BottomContainer/ResetButton");
		r.ProcessMode = ProcessModeEnum.Always;
	}

	/// <summary>
	/// Atualiza as informações no HUD
	/// UpdateHUD é chamado a cada frame  :: _Process ::
	/// </summary>
	void UpdateHUD()
	{
		elapsedTimeLabel.Text = $"{elapsedTime:F0}";
		movesLabel.Text = $"{moves}";
	}


#region Sinais
	/// <summary>
	/// Fecha o jogo atual e volta à StartScreen
	/// </summary>
	public void OnCloseButtonPressed()
	{
		// Tirar do pause, se estiver
		Play();
		// Fechar a cena do jogo [Game.tscn]
		QueueFree();
	}

	/// <summary>
	/// Tira de pause [se estiver pausado]
	/// e reseta o jogo no modo atual
	/// </summary>
	public void OnResetButtonPressed()
	{
		// Assegurar que não esteja em pause
		if (GetTree().Paused)
			OnPlayPauseButtonPressed();

		// Resetar a partida no modo atual. Ex: 3x3
		Reset();
	}

	/// <summary>
	/// Pausa ou dá play no jogo ao clicar
	/// Se o jogo estiver em pause, tira de pause
	/// Se o jogo estiver em play, fica em pause
	/// E muda o icon de acordo com o estado
	/// </summary>
	public void OnPlayPauseButtonPressed()
	{
		bool paused = GetTree().Paused;
		
		if (paused) Play();
		else Pause();

		ChangePlayPauseIcon(paused);
	}

	/// <summary>
	/// Muda a imagem do botão 'PlayPauseButton'
	/// </summary>
	/// <param name="paused"></param>
	void ChangePlayPauseIcon(bool paused)
	{
		Resource image;

		if (paused)
			image = ResourceLoader.Load("res://Images/pause_button.png");
		else
			image = ResourceLoader.Load("res://Images/play_button.png");

		Button playPauseButton = GetNode<Button>("BottomContainer/PlayPauseButton");
		playPauseButton.Icon = (Texture2D)image;
	}
#endregion

}
