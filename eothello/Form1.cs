using GameboardGUI;
using System.Text.Json;
using System.Net.Security;
using System.Speech.Synthesis;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Diagnostics;

namespace eothello
{
    public partial class ONielo : Form
    {


        const int NUM_OF_BOARD_ROWS = 8;
        const int NUM_OF_BOARD_COL = 8;


        GameboardImageArray _gameBoardGui;
        int[,] gameBoardData;
        string tileImagesDirPath = "Resources/";
        public int TurnCounter = 1;
        public bool FirstMove = true;
        public bool FileFirst = true;
        public int num = 1;
        public bool saveGameEnabled = true;
        private readonly SpeechSynthesizer speaker = new SpeechSynthesizer();
        public bool virtualplayer = false;
        private Stopwatch stopwatch = new Stopwatch();
        private int combinationCount = 0;
        int checkBoxClicks = 1;

        public ONielo()
        {
            InitializeComponent();
            Point top = new Point(30, 30);
            Point bottom = new Point(120, 120);



            gameBoardData = this.MakeBoardArray();

            try
            {
                _gameBoardGui = new GameboardImageArray(this, gameBoardData, top, bottom, 0, tileImagesDirPath);
                _gameBoardGui.TileClicked += new GameboardImageArray.TileClickedEventDelegate(GameTileClicked);
                _gameBoardGui.UpdateBoardGui(gameBoardData);
                IterateThroughBoard();
                UserInterface();
                SpeakSettings();
                NumberSet();
                string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
                string SaveDir = Path.Combine(ActiveDir, "saves");
                string[] saveFiles = Directory.GetFiles(SaveDir, "*.json");
                if (saveFiles.Length > 0) { SaveFilesload(saveFiles); }
            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show(ex.ToString(), "GAME BOARD SIZE PROBLEM", MessageBoxButtons.OK);
                this.Close();
            }

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


        public (bool IsValid, List<Point> CapturedPoints) IsValidMove(int row, int col, int[,] gameboarddata, int TurnCounter1)
        {
            List<Point> capturedPoints = new List<Point>();


            if (gameboarddata[row, col] == 0 || gameboarddata[row, col] == 100)
            {

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

                    if (IsWithinBoard(newX, newY) && gameboarddata[newX, newY] != TurnCounter1)
                    {
                        int opponentColor = TurnCounter1 == 1 ? 10 : 1;
                        List<Point> tempCapturedPoints = new List<Point>();

                        while (IsWithinBoard(newX, newY) && gameboarddata[newX, newY] == opponentColor)
                        {
                            tempCapturedPoints.Add(new Point(newX, newY));
                            newX += offset.dx;
                            newY += offset.dy;
                        }

                        if (IsWithinBoard(newX, newY) && gameboarddata[newX, newY] == TurnCounter1 && tempCapturedPoints.Count > 0)
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
                    var (isValid, capturedPoints) = IsValidMove(row, col, gameBoardData, TurnCounter);

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
                    var (isValid, _) = IsValidMove(row, col, gameBoardData, TurnCounter);
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
            if (TurnCounter == 10)
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





        private void VirtualPlayerTurn()
        {

            int maxDepth = 5;
            int timeThreshold = 1000;

            StartStopwatch();

            int alpha = int.MinValue;
            int beta = int.MaxValue;

            int player = TurnCounter;
            int opponent = (player == 1) ? 10 : 1;

            int bestRow = -1;
            int bestCol = -1;
            int bestScore = int.MinValue;

            for (int depth = 1; depth <= maxDepth; depth++)
            {
                int currentBestScore = int.MinValue;

                for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
                {
                    for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                    {
                        var (isValid, capturedPoints) = IsValidMove(row, col, gameBoardData, player);
                        if (isValid)
                        {
                            int[,] clonedBoard = (int[,])gameBoardData.Clone();
                            SetAITile(row, col, clonedBoard, player);
                            int score = Minimax(clonedBoard, depth - 1, opponent, player, alpha, beta);

                            if (score > currentBestScore)
                            {
                                currentBestScore = score;
                                bestRow = row;
                                bestCol = col;
                            }


                            alpha = Math.Max(alpha, currentBestScore);


                            if (beta <= alpha)
                            {
                                break;
                            }
                        }
                    }
                }

                if (HasTimeExceededThreshold(timeThreshold))
                {
                    break;
                }


                bestScore = currentBestScore;
            }


            if (bestRow != -1 && bestCol != -1)
            {




                var (isValid, capturedPoints) = IsValidMove(bestRow, bestCol, gameBoardData, 10);

                if (isValid)
                {
                    int color = 10;
                    DisposeOfValidMoves();

                    _gameBoardGui.SetTile(bestRow, bestCol, color.ToString());
                    gameBoardData[bestRow, bestCol] = TurnCounter;

                    var coordinate = (bestRow.ToString() + "," + bestCol.ToString());


                    string GameTiles = string.Join(", ", capturedPoints);


                    foreach (Point capturedPoint in capturedPoints)
                    {
                        gameBoardData[capturedPoint.X, capturedPoint.Y] = TurnCounter;
                        _gameBoardGui.SetTile(capturedPoint.X, capturedPoint.Y, color.ToString());
                    }

                    Console.WriteLine(GameTiles);
                    if (toolStripMenuItem1.Checked)
                    {

                        speaker.SpeakAsync("Player placed counter at " + coordinate);
                        speaker.SpeakAsync("tile has flipped at " + GameTiles);

                        if (TurnCounter == 1)
                        {
                            speaker.SpeakAsync("Whites Turn");
                        }
                        else
                        {
                            speaker.SpeakAsync("Blacks Turn");
                        }



                    }
                    Console.WriteLine(TurnCounter);
                    TurnCounter = (TurnCounter == 1) ? 10 : 1;
                    Console.WriteLine(TurnCounter);

                    IterateThroughBoard();
                }
                UserInterface();
            }
        }
        private void StartStopwatch()
        {
            stopwatch.Start();
        }


        private bool HasTimeExceededThreshold(int milliseconds)
        {
            if (stopwatch.ElapsedMilliseconds > milliseconds)
            {

                stopwatch.Stop();
                stopwatch.Reset();
                return true;
            }
            return false;
        }


        private int Minimax(int[,] board, int depth, int player, int originalPlayer, int alpha, int beta)
        {
            if (depth == 0 || IsTerminal(board))
            {
                combinationCount++;
                return EvaluateBoard(board, originalPlayer);
            }

            if (player == originalPlayer)
            {
                int bestScore = int.MinValue;

                for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
                {
                    for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                    {
                        var (isValid, capturedPoints) = IsValidMove(row, col, board, player);
                        if (isValid)
                        {
                            int[,] clonedBoard = (int[,])board.Clone();
                            SetAITile(row, col, clonedBoard, player);
                            int score = Minimax(clonedBoard, depth - 1, GetOpponent(player), originalPlayer, alpha, beta);
                            bestScore = Math.Max(bestScore, score);
                            alpha = Math.Max(alpha, bestScore);
                            if (beta <= alpha)
                            {
                                break; // Beta pruning
                            }
                        }
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
                {
                    for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                    {
                        var (isValid, capturedPoints) = IsValidMove(row, col, board, player);
                        if (isValid)
                        {
                            int[,] clonedBoard = (int[,])board.Clone();
                            SetAITile(row, col, clonedBoard, player);
                            int score = Minimax(clonedBoard, depth - 1, GetOpponent(player), originalPlayer, alpha, beta);
                            bestScore = Math.Min(bestScore, score);
                            beta = Math.Min(beta, bestScore);
                            if (beta <= alpha)
                            {
                                break; // Alpha pruning
                            }
                        }
                    }
                }

                return bestScore;
            }
        }


        private bool IsTerminal(int[,] board)
        {
            // Implement your terminal state conditions here, e.g., no valid moves left
            // or a full board.
            return !AnyValidMoveLeft(1) && !AnyValidMoveLeft(10);
        }

        private int GetOpponent(int player)
        {
            return (player == 1) ? 10 : 1;
        }


        private int EvaluateBoard(int[,] board, int player)
        {
            int opponent = GetOpponent(player); // Get the opponent's player ID

            int playerCount = 0;
            int opponentCount = 0;

            for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
            {
                for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                {
                    if (board[row, col] == player)
                    {
                        playerCount++;
                    }
                    else if (board[row, col] == opponent)
                    {
                        opponentCount++;
                    }
                }
            }


            int score = playerCount - opponentCount;

            return score;
        }



        public void SetAITile(int row, int col, int[,] AIGameBoard, int player)
        {
            var (isValid, capturedPoints) = IsValidMove(row, col, AIGameBoard, player);

            AIGameBoard[row, col] = player;
            foreach (Point capturedPoint in capturedPoints)
            {
                AIGameBoard[capturedPoint.X, capturedPoint.Y] = player;

            }

        }





        public void GameTileClicked(object sender, EventArgs e)
        {
            int selectionRow = _gameBoardGui.GetCurrentRowIndex(sender);
            int selectionCol = _gameBoardGui.GetCurrentColumnIndex(sender);

            var (isValid, capturedPoints) = IsValidMove(selectionRow, selectionCol, gameBoardData, TurnCounter);

            if (textBox1.Text == "")
            {
                textBox1.Text = "Player #1 ";
            }
            if (textBox2.Text == "")
            {
                textBox2.Text = "Player #2 ";
            }


            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;


            if (isValid)
            {
                int color = TurnCounter;
                DisposeOfValidMoves();

                _gameBoardGui.SetTile(selectionRow, selectionCol, color.ToString());
                gameBoardData[selectionRow, selectionCol] = TurnCounter;

                var coordinate = (selectionRow.ToString() + "," + selectionCol.ToString());


                string GameTiles = string.Join(", ", capturedPoints);

                // Set all captured points to the current player's color
                foreach (Point capturedPoint in capturedPoints)
                {
                    gameBoardData[capturedPoint.X, capturedPoint.Y] = TurnCounter;
                    _gameBoardGui.SetTile(capturedPoint.X, capturedPoint.Y, color.ToString());
                }

                Console.WriteLine(GameTiles);

                if (toolStripMenuItem1.Checked)
                {

                    speaker.SpeakAsync("Player placed counter at " + coordinate);
                    speaker.SpeakAsync("tile has flipped at " + GameTiles);

                    if (TurnCounter == 1)
                    {
                        speaker.SpeakAsync("Whites Turn");
                    }
                    else
                    {
                        speaker.SpeakAsync("Blacks Turn");
                    }



                }

                TurnCounter = (TurnCounter == 1) ? 10 : 1;

                // Redisplay valid moves for the next player
                IterateThroughBoard();
            }
            if (TurnCounter == 10)
            {

                if (virtualplayer) { VirtualPlayerTurn(); }

            }
            else
            {

            }
            Console.WriteLine(combinationCount);
            SpeakSettings();
            UserInterface();

        }

        public void SpeakSettings()
        {
            var SpeakItem = toolStripMenuItem1;


            SpeechSynthesizer speaker = new SpeechSynthesizer(); // Create a new instance here

            SpeakItem.Click += (sender, e) =>
            {
                checkBoxClicks += 1;
                
                if (checkBoxClicks % 2 == 0) 
                {
                    try
                    {
                        speaker.SetOutputToDefaultAudioDevice();
                    }
                    catch
                    {

                    }
                    SpeakItem.CheckState = CheckState.Checked;
                    speaker.SpeakAsync("Dictation is on.");
                }
                else 
                {
                    SpeakItem.CheckState = CheckState.Unchecked;
                }
            };

        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Unsaved progress, Do you want to continue?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                int[,] BoardArray = new int[NUM_OF_BOARD_ROWS, NUM_OF_BOARD_COL];
                gameBoardData = BoardArray;
                gameBoardData = this.MakeBoardArray();
                _gameBoardGui.UpdateBoardGui(gameBoardData);
                TurnCounter = 1;
                IterateThroughBoard();

            }

        }
        private void saveGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!saveGameEnabled)
            {
                return; // Exit the event handler if saving is disabled
            }


            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveDir = Path.Combine(ActiveDir, "saves");

            int[,] NewBoardArray = new int[NUM_OF_BOARD_ROWS, NUM_OF_BOARD_COL];
            NewBoardArray = gameBoardData;

            int numRows = NewBoardArray.GetLength(0);
            int numCols = NewBoardArray.GetLength(1);

            int[][] jaggedArray = new int[numRows][];

            for (int i = 0; i < numRows; i++)
            {
                jaggedArray[i] = new int[numCols];
                for (int j = 0; j < numCols; j++)
                {
                    jaggedArray[i][j] = NewBoardArray[i, j];
                }
            }
            var saveData = new
            {
                GameBoardData = jaggedArray,
                TurnCounter = TurnCounter,
                BlackCounterName = textBox1.Text,
                WhiteCounterName = textBox2.Text
            };

            string saveDataJson = JsonSerializer.Serialize(saveData);

            if (num != 6)
            {
                string saveFilePath = Path.Combine(SaveDir, "gamesave" + num.ToString() + ".json");
                var TempNum = num;
                if (!Directory.Exists(saveFilePath))
                {
                    File.WriteAllText(saveFilePath, saveDataJson);
                }
                else
                {
                    MessageBox.Show("Cmom man");
                }


                saveFilePath = Path.Combine(SaveDir, "gamesave" + TempNum.ToString() + ".json");
                List<string> list = new List<string>();


                if (num == 2)
                {
                    TempNum--;
                    var saveFilePath1 = Path.Combine(SaveDir, "gamesave" + TempNum.ToString() + ".json");
                    list.Add(saveFilePath1);
                }
                list.Add(saveFilePath);
                string[] saveFiles = list.ToArray();
                SaveFilesload(saveFiles);

            }
            else
            {
                string Overwrite = Path.Combine(ActiveDir, "saves");
                string[] saveFiles = Directory.GetFiles(SaveDir, "*.json");
                MessageBox.Show("Maximum Save Game Reached, Please select save to overwrite", "Error", MessageBoxButtons.OK);
                foreach (string saveFile in saveFiles)
                {

                    string fileName = Path.GetFileNameWithoutExtension(saveFile);
                    var LoadedGame = new ToolStripMenuItem(fileName);
                    saveGameEnabled = false;
                    LoadedGame.Click += (sender, e) =>
                    {
                        string fileName = Path.GetFileNameWithoutExtension(saveFile);
                        OverWriteData(fileName);
                    };

                    saveGameToolStripMenuItem1.DropDownItems.Add(LoadedGame);
                }
            }
        }

        public void SaveFilesload(string[] Dir)
        {

            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveDir = Path.Combine(ActiveDir, "saves");
            string[] saveFiles = Dir.Select(x => x.ToString()).ToArray();
            int SaveNum = saveFiles.Length;

            if (num > 1)
            {
                num++;
                foreach (string saveFile in saveFiles)
                {

                    string fileName = Path.GetFileNameWithoutExtension(saveFile);
                    var LoadedGame = new ToolStripMenuItem(fileName);


                    LoadedGame.Click += (sender, e) =>
                    {
                        var result = DecodeJSON(saveFile);
                        var dynamicData = result.DynamicData;
                        var multidimensionalArray = result.MultidimensionalArray;
                        int turnCounter = dynamicData.TurnCounter;

                        textBox1.Text = dynamicData.BlackCounterName;
                        textBox2.Text = dynamicData.WhiteCounterName;
                        gameBoardData = multidimensionalArray;
                        TurnCounter = turnCounter;
                        _gameBoardGui.UpdateBoardGui(gameBoardData);
                        IterateThroughBoard();
                        MessageBox.Show("Loading game: " + fileName);
                    };

                    loadGameToolStripMenuItem.DropDownItems.Add(LoadedGame);
                }
            }
            else
            {
                num++;
                var saveFile = saveFiles[0];
                string fileName = Path.GetFileNameWithoutExtension(saveFile);
                var LoadedGame = loadGameToolStripMenuItem;

                LoadedGame.Click += (sender, e) =>
                {
                    var result = DecodeJSON(saveFile);
                    var dynamicData = result.DynamicData;
                    var multidimensionalArray = result.MultidimensionalArray;
                    int turnCounter = dynamicData.TurnCounter;

                    textBox1.Text = dynamicData.BlackCounterName;
                    textBox2.Text = dynamicData.WhiteCounterName;
                    gameBoardData = multidimensionalArray;
                    TurnCounter = turnCounter;
                    _gameBoardGui.UpdateBoardGui(gameBoardData);
                    IterateThroughBoard();
                    MessageBox.Show("Loading game: " + fileName);
                };

            }
        }
        public void OverWriteData(string filename)
        {
            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string Overwrite = Path.Combine(ActiveDir, "saves");


            int[,] NewBoardArray = new int[NUM_OF_BOARD_ROWS, NUM_OF_BOARD_COL];
            NewBoardArray = gameBoardData;

            int numRows = NewBoardArray.GetLength(0);
            int numCols = NewBoardArray.GetLength(1);

            int[][] jaggedArray = new int[numRows][];

            for (int i = 0; i < numRows; i++)
            {
                jaggedArray[i] = new int[numCols];
                for (int j = 0; j < numCols; j++)
                {
                    jaggedArray[i][j] = NewBoardArray[i, j];
                }
            }
            var saveData = new
            {
                GameBoardData = jaggedArray,
                TurnCounter = TurnCounter,
                BlackCounterName = textBox1.Text,
                WhiteCounterName = textBox2.Text
            };

            string saveDataJson = JsonSerializer.Serialize(saveData);


            if (!Directory.Exists(Overwrite))
            {
                Directory.CreateDirectory(Overwrite);
            }

            string saveFilePath = Path.Combine(Overwrite, filename + ".json");
            var TempNum = num;
            if (!Directory.Exists(saveFilePath))
            {
                File.WriteAllText(saveFilePath, saveDataJson);
                MessageBox.Show("Save Overwritten");
            }
            else
            {
                MessageBox.Show("Cmom man");
            }
        }
        public void NumberSet()
        {
            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveDir = Path.Combine(ActiveDir, "saves");
            string[] AllFiles = Directory.GetFiles(SaveDir, "*.json");
            foreach (string filePath in AllFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                var parts = fileName.Split("gamesave");
                if (parts.Length > 1)
                {
                    if (int.TryParse(parts[1], out int fileNum))
                    {
                        if (fileNum > num)
                        {
                            num = fileNum;

                            string[] AlFiles = Directory.GetFiles(SaveDir, "*.json");
                            if (AlFiles.Length > 0)
                            {
                                FileFirst = true;
                            }
                        }
                    }
                }
            }
        }


        private (GameData DynamicData, int[,] MultidimensionalArray) DecodeJSON(string dir)
        {
            string fileName = Path.GetFileNameWithoutExtension(dir);
            var jsondata = File.ReadAllText("saves/" + fileName + ".json");
            var dynamicData = JsonSerializer.Deserialize<GameData>(jsondata);
            var jaggedArray = dynamicData.GameBoardData;
            int numRows = jaggedArray.Length;
            int numCols = jaggedArray[0].Length;

            int[,] multidimensionalArray = new int[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    multidimensionalArray[i, j] = jaggedArray[i][j];
                }
            }

            return (dynamicData, multidimensionalArray);
        }



        public class GameData
        {
            public int[][] GameBoardData { get; set; }
            public int TurnCounter { get; set; }
            public string BlackCounterName { get; set; }
            public string WhiteCounterName { get; set; }
        }

        private void aboutMeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void virtualPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!virtualplayer) { virtualplayer = true; } else { virtualplayer = false; }

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}





