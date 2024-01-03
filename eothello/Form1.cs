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
        /// <summary>
        ///  GLobal Variables for different UI functions and calculations
        /// <param name="NUM_OF_BOARD_ROWS/COL"> is equal to the board width/height, needed for the constructor.</param>
        /// <param name="_gameboardGui"> control for gameboard, Requried for constructor</param>
        /// <param name="gameboardData">Required by constructor, GameBoardData 2D Array that logs the game moves</param>
        /// <param name="tileImagesDirPath"> Image path for board, required by constructor</param>
        /// <param name="TurnCounter">Counter calculate player turn, Cycles between 1 and 10, 1 = black and 10 = white.</param>
        /// <param name="num">This Number is required for the SaveGame and LoadGame Feature, calculates the number of saves NumberSet() method sets global var </param>
        /// <param name="speaker">Creates speechsynth for accessablity element</param>
        /// <param name="stopwatch">Required for Search algorithm, allows timeout period for the search to break loop if happening for too long</param>
        /// <param name="combinationCount">Debugging Setting for search algorithm, Nice little statistic to work out how many board configurations have been searched </param>
        /// <param name="checkBoxClicks, virtualboxclicks, informatationboxclick">variable to cycle between clicked states on a checkbox within winforms 1 = checked </param>
        /// </summary>


        const int NUM_OF_BOARD_ROWS = 8;
        const int NUM_OF_BOARD_COL = 8;
        GameboardImageArray _gameBoardGui;
        int[,] gameBoardData;
        string tileImagesDirPath = "Resources/";
        public int TurnCounter = 1;
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
            /// <summary>
            /// Constructor, allows communcation between GameBoardImageArray.cs, in this case builds 8x8 grids using NUM_OF_BOARD_ROWs/COLS
            /// in my case, calculates possible moves for the board, and displays them for the first move, while also intialising the SpeakSettings() for voice Dictation, and setting the "num" variable in NumberSet();
            /// Also Hides or shows the load button based on the amount of saves inside the "saves" folder in the projects root dir
            /// </summary>
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
                else { loadGameToolStripMenuItem.Visible = false; }




            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show(ex.ToString(), "GAME BOARD SIZE PROBLEM", MessageBoxButtons.OK);
                this.Close();
            }

        }

        // Builds array for the constructor, allows me to edit the configuration of the starting board, returns BoardArray to gameboarddata
        private int[,] MakeBoardArray()
        {
            int[,] BoardArray = new int[NUM_OF_BOARD_ROWS, NUM_OF_BOARD_COL];

            BoardArray[3, 3] = 10;
            BoardArray[4, 3] = 1;
            BoardArray[3, 4] = 1;
            BoardArray[4, 4] = 10;
            return BoardArray;


        }


        /// <param name="selectedRow"></param>
        /// <param name="selectedCol"></param>
        /// <param name="gameBoardData"></param>
        /// <param name="currentTurn"></param>
        
        /// Tuple that takes named variables above, and calculates if there is a possible move to be performed, returns CapturedPoints and validity to each empty sqaure its activated on. 
        public (bool IsValid, List<Point> CapturedPoints) IsValidMove(int selectedRow, int selectedCol, int[,] gameBoardData, int currentTurn)
        {
        List<Point> capturedPoints = new List<Point>();
            /// First, it checks if the selected sqaure is a empty or "valid" sqaure to be calculated, this means the value of the square is 0 or 100.
            if (gameBoardData[selectedRow, selectedCol] == 0 || gameBoardData[selectedRow, selectedCol] == 100)
            {
            // Then, the method builds a list of Points, these points are known as the offset to the base selectedRow/Col, offseting itself -1 or +1 in each coordindal direction, to detect any nearby sqaures with the opposite color
            Point selectedPosition = new Point(selectedRow, selectedCol);
            (int dx, int dy)[] moveOffsets =
            {
                (1, 0), (1, 1), (0, 1), (-1, 1),
                (-1, 0), (-1, -1), (0, -1), (1, -1)
            };

            foreach (var offset in moveOffsets)
            {
                int newX = selectedPosition.X + offset.dx;
                int newY = selectedPosition.Y + offset.dy;
                    // after ,the method checks if the offset chosen is within the board rows (if more than 8, 8 in any coordinal direction in this case) 
                    // if that condition is passed, it allows the board to check for opposite colours than opposite than current turn,
                    if (WithinBoard(newX, newY) && gameBoardData[newX, newY] != currentTurn)
                    {
                    int opponentColor = currentTurn == 1 ? 10 : 1;
                    List<Point> tempCapturedPoints = new List<Point>();


                    // when this condition is passed, it will then save the offset
                    while (WithinBoard(newX, newY) && gameBoardData[newX, newY] == opponentColor)
                    {
                        tempCapturedPoints.Add(new Point(newX, newY));
                        newX += offset.dx;
                        newY += offset.dy;
                    }

                    if (WithinBoard(newX, newY) && gameBoardData[newX, newY] == currentTurn && tempCapturedPoints.Count > 0)
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

        // Calculates the overflow limit for current setting. 
        private bool WithinBoard(int x, int y)
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

        // condition to calculate if the game is terminal or not, used for Searcha algorithm terminality and normal players 
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

        // Shows winners and counts the amount of each counter on the board for UI elements and board evaulation 
        public (int Player1Count, int Player10Count) CalculateWinner()
        {
            int player1Count = 0;
            int player10Count = 0;

            // Count the pieces for each player
            for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
            {
                for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                {
                    // if Counter is black, add player count to associated player
                    if (gameBoardData[row, col] == 1)
                    {
                        player1Count++;

                    }
                    // if Counter is White, add player count to associated player
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

        // Settings for UserInterface even when game is terminal 
        public void UserInterface()
        {
            var (player1Count, player10Count) = CalculateWinner(); // Runs every turn to update UI elements 
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


        // Search algorithm for virtual player option
        private async Task VirtualPlayerTurn()
        {
            int maxDepth = 5; // How many turns in front before board evaluation
            int timeThreshold = 1000; // Stopwatch, if board searches for more than 1second terminate search

            StartStopwatch();

            int alpha = int.MinValue; // Alpha/Beta Var for Beta pruning implementation, minimising the amount of searches per game tree based on previous result of previous branch
            int beta = int.MaxValue; 

            int player = TurnCounter;
            int opponent = (player == 1) ? 10 : 1;

            int bestRow = -1; // Variables for the best possible move at the end of evaluation 
            int bestCol = -1;
            int bestScore = int.MinValue; // Variable to calculate best row/col based on the evaluation score in the entire search tree 

            for (int depth = 1; depth <= maxDepth; depth++)
            {
                int currentBestScore = int.MinValue;

                for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
                {
                    for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                    {
                        // For every row, if the move is a valid move calculate the evaluation for each possible move from this move. only one layer deep
                        var (isValid, capturedPoints) = IsValidMove(row, col, gameBoardData, player);
                        if (isValid)
                        {
                            int[,] clonedBoard = (int[,])gameBoardData.Clone();
                            SetAITile(row, col, clonedBoard, player);
                            // calculates score depending on the depth, CalculateAIMove is the loop for the tree. 
                            int score = await Task.Run(() => CalculateAIMove(clonedBoard, depth - 1, opponent, player, alpha, beta));
                            // Sets best row and column based on previous ValidMove Scanned
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


                // Sets tile and comfirms validity of the move, allows virtual player to make turn on the real board.  
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

                    // AI player speach synth
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

        // Async task for search timeout
        private async Task StartStopwatch()
        {
            stopwatch.Start();
        }

        // Timeout threshold 
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

        // Logic for virtual board created by the virtual player, Allows the board to be predicted based on itself
        private async Task<int> CalculateAIMove(int[,] board, int depth, int player, int originalPlayer, int alpha, int beta)
        {
            // Base case: reached the specified depth or terminal state
            if (depth == 0 || IsTerminal(board))
            {
                combinationCount++;
                return EvaluateBoard(board, originalPlayer);
                // Depth Calculation 
            }
            // Maximize the score if it's the original player's turn
            if (player == originalPlayer)
            {
                int bestScore = int.MinValue;
                // Iterate over each possible move on the board
                for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
                {
                    for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                    {
                        // for each valid item in board, calculate the best evaluation 
                        var (isValid, capturedPoints) = IsValidMove(row, col, board, player);
                        if (isValid)
                        {
                            int[,] clonedBoard = (int[,])board.Clone();
                            SetAITile(row, col, clonedBoard, player);
                            int score = await CalculateAIMove(clonedBoard, depth - 1, GetOpponent(player), originalPlayer, alpha, beta);
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
            else // Switch from Min Value to Max 
            {
                int bestScore = int.MaxValue;
                // Iterate over each possible move on the board
                for (int row = 0; row < NUM_OF_BOARD_ROWS; row++)
                {
                    for (int col = 0; col < NUM_OF_BOARD_COL; col++)
                    {
                        var (isValid, capturedPoints) = IsValidMove(row, col, board, player);
                        if (isValid)
                        {
                            int[,] clonedBoard = (int[,])board.Clone();
                            SetAITile(row, col, clonedBoard, player);
                            int score = await CalculateAIMove(clonedBoard, depth - 1, GetOpponent(player), originalPlayer, alpha, beta);
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

        // Checks if the game has reached a terminal state
        private bool IsTerminal(int[,] board)
        {

            return !AnyValidMoveLeft(1) && !AnyValidMoveLeft(10);
        }

        // Gets the opponent's player ID
        private int GetOpponent(int player)
        {
            return (player == 1) ? 10 : 1;
        }

        // Evaluates the current state of the board based on the player's perspective
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


        // Method for placing tile on the virtual board 
        public void SetAITile(int row, int col, int[,] AIGameBoard, int player)
        {
            var (isValid, capturedPoints) = IsValidMove(row, col, AIGameBoard, player);

            AIGameBoard[row, col] = player;
            foreach (Point capturedPoint in capturedPoints)
            {
                AIGameBoard[capturedPoint.X, capturedPoint.Y] = player;

            }

        }

        // Tile Click Event
        public void GameTileClicked(object sender, EventArgs e)
        {
            // Extract row and column indices from the clicked tile
            int selectionRow = _gameBoardGui.GetCurrentRowIndex(sender);
            int selectionCol = _gameBoardGui.GetCurrentColumnIndex(sender);

            // Pull IsValid And capturedPoints from s
            var (isValid, capturedPoints) = IsValidMove(selectionRow, selectionCol, gameBoardData, TurnCounter);

            // Set default names if not provided
            if (textBox1.Text == "")
            {
                textBox1.Text = "Player #1 ";
            }
            if (textBox2.Text == "")
            {
                textBox2.Text = "Player #2 ";
            }

            // nametag readonly
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
                    // Speak the move details if text-to-speech is enabled
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
        // Configures settings for the speech synthesizer
        public void SpeakSettings()
        {
            var SpeakItem = toolStripMenuItem1;


            SpeechSynthesizer speaker = new SpeechSynthesizer(); // Create a new instance here

            SpeakItem.Click += (sender, e) =>
            {
                if (checkBoxClicks == 1)
                {
                    try
                    {
                        speaker.SetOutputToDefaultAudioDevice();
                    }
                    catch
                    {
                        // Handle other cases if needed
                    }
                    SpeakItem.CheckState = CheckState.Checked;
                    speaker.SpeakAsync("Dictation is on.");
                    checkBoxClicks--;
                }
                else
                {
                    checkBoxClicks++;
                    SpeakItem.CheckState = CheckState.Unchecked;
                }
            };

        }


        // Starts a new game, resets the board, and updates the display
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
            // User input for save name
            var saveName = Microsoft.VisualBasic.Interaction.InputBox("Please Enter Save Name", "Save Game", defaultString); 

            if (string.IsNullOrEmpty(saveName))
            {
                saveName = defaultString; // Set default value as local date if the string is empty
            }


            // Check if saving is enabled
            if (!saveGameEnabled)
            {
                return;
            }


            // Get directory for the save game file 
            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveDir = Path.Combine(ActiveDir, "saves", "game_save.json");
            string SaveDirPath = Path.Combine(ActiveDir, "saves");
            int[,] NewBoardArray = new int[NUM_OF_BOARD_ROWS, NUM_OF_BOARD_COL];
            NewBoardArray = gameBoardData;

            int numRows = NewBoardArray.GetLength(0);
            int numCols = NewBoardArray.GetLength(1);

            int[][] jaggedArray = new int[numRows][];

            // turn 2D array into jagged array
            for (int i = 0; i < numRows; i++)
            {
                jaggedArray[i] = new int[numCols];
                for (int j = 0; j < numCols; j++)
                {
                    jaggedArray[i][j] = NewBoardArray[i, j];
                }
            }

            // Create GameData object for serialization
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
            // Convert GameData object to JSON
            string saveDataJson = JsonSerializer.Serialize(saveData);
            Console.WriteLine(num);

            if (!File.Exists(SaveDir))
            {
                // Create file and write data inside

                string jsonArray = "[" + saveDataJson + "]";
                File.WriteAllText(SaveDir, jsonArray);
                num = 2;
                loadGameToolStripMenuItem.Visible = true;
                SaveFilesload();

            }
            else if (num != 6)
            {
                // Append onto the existing JSON file
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
                // Overwrite data, compare data, and replace data within each list
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
                    // Generate click even on save dropdown to replace
                    int currentTempNum = tempnum1; 
                    var name = saveDataList[currentTempNum - 1].SaveName;
                    var LoadedGame = new ToolStripMenuItem(name);
                    LoadedGame.Name = name; 
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

        // Loads the list of saved games for display in the UI
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
        // Replaces existing game data with new game data in the save file
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



        //  Set Global varible "num" to the correct index in the save game json, allowing for the saving to work properly and index properly each time the program is restarted.
        public void NumberSet()
        {
            string ActiveDir = AppDomain.CurrentDomain.BaseDirectory;
            string SaveFilePath = Path.Combine(ActiveDir, "saves", "game_save.json");
            var count = 0;
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
                    count++;
                }
                if (count == 5) { count++; }
                num = count + 1;

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





