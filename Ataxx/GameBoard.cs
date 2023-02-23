using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ataxx
{
    internal class GameBoard : TableLayoutPanel
    {
        public Square[,] board { get; set; }
        public GameRules rules { get; set; }

        public GameBoard(int RowCount, int ColumnCount, bool AutoSize) {
            this.RowCount = RowCount;
            this.ColumnCount = ColumnCount;
            this.AutoSize = AutoSize;
            rules = new GameRules(this);
            board = new Square[RowCount, ColumnCount];
            for (int x = 0; x < RowCount; x++)
            {
                for (int y = 0; y < ColumnCount; y++)
                {
                    Square square = new Square(x, y, Square.Team.Neutral);
                    square.Click += new EventHandler(rules.play);
                    board[x, y] = square;
                    this.Controls.Add(square, x, y);
                }
            }
            addPlayerSquares();
            paintBoard();
        }
        public void addPlayerSquares()
        {
            // Team Hoodie
            board[0, 5].team = Square.Team.Hoodie;
            board[0, 6].team = Square.Team.Hoodie;
            board[1, 5].team = Square.Team.Hoodie;
            board[1, 6].team = Square.Team.Hoodie;

            // Team Sweater
            board[5, 0].team = Square.Team.Sweater;
            board[5, 1].team = Square.Team.Sweater;
            board[6, 0].team = Square.Team.Sweater;
            board[6, 1].team = Square.Team.Sweater;

        }

        public void paintBoard()
        {
            for(int x = 0;x < RowCount;x++)
            {
                for(int  y = 0;y < ColumnCount;y++)
                {
                    if (board[x, y].team == Square.Team.Neutral) { board[x, y].BackColor = Color.WhiteSmoke;}
                    if (board[x, y].team == Square.Team.Hoodie) { board[x,y].BackColor = Color.CornflowerBlue;}
                    if (board[x, y].team == Square.Team.Sweater) { board[x,y].BackColor = Color.Coral;}
                }
            }
        }
    }

    internal class Square : Button
    {
        public int x { get; set; }
        public int y { get; set; }
        public Team team { get; set; }
        public enum Team
        {
            Neutral,
            Hoodie,
            Sweater
        }

        public Square(int x, int y, Team team)
        {
            this.x = x;
            this.y = y;
            this.team = team;
            Text = (x + 1) + " - " + (y + 1);
        }
    }

    internal class GameRules
    {
        private bool player1Turn = true;
        private GameBoard board;
        public GameRules(GameBoard board) { this.board = board; }


        public void play(object sender, EventArgs e) {
            Square square = (Square)sender;
            if (player1Turn)
            {
                if (square.team == Square.Team.Hoodie) {
                    board.paintBoard();
                    getCloneMoves(square);
                }
                if (square.team == Square.Team.Neutral && square.BackColor == Color.Red)
                {
                    square.team = Square.Team.Hoodie;
                    infect(square);
                    board.paintBoard();
                    player1Turn = false;
                }
            }
            else
            {
                if (square.team == Square.Team.Sweater)
                {
                    board.paintBoard();
                    getCloneMoves(square);
                }
                if (square.team == Square.Team.Neutral && square.BackColor == Color.Red)
                {
                    square.team = Square.Team.Sweater;
                    infect(square);
                    board.paintBoard();
                    player1Turn = true;
                }
            }
        }

        public void getCloneMoves(Square square)
        {
            int startX = square.x - 1 < 0 ? square.x : square.x - 1;
            int endX = square.x + 1 > 6 ? square.x : square.x + 1;
            int startY = square.y - 1 < 0 ? square.y : square.y - 1;
            int endY = square.y + 1 > 6 ? square.y : square.y + 1;


            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (board.board[x,y].team == Square.Team.Neutral)
                    {
                        board.board[x,y].BackColor = Color.Red;
                    }
                }
            }
        }

        public void infect(Square square)
        {
            int startX = square.x - 1 < 0 ? square.x : square.x - 1;
            int endX = square.x + 1 > 6 ? square.x : square.x + 1;
            int startY = square.y - 1 < 0 ? square.y : square.y - 1;
            int endY = square.y + 1 > 6 ? square.y : square.y + 1;


            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (board.board[x, y].team != Square.Team.Neutral)
                    {
                        board.board[x, y].team = square.team;
                    }
                }
            }
        }

    }
}
