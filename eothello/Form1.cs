using GameboardGUI; 

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

        public void GameTileClicked(object sender, EventArgs e)
        {
            int selectionRow = _gameBoardGui.GetCurrentRowIndex(sender);
            int selectionCol = _gameBoardGui.GetCurrentColumnIndex(sender);
            
               // GAME BOARD DATA IS UR CLUE FUTRUE MITCHELL

               // UPDATE IT WITH THE TURN OF THE PLAYER SO 10 or 1

            // SO THE ARRAY SHOWS VALUE 10 EVERYTIME YOU CLICK 

            // THEN USE THAT TO DETERMINE IF CLICkABLE

            int color = TurnCounter;
                _gameBoardGui.SetTile(selectionRow, selectionCol, color.ToString());
            if (TurnCounter == 1) {TurnCounter = 10;} else { TurnCounter = 1;}
            




        }


    }
    
}