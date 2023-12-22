using Godot;
using System;

namespace Puzzle;

public enum Colors
{
	Orange, Blue,
}

public partial class Piece : Button
{
#region Variaveis
	[Export] Label numberLabel;

	int _number;
	public int Number
	{
		get => _number;
		set
		{
			_number = value;
			// Mesmo tamanho de Piece aplicado ao tamanho da fonte do label (com subtração de 50)
			// Na prática, só será definido ao iniciar
			numberLabel.Set("theme_override_font_sizes/font_size", PieceSize.X - 50);
			numberLabel.Text = _number.ToString();
		}
	}

	public Vector2 PieceSize
	{
		// Size é uma propriedade do próprio Button
		get => Size;
		set
		{
			// Diminuir um pouco o tamanho para servir como margem externa
			Vector2 newSize = value;
			newSize.X -= 10;
			newSize.Y -= 10;
			Size = newSize;
		}
	}
#endregion
	public override void _Ready()
	{
	}

	/// <summary>
	/// Move a 'Piece' para uma nova posição no grid
	/// </summary>
	/// <param name="newPosition"></param>
	public void Move(Vector2 newPosition)
	{
		// this.TweenPosition(newPosition, 0.2f).SetEasing(GTweens.Easings.Easing.Linear);
		/*
		_ = GTweenGodotExtensions.Tween(
			() => Position,
			(x) => Position = x,
			newPosition,
			0.2f
		).SetEasing(GTweens.Easings.Easing.InOutBounce);
		*/
		/*
		Tween tween = GetTree().CreateTween();
		tween?.Kill();  // null propagation
		tween = CreateTween(); //.BindNode(this);
		tween.TweenProperty(
			this,
			new NodePath("Position"),
			newPosition,
			0.2f
		).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
		*/
		// HACK
		/*
		Chamar o GDScript de dentro do C# pois
		diretamente do C# o tween não funciona
		*/
		Node GodotHack = GetNode("/root/TweenManager");  //Autoload
		//tween.DoTween
		GodotHack.Call(
			"DoTween",
			new Variant[]
			{
				this,
				"position",
				newPosition,
				0.2f
			}
		);
	}

	/// <summary>
	/// Muda a cor da peça
	/// (de acordo com a posição certa no grid)
	/// </summary>
	/// <param name="color"></param>
	public void ChangeColor(Colors color)
	{
		string newColor = ChooseColor(color);

		StyleBoxFlat newStyleBoxFlat = GetThemeStylebox("normal").Duplicate() as StyleBoxFlat;
		Color defaultColor = newStyleBoxFlat.BgColor;
		newStyleBoxFlat.BgColor = Color.FromString(newColor, defaultColor);
		//StyleBoxFlat styleBoxFlat = new() {BgColor = Color.FromString(newColor, Color.Color8(0,0,0,0)) };
		//Set("theme_override_styles/normal", styleBoxFlat);
		AddThemeStyleboxOverride("normal", newStyleBoxFlat);
		/*
		StyleBoxFlat style = (StyleBoxFlat)Get("theme_override_styles/normal");
		style.Set("bg_color", newColor);
		*/
	}

	/// <summary>
	/// Determina a cor HEX de acordo com a
	/// cor escolhida no enum 'Colors'
	/// </summary>
	/// <param name="color"></param>
	/// <returns></returns>
	static string ChooseColor(Colors color)
	{
		return color switch
		{
			Colors.Blue => "#47bcff",
			// Colors.Orange
			_ => "#ffbc00",
		};

	}
}
