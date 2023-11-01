using GameboardGUI;
using System.Drawing;

namespace eothello
{
    public partial class Form1 : Form
    {

        // Define Boardsize 
        const int NUM_OF_BOARD_ROWS = 8;
        const int NUM_OF_BOARD_COL = 8;


        GameboardImageArray _gameBoardGui;
        int[,] gameBoardData;
        string tileImagesDirPath = "Resources/";
        public int TurnCounter = 1;
        public bool FirstMove = true;
        public Form1()
        {
            InitializeComponent();
            Point top = new Point(30, 30);
            Point bottom = new Point(120, 120);
            

            // updates the games state, actual tiles = (_gameboardgui) LoggedPositions = (gameboardData)
            gameBoardData = this.MakeBoardArray();
            
            try
            {
                _gameBoardGui = new GameboardImageArray(this, gameBoardData, top, bottom, 0, tileImagesDirPath);
                _gameBoardGui.TileClicked += new GameboardImageArray.TileClickedEventDelegate(GameTileClicked);
                _gameBoardGui.UpdateBoardGui(gameBoardData);
                IterateThroughBoard();
                UserInterface();
            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show(ex.ToString(), "GAME BOARD SIZE PROBLEM", MessageBoxButtons.OK);
                this.Close();
            }

        }

        //When game is started it uses this to setup the board
        private int[,] MakeBoardArray()
        {
            int[,] BoardArray = new int[NUM_OF_BOARD_ROWS, NUM_OF_BOARD_COL];

            BoardArray[3, 3] = 10;
            BoardArray[4, 3] = 1;
            BoardArray[3, 4] = 1;
            BoardArray[4, 4] = 10;
            return BoardArray;

        }

        // Tuple to return if a position ((0 - 0) --> (8, 8)) = is a validmove, and if it is returns correct tiles 
        public (bool IsValid, List<Point> CapturedPoints) IsValidMove(int row, int col)
        {
            List<Point> capturedPoints = new List<Point>();

            if (gameBoardData[row, col] == 0 || gameBoardData[row, col] == 100)
            {
                Console.WriteLine(TurnCounter.ToString()); // Print the current TurnCounter value

                Point ScanPos = new Point(row, col);
                (int dx, int dy)[] offsets =
                {
            (1, 0), (1, 1), (0, 1), (-1, 1),
            (-1, 0), (-1, -1), (0, -1), (1, -1)
        };

                foreach (var offset in offsets)
                {
                    int newX = ScanPos.X + offset.dx;
                    int newY = ScanPos.Y + offset.dy;

                    if (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] != TurnCounter)
                    {
                        int opponentColor = TurnCounter == 1 ? 10 : 1;
                        List<Point> tempCapturedPoints = new List<Point>();

                        while (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] == opponentColor)
                        {
                            tempCapturedPoints.Add(new Point(newX, newY));
                            newX += offset.dx;
                            newY += offset.dy;
                        }

                        if (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] == TurnCounter && tempCapturedPoints.Count > 0)
                        {
                            // A valid move is found
                            capturedPoints.AddRange(tempCapturedPoints);
                        }
                    }
                }

                if (capturedPoints.Count > 0)
                {
                    return (true, capturedPoints); // Return true and the captured points
                }
            }

            return (false, capturedPoints); // Return false and an empty list of captured points
        }


        // does what it says on tin ;)
        private bool IsWithinBoard(int x, int y)
      {     
          return x >= 0 && x < gameBoardData.GetLength(0) && y >= 0 && y < gameBoardData.GetLength(1);
      }


        // Draws the valid squares with an outlined box
        public void IterateThroughBoard()
        {
            for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
            {
                for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                {
                    var (isValid, capturedPoints) = IsValidMove(row, col);

                    if (isValid)
                    {
                        _gameBoardGui.SetTile(row, col, 100.ToString());
                        gameBoardData[row, col] = 100;
                    }
                }
            }
  
        }

        // the turns should clear the board of any valid moves markers 
        public void DisposeOfValidMoves()
        {

            for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
            {
                for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                {
                    if (gameBoardData[row, col] == 100)
                    {
                        _gameBoardGui.SetTile(row, col, 0.ToString());
                    }
                }
            }

        }

        public bool AnyValidMoveLeft(int player)
        {
            for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
            {
                for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                {
                    var (isValid, _) = IsValidMove(row, col);
                    if (isValid)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public (int Player1Count, int Player10Count) CalculateWinner()
        {
            int player1Count = 0;
            int player10Count = 0;

            // Count the pieces for each player
            for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
            {
                for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                {
                    if (gameBoardData[row, col] == 1)
                    {
                        player1Count++;

                    }
                    else if (gameBoardData[row, col] == 10)
                    {
                        player10Count++;
                    }
                }
            }
            if (!AnyValidMoveLeft(1) && !AnyValidMoveLeft(10))
            {
                if (player1Count > player10Count)
                {
                    // Player 1 (color 1) wins
                    MessageBox.Show("Player 1 Wins");
                    
                }
                else if (player10Count > player1Count)
                {
                    // Player 2 (color 10) wins
                    MessageBox.Show("Player 2 Wins");

                }
                else
                {
                    // It's a tie
                    MessageBox.Show("It's a tie");

                }
                
            }
            return (player1Count, player10Count);
        }


        public void UserInterface()
        {
            var (player1Count, player10Count) = CalculateWinner();
            label2.Text = player10Count.ToString();
            label1.Text = player1Count.ToString();
            Color color = Color.FromArgb(123, 255, 128);
            if(TurnCounter == 10)
            {
                pictureBox4.BackColor = color;
                pictureBox5.BackColor = Color.Green;
            }
            else
            {
                pictureBox5.BackColor = color;
                pictureBox4.BackColor = Color.Green;
            }
  
        }

        // checks if move is valid using isvalidMove and then places it if it is
        public void GameTileClicked(object sender, EventArgs e)
        {
            int selectionRow = _gameBoardGui.GetCurrentRowIndex(sender);
            int selectionCol = _gameBoardGui.GetCurrentColumnIndex(sender);

            var (isValid, capturedPoints) = IsValidMove(selectionRow, selectionCol);

            if (isValid)
            {
                int color = TurnCounter;             
                DisposeOfValidMoves();

                _gameBoardGui.SetTile(selectionRow, selectionCol, color.ToString());
                gameBoardData[selectionRow, selectionCol] = TurnCounter;

              

                // Set all captured points to the current player's color
                foreach (Point capturedPoint in capturedPoints)
                {
                    gameBoardData[capturedPoint.X, capturedPoint.Y] = TurnCounter;
                    _gameBoardGui.SetTile(capturedPoint.X, capturedPoint.Y, color.ToString());
                }

               
                TurnCounter = (TurnCounter == 1) ? 10 : 1;

                // Redisplay valid moves for the next player
                IterateThroughBoard();

                

            }
            UserInterface();
        }
    }

}