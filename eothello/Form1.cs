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

        public void ValidMove(int row, int col)
        {
            if (gameBoardData[row, col] == 0)
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

                    if (gameBoardData[newX, newY] > 0 && gameBoardData[newX, newY] != TurnCounter)
                    {
                        bool ValidPoint = false;
                        while (ValidPoint == false)
                        {
                            Point Offsetpoint = new Point(newX, newY);
                            Point OffsetBase = new Point(offset.dx, offset.dy);

                            int OffsettedPointX = Offsetpoint.X + OffsetBase.X;
                            int OffsettedPointY = Offsetpoint.Y + OffsetBase.Y;

                            Point beforelist = new Point(OffsettedPointX, OffsettedPointY);

                            List<Point> Pointlist = new List<Point>();

                            Pointlist.Add(beforelist);


                            if (gameBoardData[OffsettedPointX, OffsettedPointY] >= 0 && gameBoardData[newX, newY] != TurnCounter)
                            {
                                int color = TurnCounter;
                                _gameBoardGui.SetTile(row, col, color.ToString());
                                if (TurnCounter == 1) { TurnCounter = 10; } else { TurnCounter = 1; }
                                gameBoardData[row, col] = TurnCounter;
                                
                                ValidPoint = true; 

                               // foreach(var place in PointList)
                               // {
                                //    _gameBoardGui.SetTile(row, col, color.ToString());
                                //}

                            }
                            else if (gameBoardData[OffsettedPointX, OffsettedPointY] == 0)
                            {

                                break;
                            }

                           }
                    }
                }

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

            ValidMove(selectionRow, selectionCol);





        }


    }

}