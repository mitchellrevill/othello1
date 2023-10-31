using GameboardGUI;
using System.Drawing;

namespace eothello
{
    public partial class Form1 : Form
    {
        const int NUM_OF_BOARD_ROWS = 8;
        const int NUM_OF_BOARD_COL = 8;


        GameboardImageArray _gameBoardGui;
        int[,] gameBoardData;
        string tileImagesDirPath = "Resources/";
        public int TurnCounter = 1;

        public Form1()
        {
            InitializeComponent();

            Point top = new Point(10, 10);
            Point bottom = new Point(10, 10);

            gameBoardData = this.MakeBoardArray();
            
            try
            {
                _gameBoardGui = new GameboardImageArray(this, gameBoardData, top, bottom, 0, tileImagesDirPath);
                _gameBoardGui.TileClicked += new GameboardImageArray.TileClickedEventDelegate(GameTileClicked);
                _gameBoardGui.UpdateBoardGui(gameBoardData);
                IterateThroughBoard();

            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show(ex.ToString(), "GAME BOARD SIZE PROBLEM", MessageBoxButtons.OK);
                this.Close();
            }

        }


        public bool isValidMove(int row, int col)
        {
            if (gameBoardData[row, col] == 0)
            {
                Console.WriteLine(TurnCounter.ToString()); // Print the current TurnCounter value.

                Point ScanPos = new Point(row, col);
                (int dx, int dy)[] offsets =
                {
            (1, 0), (1, 1), (0, 1), (-1, 1),
            (-1, 0), (-1, -1), (0, -1), (1, -1)
        };

                foreach (var offset in offsets)
                {
                    List<Point> capturedPoints = new List<Point>();

                    int newX = ScanPos.X + offset.dx;
                    int newY = ScanPos.Y + offset.dy;

                    if (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] != TurnCounter)
                    {
                        int opponentColor = TurnCounter == 1 ? 10 : 1;

                        while (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] == opponentColor)
                        {
                            capturedPoints.Add(new Point(newX, newY));

                            newX += offset.dx;
                            newY += offset.dy;
                        }

                        if (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] == TurnCounter && capturedPoints.Count > 0)
                        {
                            // A valid move is found
                            return true; // Return true without making the move
                        }
                    }
                }
            }

            return false; // Return false if no valid move is found at (row, col)
        }

        public void IterateThroughBoard()
        {

            for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
            {
                for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                {
                    if (isValidMove(row, col))
                    {
                        _gameBoardGui.SetTile(row, col, 100.ToString());
                        gameBoardData[row, col] = 100;
                    }
                }
            }
        }

        public void Dispose1()
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






        public void ValidMove(int row, int col)
        {
            if (gameBoardData[row, col] == 0 || gameBoardData[row, col] == 100)
            {
                Console.WriteLine(TurnCounter.ToString()); // Print the current TurnCounter value.
      
                Point ScanPos = new Point(row, col);
                (int dx, int dy)[] offsets =
                {
            (1, 0), (1, 1), (0, 1), (-1, 1),
            (-1, 0), (-1, -1), (0, -1), (1, -1)
        };
      
                bool validMoveFound = false;
      
                foreach (var offset in offsets)
                {
                    List<Point> capturedPoints = new List<Point>();
      
                    int newX = ScanPos.X + offset.dx;
                    int newY = ScanPos.Y + offset.dy;
      
                    if (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] != TurnCounter)
                    {
                        int opponentColor = TurnCounter == 1 ? 10 : 1;
      
                        while (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] == opponentColor)
                        {
                            capturedPoints.Add(new Point(newX, newY));
      
                            newX += offset.dx;
                            newY += offset.dy;
                        }
      
                        if (IsWithinBoard(newX, newY) && gameBoardData[newX, newY] == TurnCounter && capturedPoints.Count > 0)
                        {
                            // A valid move is found
                            int color = TurnCounter;
                            _gameBoardGui.SetTile(row, col, color.ToString());
                            gameBoardData[row, col] = TurnCounter;
      
                            foreach (Point capturedPoint in capturedPoints)
                            {
                                gameBoardData[capturedPoint.X, capturedPoint.Y] = TurnCounter;
                                _gameBoardGui.SetTile(capturedPoint.X, capturedPoint.Y, color.ToString());
                            }
      
                            validMoveFound = true;
                        }
                    }
                }
                if (validMoveFound)
                {
                    // Toggle the TurnCounter for the next player's turn
                    TurnCounter = (TurnCounter == 1) ? 10 : 1;
                }
            }
        }
      
      private bool IsWithinBoard(int x, int y)
      {
         // Check if the coordinates (x, y) are within the boundaries of the game board.
          return x >= 0 && x < gameBoardData.GetLength(0) && y >= 0 && y < gameBoardData.GetLength(1);
      }



        private int[,] MakeBoardArray()
        {
            int[,] BoardArray = new int[NUM_OF_BOARD_ROWS, NUM_OF_BOARD_COL];

            BoardArray[3, 3] = 10;
            BoardArray[4, 3] = 1;
            BoardArray[3, 4] = 1;
            BoardArray[4, 4] = 10;
            return BoardArray;

        }

        public void GameTileClicked(object sender, EventArgs e)
        {
            int selectionRow = _gameBoardGui.GetCurrentRowIndex(sender);
            int selectionCol = _gameBoardGui.GetCurrentColumnIndex(sender);
            Dispose1();
            ValidMove(selectionRow, selectionCol);
            IterateThroughBoard();
           

        }


    }

}