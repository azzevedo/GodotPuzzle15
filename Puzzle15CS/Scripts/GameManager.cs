using Godot;
using System;

namespace Puzzle;
public partial class GameManager : Node
{
/*
#region Singleton
	//GameManager() {}
	static GameManager _manager;
	public static GameManager Instance
	{
		//if (_manager == null)
			//return new Mananger();
		// Equivalente da linha abaixo (compound)
		// get => _manager ??= new();
		//  get => _maanger ??= GetNode<GameManager>("/root/GameManager");
		get
		{
			if (_manager == null)
				_manager = new();
			return _manager;
		}
	}
#endregion
*/

#region Variaveis
	Game game;
	public int GameMode { get; set; }
	PackedScene preGame = (PackedScene)ResourceLoader.Load("res://Scenes/Game.tscn");
#endregion

	/// <summary>
	/// Instancia a cena Game.tscn
	/// Delega a função de iniciar o jogo ao Game.tscn
	/// [mode] indica o tamanho do grid (3x3, 4x4, 5x5, 6x6)
	/// </summary>
	/// <param name="mode"></param>
	public void StartGame(int mode)
	{
		GameMode = mode;
		game = preGame.Instantiate<Game>();

		GetTree().Root.AddChild(game);
	}

	/// <summary>
	/// Reiniciar o jogo com o mesmo modo escolhido
	/// anteriormente
	/// </summary>
	public void PlayAgain()
	{
		// EndGame é quem chama GameManager.PlayAgain()
		game.ReEnableGameButtons();
		game.OnResetButtonPressed();
	}

	/// <summary>
	/// Vai para a StartScreen
	/// </summary>
	public void ExitToLobby()
	{
		// Em EndGame.cs, o jogador opta por escolher outro modo de jogo
		game.QueueFree();
	}
}
