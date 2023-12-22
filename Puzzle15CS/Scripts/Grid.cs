using Godot;
using System;

namespace Puzzle;
public partial class Grid : Control
{
	/// <summary>
	/// Como funciona o 'signal' no C#
	/// O sinal tem que terminar com o sufixo 'EventHandler'
	/// Game._Ready() => onde tem a inscrição ao evento
	/// A assinatura do método que se inscreve tem que ser a mesma do delegate
	/// </summary>
	/// <param name="pixelPos"></param>
	[Signal]
	public delegate void TouchEventHandler(Vector2 pixelPos);

	/// <summary>
	/// Envia um 'signal' com a posição onde
	/// grid (Control) foi clicado
	/// </summary>
	/// <param name="event"></param>
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!@event.IsActionPressed("touch")) return;
		// Só executa os próximos comandos se for o evento 'touch'
		// e estiver dentro do grid (Control)

		Vector2 pos = GetLocalMousePosition();
		if (pos.X < 0 || pos.X > Size.X) return;
		if (pos.Y < 0 || pos.Y > Size.X) return;

		// Inverter para não dar erro ao acessar o 'grid_data' em Game.gd
		EmitSignal("Touch", InvertInputPosition(pos));
	}

	/// <summary>
	/// Troca as posições de X e Y e retorna um Vector2
	/// </summary>
	/// <param name="inputPos"></param>
	/// <returns></returns>
	static Vector2 InvertInputPosition(Vector2 inputPos)
	{
		Vector2 invertedPos = new()
		{
			X = inputPos.Y,
			Y = inputPos.X
		};

		return invertedPos;
	}
}
