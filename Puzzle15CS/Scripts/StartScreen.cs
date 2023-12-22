using Godot;
using System;

namespace Puzzle;
public partial class StartScreen : Control
{
#region Botões
	public void OnTextureButto3x3Pressed()
	{
		InitiateGame(3);
	}

	public void OnTextureButton4x4Pressed()
	{
		InitiateGame(4);
	}

	public void OnTextureButton5x5Pressed()
	{
		InitiateGame(5);
	}

	public void OnTextureButton6x6Pressed()
	{
		InitiateGame(6);
	}
#endregion

	void InitiateGame(int mode)
	{
		GameManager gm = GetNode<GameManager>("/root/GameManager");
		gm.StartGame(mode);
		//.Instance.StartGame(mode);
		/*
		PackedScene preGame = (PackedScene)ResourceLoader.Load("res://Scenes/Game.tscn");
		GetTree().Root.AddChild(preGame.Instantiate());
		*/
	}
}
