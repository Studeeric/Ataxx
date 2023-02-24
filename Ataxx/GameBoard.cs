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
        public Button undo { get; set; }

        public GameBoard(int RowCount, int ColumnCount, bool AutoSize) {
            this.RowCount = RowCount;
            this.ColumnCount = ColumnCount;
            this.AutoSize = AutoSize;
            rules = new GameRules(this);
            board = new Square[RowCount, ColumnCount];
            undo = new Button() { Text = "Undo"};
            undo.Click += new EventHandler(rules.undo);
            this.Controls.Add(undo);
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
        private Square lastClicked = null;
        private CPU CPU = null;
        private Stapel<Square.Team[,]> vorigeZet = new Stapel<Square.Team[,]>();
        private GameBoard board;
        public GameRules(GameBoard board)
        {
            this.board = board;
            this.CPU = new CPU(board, this, Square.Team.Sweater);
        }


        public void play(object sender, EventArgs e) {
            Square square = (Square)sender;
            if (player1Turn)
            {   
                if (square.team == Square.Team.Hoodie) {
                    board.paintBoard();
                    getJumpMoves(square);
                    getCloneMoves(square);
                }
                if (square.team == Square.Team.Neutral)
                {
                    slaOp();
                    if (square.BackColor == Color.Red)
                    {
                        square.team = Square.Team.Hoodie;
                        infect(square);
                        board.paintBoard();
                        player1Turn = false;
                    }
                    if (square.BackColor == Color.Green)
                    {
                        square.team = Square.Team.Hoodie;
                        lastClicked.team = Square.Team.Neutral;
                        infect(square);
                        board.paintBoard();
                        player1Turn = false;
                    }
                    if (!player1Turn)
                    {
                        CPU.PlayGameRandom();
                    }
                }
            }
            else
            {
                if (square.team == Square.Team.Sweater)
                {
                    board.paintBoard();
                    getJumpMoves(square);
                    getCloneMoves(square);
                }
                if (square.team == Square.Team.Neutral)
                {
                    if (square.BackColor == Color.Red)
                    {
                        square.team = Square.Team.Sweater;
                        infect(square);
                        board.paintBoard();
                        player1Turn = true;
                    }
                    if (square.BackColor == Color.Green)
                    {
                        square.team = Square.Team.Sweater;
                        lastClicked.team = Square.Team.Neutral;
                        infect(square);
                        board.paintBoard();
                        player1Turn = true;
                    }
                }
            }
            lastClicked = square;
            checkTotals();
        }

        public void slaOp()
        {
            Square.Team[,] teams = new Square.Team[board.RowCount, board.ColumnCount];
            for (int x = 0; x < board.RowCount; x++)
            {
                for (int y = 0; y < board.ColumnCount; y++)
                {
                    teams[x, y] = board.board[x, y].team;
                }
            }
            vorigeZet.duw(teams);
        }

        public void undo(object sender, EventArgs e)
        {
            Square.Team[,] teams = vorigeZet.pak();
            if (teams == null)
            {
                return;
            }
            for (int x = 0; x < board.RowCount; x++)
            {
                for (int y = 0; y < board.ColumnCount; y++)
                {
                    board.board[x, y].team = teams[x, y];
                }

            }
            player1Turn = true;
            board.paintBoard();
        }

        public void checkTotals()
        {
            int hoodies = 0;
            int sweaters = 0;
            int neutrals = 0;
            for (int x = 0; x < board.RowCount; x++)
            {
                for (int y = 0; y < board.ColumnCount; y++)
                {
                    if(board.board[x, y].team == Square.Team.Hoodie)
                    {
                        hoodies++;
                    }
                    if (board.board[x, y].team == Square.Team.Sweater)
                    {
                        sweaters++;
                    }
                    if (board.board[x, y].team == Square.Team.Neutral)
                    {
                        neutrals++;
                    }
                }
            }
            if (neutrals == 0 || sweaters == 0 || hoodies == 0)
            {
                if (hoodies > sweaters)
                {
                    setVictorySquares(Square.Team.Hoodie);
                }
                if (hoodies < sweaters)
                {
                    setVictorySquares(Square.Team.Sweater);
                }
                if (hoodies == sweaters)
                {
                    setVictorySquares(Square.Team.Neutral);
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

        public void getJumpMoves(Square square)
        {
            int startX = square.x - 2 < 0 ? (square.x - 1 < 0 ? square.x : square.x - 1) : square.x - 2;
            int endX = square.x + 2 > 6 ? (square.x + 1 > 6 ? square.x : square.x + 1) : square.x + 2;
            int startY = square.y - 2 < 0 ? (square.y - 1 < 0 ? square.y : square.y - 1) : square.y - 2;
            int endY = square.y + 2 > 6 ? (square.y + 1 > 6 ? square.y : square.y + 1) : square.y + 2;


            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (board.board[x, y].team == Square.Team.Neutral)
                    {
                        board.board[x, y].BackColor = Color.Green;
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

        public void setVictorySquares(Square.Team team)
        {
            for (int x = 0; x < board.RowCount; x++)
            {
                for (int y = 0; y < board.ColumnCount; y++)
                {
                    board.board[x, y].team = team;
                }
            }
            board.paintBoard();
        }

    }
}
