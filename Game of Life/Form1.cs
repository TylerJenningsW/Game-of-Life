using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[10, 10];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            bool[,] scratchPad = new bool[10, 10];
            for (int y = 0; y < universe.GetLength(1); y++)
            {
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

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = ((float)graphicsPanel1.ClientSize.Width) / ((float)universe.GetLength(0));
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = ((float)graphicsPanel1.ClientSize.Height) / ((float)universe.GetLength(1));

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

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
                    // Font Painter for Numbers
                    Font font = new Font("Arial", 20f, FontStyle.Bold, GraphicsUnit.Point);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    int neighbors = CountNeighborsFinite(x,y);


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
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

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
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) // This method exits the program when a user clicks the tool strip exit button.
        {
            this.Close();
        }

        private void ToolStripButton1_Click(object sender, EventArgs e) //Start Button
        {
            timer.Interval = 100;
            timer.Start();
            graphicsPanel1.Invalidate();
        }

        private void ToolStripButton2_Click(object sender, EventArgs e) // Stop Button
        {
            timer.Stop();
            graphicsPanel1.Invalidate();
        }

        private void ToolStripButton3_Click(object sender, EventArgs e) // Skip Button
        {
            NextGeneration();
            timer.Stop();
            graphicsPanel1.Invalidate();
        }

        private void ToolStripButton4_Click(object sender, EventArgs e) // Slow Button
        {
            timer.Interval = 1000;
            timer.Start();
            graphicsPanel1.Invalidate();
        }



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
    }
}
