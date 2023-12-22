using Godot;
using System;

namespace Puzzle;
public partial class EndGame : ColorRect
{
	Button playAgainButton;
	GameManager gm;

	/// <summary>
	/// Assim que esta cena entra na árvore, pausar o jogo
	/// pois o jogo finalizou
	/// </summary>
	public override void _Ready()
	{
		playAgainButton = GetNode<Button>("PlayAgainButton");
		gm = GetNode<GameManager>("/root/GameManager");
		GetTree().Paused = true;
	}

	/// <summary>
	/// Delega a responsabilidade de jogar novamente ao GameManager,
	/// que, por sua vez, vai chamar um método de Game.cs
	/// Para não ter que, neste arquivo [EndGame.cs], ter uma
	/// referência ao Game.tscn
	/// </summary>
	public void OnPlayAgainButtonPressed()
	{
		gm.PlayAgain();
		QueueFree();
	}

	/// <summary>
	/// Tira o jogo de pause e volta à StarScreen
	/// </summary>
	public void OnCloseButtonPressed()
	{
		// Sair para a seleção de modo de jogo
		// Tirar o jogo de pause :: definido no _ready
		GetTree().Paused = false;
		// O GameManager é quem faz esta articulação
		gm.ExitToLobby();
		QueueFree();
	}
}
