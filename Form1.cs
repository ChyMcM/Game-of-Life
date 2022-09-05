using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GOL4
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[100, 100];
        bool[,] status = new bool[100, 100];

        // Drawing colors
        Color gridColor = Color.LightPink;
        Color cellColor = Color.PaleGreen;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        //Living cell count
        int livingcells = 0;
        //count type
        bool fin = true;
        bool tor = false;
        //show count
        bool show = false;
        //randomizer's seed
        int seed = 10;
        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
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
                  if(xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                  if(xCheck < 0)
                    {
                        continue;
                    }
                  if(yCheck < 0)
                    {
                        continue;
                    }
                  if(xCheck >= xLen)
                    {
                        continue;
                    }
                  if(yCheck >= yLen)
                    {
                        continue;
                    }
            
                  if (universe[xCheck, yCheck] == true) count++;
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
                    if(xCheck == 0 && yCheck == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if(xCheck < 0)
                    {
                        xLen--;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if(yCheck < 0)
                    {
                        yLen--;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if(xCheck >= xLen)
                    {
                        xLen = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if(yCheck >= yLen)
                    {
                        yLen = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            if (fin == true)
            {
                for (int i = 0; i < universe.GetLength(0); i++)
                {
                    for (int j = 0; j < universe.GetLength(1); j++)
                    {
                        int livingneigh = CountNeighborsFinite(i, j);
                        if (universe[i, j] == true && livingneigh < 2)
                        {
                            status[i, j] = false;
                        }
                        else if (universe[i, j] == true && livingneigh > 3)
                        {
                            status[i, j] = false;
                        }
                        else if (universe[i, j] == false && livingneigh == 3)
                        {
                            status[i, j] = true;
                        }
                        else
                        {
                            status[i, j] = universe[i, j];
                        }
                    }
                }
            }
            if(tor == true)
            {
                for (int i = 0; i < universe.GetLength(0); i++)
                {
                    for (int j = 0; j < universe.GetLength(1); j++)
                    {
                        int livingneigh = CountNeighborsToroidal(i, j);
                        if (universe[i, j] == true && livingneigh < 2)
                        {
                            status[i, j] = false;
                        }
                        else if (universe[i, j] == true && livingneigh > 3)
                        {
                            status[i, j] = false;
                        }
                        else if (universe[i, j] == false && livingneigh == 3)
                        {
                            status[i, j] = true;
                        }
                        else
                        {
                            status[i, j] = universe[i, j];
                        }
                    }
                }
            }
            bool[,] temp = universe;
            universe = status;
            status = temp;
            graphicsPanel1.Invalidate();
            Array.Clear(status, 0, status.Length);
            // Increment generation count
            generations++;
            livingcells = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        livingcells++;
                    }
                }
            }
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel2.Text = "Living Cells = " + livingcells.ToString();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

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
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    if (show == true)
                    {
                        
                            Font font = new Font("Arial", 6f);

                            StringFormat stringFormat = new StringFormat();
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            Rectangle rect = new Rectangle(x, y, 100, 100);
                            int neighbors = 0;
                            if (tor == true)
                            {
                                neighbors = CountNeighborsToroidal(x, y);
                            }
                            else if (fin == true)
                            {
                                neighbors = CountNeighborsFinite(x, y);
                            }
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Black, cellRect, stringFormat);
                        
                    }
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            generations = 0;
            Array.Clear(universe, 0, universe.Length);
            Array.Clear(status, 0, status.Length);
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
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
                writer.WriteLine("!Conway's Game of Life!");

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
                        if(universe[x,y] == true)
                        {
                            currentRow = currentRow.Insert(x, "O");
                        }

                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else
                        {
                            currentRow = currentRow.Insert(x, ".");
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

        private void openToolStripButton_Click(object sender, EventArgs e)
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
                    if (row.StartsWith("!"))
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
                    maxWidth = row.Length;
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxWidth, maxHeight];
                status = new bool[maxWidth, maxHeight];
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                // Iterate through the file again, this time reading in the cells.
                int y = 0;
                while (!reader.EndOfStream)
                {
                    
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    int rowon = 0;
                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    
                    for(int i = 0; i < maxWidth; i++)
                    {
                        if(row.ElementAt(i) == 'O')
                        {
                            status[i, y] = true;
                        }
                        else
                        {
                            status[i, y] = false;
                        }
                    }
                    y++;
                }
                bool[,] temp = universe;
                universe = status;
                status = temp;
                graphicsPanel1.Invalidate();
                Array.Clear(status, 0, status.Length);
                // Close the file.
                reader.Close();
            }
        }
        
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Random mew = new Random();
            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int torf = mew.Next(0, 10);
                    if(torf < 5)
                    {
                        status[x, y] = false;
                    }
                    else
                    {
                        status[x, y] = true;
                    }
                }
            }
            bool[,] temp = universe;
            universe = status;
            status = temp;
            livingcells = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        livingcells++;
                    }
                }
            }
            toolStripStatusLabel2.Text = "Living Cells = " + livingcells.ToString();
            graphicsPanel1.Invalidate();
            Array.Clear(status, 0, status.Length);
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            livingcells = 0;
            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    if(universe[x,y] == true)
                    {
                        livingcells++;
                    }
                }
            }
            Array.Clear(universe, 0, universe.Length);
            Array.Clear(status, 0, status.Length);
            toolStripStatusLabel2.Text = "Living Cells = " + livingcells.ToString();
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.White;
            graphicsPanel1.Invalidate();
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Blue;
            graphicsPanel1.Invalidate();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Red;
            graphicsPanel1.Invalidate();
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Green;
            graphicsPanel1.Invalidate();
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Color.Yellow;
            graphicsPanel1.Invalidate();
        }

        private void blueToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Blue;
            graphicsPanel1.Invalidate();
        }

        private void redToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Red;
            graphicsPanel1.Invalidate();
        }

        private void greenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Green;
            graphicsPanel1.Invalidate();
        }

        private void yellowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColor = Color.Yellow;
            graphicsPanel1.Invalidate();
        }

        private void gridSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fin = true;
            tor = false;
        }

        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tor = true;
            fin = false;
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(show == false)
            {
                show = true;
            }
            else
            {
                show = false;
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // gridColor = colorDialog1;
            ColorDialog colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                gridColor = colorDlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                cellColor = colorDlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void advancedSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unisettings uni = new unisettings();
            uni.Show(this);
            if(uni.accept == true)
            {
                universe = new bool[uni.sizeofuni(), uni.sizeofuni()];
                status = new bool[uni.sizeofuni(), uni.sizeofuni()];
                seed = uni.randseed();
                timer.Interval = uni.speedofuni();
            }
            graphicsPanel1.Invalidate();
        }
    }
}
