using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ataxx
{
    internal class CPU
    {
        Random random = new Random();
        GameBoard board;
        GameRules rules;
        Square.Team team;

        public CPU (GameBoard board, GameRules rules, Square.Team team)
        {
            this.board = board;
            this.rules = rules;
            this.team = team;
        }

        public void PlayGameRandom()
        {
            List<Square> moves = new List<Square>();
            List<Square> squares = getPossibleSquares();
            if (squares.Count == 0)
            {
                return;
            }
            while (moves.Count == 0)
            {
                int activeSquare = random.Next(0, squares.Count);
                squares[activeSquare].PerformClick();
                moves = getPossibleMoves();
            }
            int? move = getBestMove(moves);
            int activeMove = random.Next(0, moves.Count);
            moves[(int)move].PerformClick();
        }
            // int? move = getBestMove(moves);

        public List<Square> getPossibleSquares()
        {
            List<Square> moves = new List<Square>();
            foreach (Square s in board.board)
            {
                if (s.team == team)
                {
                    moves.Add(s);
                }
            }
            return moves;
        }

        public List<Square> getPossibleMoves()
        {
            List<Square> moves = new List<Square>();
            foreach (Square s in board.board)
            {
                if (s.BackColor == Color.Red || s.BackColor == Color.Green)
                {
                    moves.Add(s);
                }
            }
            return moves;
        }

        public int? getBestMove(List<Square> moves)
        {
            int? bestMove = new int();
            int mostCaptured = 0;
            for (int i = 0; i < moves.Count; i++)
            {
                int startX = moves[i].x - 1 < 0 ? moves[i].x : moves[i].x - 1;
                int endX = moves[i].x + 1 > 6 ? moves[i].x : moves[i].x + 1;
                int startY = moves[i].y - 1 < 0 ? moves[i].y : moves[i].y - 1;
                int endY = moves[i].y + 1 > 6 ? moves[i].y : moves[i].y + 1;
                int totalCaptured = 0;
                if (moves[i].BackColor == Color.Red)
                {
                    totalCaptured++;
                }
                for (int x = startX; x < endX; x++)
                {
                    for (int y =  startY; y < endY; y++)
                    {
                        Square neighbour = board.board[x, y];
                        if (neighbour.team == Square.Team.Hoodie)
                        {
                            totalCaptured++;
                        }
                    }
                }
                if (totalCaptured > mostCaptured)
                {
                    bestMove = i;
                }
            }
            if (bestMove == null)
            {
                bestMove = random.Next(0, moves.Count);
            }
            return bestMove;
        }
        
    }
}
