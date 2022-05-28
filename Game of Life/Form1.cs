using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        #region Members

        // The universe array
        bool[,] universe;

        // Drawing colors
        Color gridColor = Color.Black;
        Color gridColorx10 = Color.Black;
        Color cellColor = Color.Gray;
        Color cellAlive = Color.Green;
        Color cellDead = Color.Red;

        // The Universe Seed
        int seed = 0;

        // The Universe Dimesions
        public int uniWidth = Properties.Settings.Default.uniWidth; // Universe Width
        public int uniHeight = Properties.Settings.Default.uniHeight; // Universe Height

        // Bools
        bool isToroidal = Properties.Settings.Default.isToridial;
        bool showNeighbors = Properties.Settings.Default.showNeighbors;
        bool gridcheck = Properties.Settings.Default.gridCheck;
        bool hudCheck = Properties.Settings.Default.hudCheck;
        //Interval
        public int interval = Properties.Settings.Default.interval; // timer speed

        // Cell Count
        int CellsLiving = 0;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        #endregion

        #region Form
        public Form1()
        {
            InitializeComponent();
            // Load Settings
            LoadSettings();
            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }
        #endregion

        #region Loader
        private void LoadSettings()
        {
            // All of the settings to initialize at the start

            uniWidth = Properties.Settings.Default.uniWidth; // The universe width
            uniHeight = Properties.Settings.Default.uniHeight; // The universe height
            interval = Properties.Settings.Default.interval; //  The timer speed
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor; // The panel background
            gridColor = Properties.Settings.Default.gridColor; // The inner grid
            gridColorx10 = Properties.Settings.Default.gridColorx10; // The outer grid
            cellColor = Properties.Settings.Default.cellColor; // The living cell color
            seed = Properties.Settings.Default.Seed; // The seed to generate from
            isToroidal = Properties.Settings.Default.isToridial; // The toroidal neighbor method
            showNeighbors = Properties.Settings.Default.showNeighbors; // The display neighbors
            gridcheck = Properties.Settings.Default.gridCheck; // The display grid
            hudCheck = Properties.Settings.Default.hudCheck; // The display hud
            universe = new bool[uniWidth, uniHeight]; // The universe array
            StatusLabelInterval.Text = $"Interval = {interval}"; // The timer speed
            StatusLabelAlive.Text = $"Alive: {CellsLiving}"; // The live cells
            StatusLabelSeed.Text = $"Seed: {seed}"; // The seed to generate from
        }
        #endregion

        #region Next Generation
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            bool[,] scratchPad = new bool[uniWidth, uniHeight]; // Creates Scratchpad
            //Iterate through the universe, in the y, top / bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // then the x left / right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count = CountNeighborsFinite(x, y);

                        //Apply Rules
                    if (universe[x, y] == true && count < 2)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (universe[x, y] == true && count > 3)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (universe[x, y] == true && count == 2)
                    {
                        scratchPad[x, y] = true;
                    }
                    else if (universe[x, y] == true && count == 3)
                    {
                        scratchPad[x, y] = true;
                    }
                    else if (universe[x, y] == false && count == 3)
                    {
                        scratchPad[x, y] = true;
                    }

                }

            }

            // Copy from scratchpad to universe
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }
        #endregion

        #region Timer
        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            //Calls Next Gen
            NextGeneration();
            //Repaint
            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Paint
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Cells that are living
            int CellsLiving = 0;
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = ((float)graphicsPanel1.ClientSize.Width) / ((float)universe.GetLength(0));
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = ((float)graphicsPanel1.ClientSize.Height) / ((float)universe.GetLength(1));

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // The cells that are alive
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = (float)x * cellWidth;
                    cellRect.Y = (float)y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    int neighbors;
                    if(isToroidal == true)
                    {
                        neighbors = CountNeighborsToroidal(x, y);
                    }
                    else
                    {
                        neighbors = CountNeighborsFinite(x, y);
                    }
                    if (universe[x, y] == true)
                    {
                        CellsLiving++;
                    }
                    // Font Painter for Numbers
                    Font font = new Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Point);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
    
                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true && neighbors ==  0)
                    {
                        //Generates cells based on generation Alive / Dead.
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else if (universe[x, y] == true && neighbors < 2)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                    }
                    else if(universe[x, y] == true && neighbors > 3)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                    }
                    else if (universe[x, y] == true && neighbors == 2)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                    }
                    else if (universe[x, y] == true && neighbors == 2)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                    }
                    else if (universe[x, y] == true && neighbors == 3)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                    }
                    else if (universe[x, y] == false && neighbors == 3)
                    {
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                    }
                    else if(universe[x, y] == false && neighbors > 0)
                    {
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                    }
                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
                    //Update living cells text
                    StatusLabelAlive.Text = $"Alive: {CellsLiving}";
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }
        #endregion

        #region LMB Method
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = ((float)graphicsPanel1.ClientSize.Width) / ((float)universe.GetLength(0));
                float cellHeight = ((float)graphicsPanel1.ClientSize.Height) / ((float)universe.GetLength(1));

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float tX = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float tY = e.Y / cellHeight;
                int x = (int)tX;
                int y = (int)tY;
                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
            else if (e.Button == MouseButtons.Middle)  // Middle Mouse Button functionality to allow users to activate multiple cells at once
            {

                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();

            }
        }
        #endregion

        #region (Button)Exit, Start, Stop, Next, Skip, Slow
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) // This method exits the program when a user clicks the tool strip exit button.
        {
            this.Close();
        }

        private void Start_Click(object sender, EventArgs e) //Start Button
        {
            // Starts the game
            timer.Enabled = true;
            graphicsPanel1.Invalidate();
        }

        private void Stop_Click(object sender, EventArgs e) // Stop Button
        {
            //Stops the game
            timer.Enabled = false;
            graphicsPanel1.Invalidate();
        }

        private void Next_Click(object sender, EventArgs e) // Skip Button
        {
            NextGeneration();
            timer.Enabled = false;
            graphicsPanel1.Invalidate();
        }

        private void Slow_Click(object sender, EventArgs e) // Slow Button
        {
            timer.Interval = 1000;
            timer.Start();
            graphicsPanel1.Invalidate();
        }
        #endregion
        #region (Button)Seed Setter
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random_gen_Form random_Gen_Form = new Random_gen_Form();
            //Set and Mutate
            if (seed < 0)
            {
                seed *= -1;
            }
            random_Gen_Form.theSeed = seed;
            if (DialogResult.OK == random_Gen_Form.ShowDialog())
            {
                // Get and Retrieve/Access
                seed = random_Gen_Form.theSeed;
                // Randomize  Method to fill entire universe with a entirely new seed.
                Randomize();
            }
            // Display the new seed Status
            StatusLabelSeed.Text = $"Seed: {seed}";
            //repaint
            graphicsPanel1.Invalidate();
        }
        #endregion
        #region  (Button)New Universe
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                //Iterate through the universe in the x. left to right
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            // Reset the generations back to zero
            generations = 0;
            // Update the generations text to present a new slate
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Count Finite / Toroidal
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    // if xCheck is less than 0 then set to xLen - 1
                    // if yCheck is less than 0 then set to yLen - 1
                    // if xCheck is greater than or equal too xLen then set to 0
                    // if yCheck is greater than or equal too yLen then set to 0

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        #endregion

        #region Randomize Method
        private void Randomize()
        {
            //Construct the Random and give it a seed.
            Random random = new Random();
            // Iterate through the universe(y), from top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe(x), from top to bottom
                for ( int x = 0; x < universe.GetLength(0); x++)
                {
                    // 1 of 3 Cells made Alive
                    int rand = random.Next(-1,2);
                    if(rand == -1) // If rand is less than 1 then do>
                    {
                        universe[x, y] = true; // sets Alive
                    }
                    else
                    {
                        universe[x, y] = false; // sets Dead
                    }

                }
            }
        }
        #endregion

        #region File System
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!This is my comment.");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
     {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
          {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y] == true)
                        {
                            currentRow += "O";
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else if (universe[x, y] == false)
                        {
                            currentRow = ".";
                        }
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if(row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    else
                    {
                        maxHeight++;
                    }
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    if(row.Length > maxWidth)
                    {
                        maxWidth = row.Length;
                    }
                }
                //setting y pos
                int yPos = 0;
                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                uniWidth = maxWidth;
                uniHeight = maxHeight;
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    else
                    {
                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                            {
                                universe[xPos, yPos] = true;
                            }
                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                            else if (row[xPos] == '.')
                            {
                                universe[xPos, yPos] = false;
                            }
                            yPos++;
                        }
                    }
                }

                // Close the file.
                reader.Close();
                universe = new bool[uniWidth, uniHeight]; // the universe array
                // Repaint
                graphicsPanel1.Invalidate();
            }
        }
        #endregion


        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Construct the options form
            Options optionsDialog = new Options();
            //Check to see if universe updates
            int tempW = uniWidth;
            int tempH = uniHeight;
            //The Setters
            optionsDialog.TimeInterval = interval;  // Timer Speed
            optionsDialog.UniverseWidth = uniWidth;   // The Universe Width
            optionsDialog.UniverseHeight = uniHeight; // The Universe Height
            if (DialogResult.OK == optionsDialog.ShowDialog())
            {
                // The Getters
                uniHeight = optionsDialog.UniverseHeight; // The Universe Height
                uniWidth = optionsDialog.UniverseWidth;   // The Universe Width
                interval = optionsDialog.TimeInterval;  // The Timer Speed
                // Update the timer speed
                timer.Interval = interval;
                // Update the timer status Label
                StatusLabelInterval.Text = $"Interval: {interval}";
            }
            // Check to see if the universe is different
            if (tempW != uniWidth || tempH != uniHeight)
            {
                // Get a copy
                bool[,] temp = universe;
                for (int y = 0; y < temp.GetLength(1); y++)
                {
                    // Iterate through the universe in the x, left to right
                    for (int x = 0; x < temp.GetLength(0); x++)
                    {
                        // And refill the array
                        temp[x, y] = universe[x, y];
                    }
                }
                // Then new
                universe = new bool[uniWidth, uniHeight];
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Iterate through the universe in the x, left to right
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // Makesure it's in range
                        if (temp.GetLength(1) > y && temp.GetLength(0) > x)
                        {
                            // Refill Array
                            universe[x, y] = temp[x, y];
                        }
                    }
                }
            }
            // Repaint
            graphicsPanel1.Invalidate();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reload Save
            Properties.Settings.Default.Reload();
            // Load Settings
            LoadSettings();
            // Repaint
            graphicsPanel1.Invalidate();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset back to default
            Properties.Settings.Default.Reset();
            // Load Settings
            LoadSettings();
            // Repaint
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Keep the tool strip equal to the context menu
            gridToolStripMenuItem.Checked = gridToolStripMenuItem.Checked;
            gridcheck = gridToolStripMenuItem.Checked;
            // Repaint
            graphicsPanel1.Invalidate();
        }

        #region Finite and Toroidal Buttons
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Keep Toroidal and Finite opposite
            if (toroidalToolStripMenuItem.Checked == false)
            {
                finiteToolStripMenuItem.Checked = true;
                isToroidal = false;
            }
            else
            {
                finiteToolStripMenuItem.Checked = false;
                isToroidal = true;

            }
            // Repaint
            graphicsPanel1.Invalidate();
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Keep Toroidal and Finite opposite
            if (finiteToolStripMenuItem.Checked == false)
            {
                toroidalToolStripMenuItem.Checked = true;
                isToroidal = true;
            }
            else
            {
                toroidalToolStripMenuItem.Checked = false;
                isToroidal = false;

            }
            // Repaint
            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Form Closed
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.uniHeight = uniHeight; ;
            Properties.Settings.Default.uniWidth = uniWidth;
            Properties.Settings.Default.interval = interval;
        }
        #endregion
    }
}
