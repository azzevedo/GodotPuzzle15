using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Puzzle;

/*
Classe Game [partial] com métodos relacionados
ao funcionamento [mecânica]
*/
public partial class Game : TextureRect
{
	// As variáveis podem ficar no outro Game.cs

	/// <summary>
	/// Encontra as posições adjacentes válidas à posição 'null'
	/// em gridData
	/// </summary>
	/// <param name="nullPos">A posição com o valor 'null'</param>
	/// <returns>
	/// Retorna um Array de Vector2I representando estas posições
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
	/// Retorna true se o movimento em gridData pode ser
	/// realizado
	/// </summary>
	/// <param name="gridPos"></param>
	/// <returns></returns>
	bool CanMakeMove(Vector2I gridPos)
	{
		// Não tem peça na posição clicada
		if (IsPositionEmpty(gridPos.X, gridPos.Y)) return false;

		Vector2I nullPos = FindNullPosition();
		// Mesma linha da posição vazia
		if (gridPos.X == nullPos.X) return true;
		// Mesma coluna
		if (gridPos.Y == nullPos.Y) return true;

		/*
		Clicou em uma peça mas nesta linha ou coluna
		não tem espaço vazio
		*/
		return false;
	}

	/// <summary>
	/// Movimenta uma ou mais peças em 'gridData'
	/// a partir da peça clicada [selectedPiecePos]
	/// e suas representações visuais em 'grid'
	/// em direção à posição vazia.
	/// </summary>
	/// <param name="selectedPiecePos"></param>
	void MovePieces(Vector2I selectedPiecePos)
	{
		Vector2I nullPos = FindNullPosition();

		// Mover as peças na mesma linha
		if (selectedPiecePos.X == nullPos.X)
		{
			MovePiecesInLine(selectedPiecePos, nullPos);
		}

		// Mover as peças na mesma coluna
		if (selectedPiecePos.Y == nullPos.Y)
		{
			MovePiecesInColumn(selectedPiecePos, nullPos);
		}
	}

	/// <summary>
	/// Move as peças entre a posição vazia [null]
	/// e a peça selecionada (inclusive) [selectedPiecePos]
	/// para a posição vazia [null] na mesma linha
	/// </summary>
	/// <param name="selectedPiecePos"></param>
	/// <param name="nullPos"></param>
	void MovePiecesInLine(Vector2I selectedPiecePos, Vector2I nullPos)
	{
		int x = selectedPiecePos.X;
		int yNull = nullPos.Y;
		int ySelected = selectedPiecePos.Y;

		// Mover da esquerda para a direita
		if (ySelected < yNull)
		{
			/*
			3,0  -- 3,3 [null]
			y é um valor a menos que a posição Y de null
			tem que ser ordem decrescente até o valor Y da peça selecionada
			*/
			for (int y = yNull - 1; y >= ySelected; y--)
			{
				// x não muda
/* 				Piece piece = gridData[x, y];
				Swipe(new(x, y), new(x, y + 1));
				piece.Move(GridToPixel(x, y + 1));
 */
				MoveSinglePiece(new(x, y), new(x, y + 1));
			}

			return;
		}

		// Da direita para a esquerda
		for (int y = yNull + 1; y <= ySelected; y++)
		{
/* 			Piece piece = gridData[x, y];
			Swipe(new(x, y), new(x, y - 1));
			piece.Move(GridToPixel(x, y - 1));
 */
			MoveSinglePiece(new(x, y), new(x, y - 1));
		}
	}

	/// <summary>
	/// Move as peças entre a posição vazia [null]
	/// e a peça selecionada (inclusive) [selectedPiecePos]
	/// para a posição vazia [null] na mesma coluna
	/// </summary>
	/// <param name="selectedPiecePos"></param>
	/// <param name="nullPos"></param>
	void MovePiecesInColumn(Vector2I selectedPiecePos, Vector2I nullPos)
	{
		int y = selectedPiecePos.Y;
		int xNull = nullPos.X;
		int xSelected = selectedPiecePos.X;

		// Mover de cima pra baixo
		if (xSelected < xNull)
		{
			for (int x = xNull - 1; x >= xSelected; x--)
			{
/* 				Piece piece = gridData[x, y];
				Swipe(new(x, y), new(x + 1, y));
				piece.Move(GridToPixel(x + 1, y));
 */
				MoveSinglePiece(new(x, y), new(x + 1, y));
			}

			return;
		}

		// Mover de baixo pra cima
		for (int x = xNull + 1; x <= xSelected; x++)
		{
/* 			Piece piece = gridData[x, y];
			Swipe(new(x, y), new(x - 1, y));
			piece.Move(GridToPixel(x - 1, y));
 */
			MoveSinglePiece(new(x,y), new(x - 1,y));
		}
	}

	/// <summary>
	/// Move uma peça por vez no gridData e grid [View],
	/// da posição antiga [oldPos] para a nova [newPos]
	/// e contabiliza esse movimento [moves]
	/// </summary>
	/// <param name="oldPos"></param>
	/// <param name="newPos"></param>
	void MoveSinglePiece(Vector2I oldPos, Vector2I newPos)
	{
		/*
		Esta linha tem que ser antes do Swipe,
		senão o Swipe vai acessar um valor errado
		*/
		Piece piece = gridData[oldPos.X, oldPos.Y];
		// Mover a peça no 'gridData'
		Swipe(new(oldPos.X, oldPos.Y), new(newPos.X, newPos.Y));
		// Mover a peça no grid (Control)
		piece.Move(GridToPixel(newPos.X, newPos.Y));

		moves++;
	}
}