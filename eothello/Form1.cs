using GameboardGUI;
using Microsoft.VisualBasic;
using System.Text.Json;
using System.Net.Security;
using System.Speech.Synthesis;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static eothello.ONielo;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

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
        int virtualboxclicks = 1;
        int informtationboxclick = 0;
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
                string SaveFilePath = Path.Combine(ActiveDir, "saves", "game_save.json");
                var options = new JsonSerializerOptions
                {
                    WriteIndented = false
                };

                if (File.Exists(SaveFilePath))
                {
                    SaveFilesload();
                }




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

        private async Task VirtualPlayerTurn()
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
                            int score = await Task.Run(() => Minimax(clonedBoard, depth - 1, opponent, player, alpha, beta));

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

        private async Task StartStopwatch()
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


        private async Task<int> Minimax(int[,] board, int depth, int player, int originalPlayer, int alpha, int beta)
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
                            int score = await Minimax(clonedBoard, depth - 1, GetOpponent(player), originalPlayer, alpha, beta);
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
                            int score = await Minimax(clonedBoard, depth - 1, GetOpponent(player), originalPlayer, alpha, beta);
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
        private void newGameToolStripMenuItem_Click_1(object sender, EventArgs e)
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
            var localDate = DateTime.Now;
            string defaultString = localDate.ToString();


            var saveName = Microsoft.VisualBasic.Interaction.InputBox("Please Enter Save Name", "Save Game", defaultString);

            if (string.IsNullOrEmpty(saveName))
            {
                saveName = defaultString;
            }



            if (!saveGameEnabled)
            {
                return;
            }

            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveDir = Path.Combine(ActiveDir, "saves", "game_save.json");
            string SaveDirPath = Path.Combine(ActiveDir, "saves");
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

            var saveData = new GameData
            {
                GameBoardData = jaggedArray,
                TurnCounter = TurnCounter,
                BlackCounterName = textBox1.Text,
                WhiteCounterName = textBox2.Text,
                SaveNum = num,
                SaveName = saveName
            };


            if (!File.Exists(SaveDirPath))
            {
                Directory.CreateDirectory(SaveDirPath);
            }

            string saveDataJson = JsonSerializer.Serialize(saveData);
            Console.WriteLine(num);

            if (!File.Exists(SaveDir))
            {
                string jsonArray = "[" + saveDataJson + "]";
                File.WriteAllText(SaveDir, jsonArray);
                num = 2;
            }
            else if (num != 6)
            {
                string existingData = File.ReadAllText(SaveDir);
                List<string> jsonDataList = new List<string>();

                var regex = new Regex(@"{[^}]*}");
                var matches = regex.Matches(existingData);
                foreach (Match match in matches)
                {
                    jsonDataList.Add(match.Value);
                }

                jsonDataList.Add(saveDataJson);

                string updatedJsonArray = "[" + string.Join("," + Environment.NewLine, jsonDataList) + "]";

                File.WriteAllText(SaveDir, updatedJsonArray);
                SaveFilesload();
                num++;
            }
            else
            {
                MessageBox.Show("Save Folder is full select which save to overwrite");
                string existingData = File.ReadAllText(SaveDir);
                List<string> jsonDataList = new List<string>();

                var regex = new Regex(@"{[^}]*}");
                var matches = regex.Matches(existingData);
                foreach (Match match in matches)
                {
                    jsonDataList.Add(match.Value);
                }

                List<GameData> saveDataList = JsonSerializer.Deserialize<List<GameData>>(existingData);

                int tempnum1 = 1;
                foreach (string item in jsonDataList)
                {
                    int currentTempNum = tempnum1; // Capture the current value of tempnum1 in a local variable
                    var name = saveDataList[currentTempNum - 1].SaveName;
                    var LoadedGame = new ToolStripMenuItem(name);
                    LoadedGame.Name = name; // Set the Name property to the new name
                    LoadedGame.Click += (sender, e) =>
                    {

                        OverWriteData(currentTempNum, saveData, name);
                        MessageBox.Show("Save Overwritten");
                        SaveFilesload();
                        saveGameToolStripMenuItem1.DropDownItems.Clear();
                    };
                    saveGameToolStripMenuItem1.DropDownItems.Add(LoadedGame);
                    tempnum1++;

                }
            }
        }


        public void SaveFilesload()
        {

            loadGameToolStripMenuItem.DropDownItems.Clear();
            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveFilePath = Path.Combine(ActiveDir, "saves", "game_save.json");
            string jsonData = File.ReadAllText(SaveFilePath);
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };
            List<GameData> saveDataList = JsonSerializer.Deserialize<List<GameData>>(jsonData);

            foreach (var item in saveDataList)
            {
                var jaggedArray = item.GameBoardData;
                int numRows = jaggedArray.Length;
                int numCols = jaggedArray[0].Length;
                int[,] multidimensionalArray = new int[numRows, numCols];
                var name = item.SaveName;

                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        multidimensionalArray[i, j] = jaggedArray[i][j];
                    }
                }

                var LoadedGame = new ToolStripMenuItem(name);
                LoadedGame.Click += (sender, e) =>
                {

                    textBox1.Text = item.BlackCounterName;
                    textBox2.Text = item.WhiteCounterName;
                    gameBoardData = multidimensionalArray;
                    TurnCounter = item.TurnCounter;
                    _gameBoardGui.UpdateBoardGui(gameBoardData);
                    DisposeOfValidMoves();
                    IterateThroughBoard();
                    MessageBox.Show("Loading game: " + " " + name);

                };
                loadGameToolStripMenuItem.DropDownItems.Add(LoadedGame);
            }

        }
        public void OverWriteData(int tempnum, GameData newGameData, string name)
        {
            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveFilePath = Path.Combine(ActiveDir, "saves", "game_save.json");
            string jsonData = File.ReadAllText(SaveFilePath);
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };
            List<GameData> saveDataList = JsonSerializer.Deserialize<List<GameData>>(jsonData);

            if (tempnum >= 0 && tempnum < saveDataList.Count + 1)
            {

                var CurrentFileNum = tempnum;
                tempnum--;

                saveDataList[tempnum] = newGameData;

                // Keep the original SaveNum in the new game data
                saveDataList[tempnum].SaveNum = CurrentFileNum;
                string updatedJsonArray = JsonSerializer.Serialize(saveDataList);
                File.WriteAllText(SaveFilePath, updatedJsonArray);
            }
        }




        public void NumberSet()
        {
            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveFilePath = Path.Combine(ActiveDir, "saves", "game_save.json");

            if (File.Exists(SaveFilePath))
            {
                string jsonData = File.ReadAllText(SaveFilePath);

                if (jsonData == "")
                {
                    num = 1;
                    return;
                }
                List<GameData> saveDataList = JsonSerializer.Deserialize<List<GameData>>(jsonData);

                int highestSaveNum = -1;

                foreach (var data in saveDataList)
                {
                    if (data.SaveNum > highestSaveNum)
                    {
                        highestSaveNum = data.SaveNum;
                    }
                }
                if (highestSaveNum == 5) { highestSaveNum++; }
                num = highestSaveNum;

            }
        }



        public class GameData
        {
            public int[][] GameBoardData { get; set; }
            public int TurnCounter { get; set; }
            public string BlackCounterName { get; set; }
            public string WhiteCounterName { get; set; }

            public int SaveNum { get; set; }
            public string SaveName { get; set; }
        }


        private void aboutMeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }



        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void aboutMeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Form2 f2 = new Form2();
            f2.Show();

        }

        private void gameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void virtualPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (virtualboxclicks == 1)
            {

                virtualplayer = true;
                virtualPlayerToolStripMenuItem.CheckState = CheckState.Checked;
                virtualboxclicks--;
            }
            else
            {
                virtualplayer = false;
                virtualboxclicks++;
                virtualPlayerToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void informationPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (informtationboxclick == 1)
            {
                textBox1.Show();
                textBox2.Show();
                label1.Show();
                label2.Show();
                pictureBox1.Show();
                pictureBox2.Show();
                pictureBox3.Show();
                pictureBox4.Show();
                pictureBox5.Show();
                informtationboxclick--;
                informationPanelToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                textBox1.Hide();
                textBox2.Hide();
                label1.Hide();
                label2.Hide();
                pictureBox1.Hide();
                pictureBox2.Hide();
                pictureBox3.Hide();
                pictureBox4.Hide();
                pictureBox5.Hide();
                informtationboxclick++;
                informationPanelToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void ONielo_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Unsaved progress, Do you want to continue?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {

            }
            else
            {
                e.Cancel = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}





