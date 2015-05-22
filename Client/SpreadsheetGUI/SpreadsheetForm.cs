//Written by Jack Stafford for CS 3500, November 2014

// Modified for the CS 3505 Spring 2015 Collaborative Spreadsheet Project
// By Jack Stafford, Daniel Kenner, Ella Ortega, and Zepeng Zhao

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using Microsoft.VisualBasic;


namespace SS
{
    /// <summary>
    /// A spreadsheet which can be edited, saved, opened, and closed. Inputs can be numbers, test, and formulas.
    /// </summary>
    public partial class SpreadsheetForm : Form
    {
        /// <summary>
        /// The spreadsheet model.
        /// </summary>
        private Spreadsheet spreadsheet;

        /// <summary>
        /// The name of the spreadsheet.
        /// Default is "UntitledSpreadsheet"
        /// Changes if a file is opened or saved to something other than "UntitledSpreadsheet"
        /// </summary>
        private string spreadsheetName;

        /// <summary>
        /// Stores hover text relating to each value
        /// </summary>
        private Dictionary<string, string> cellValueHover;

        // removed for CS 3505
        /// <summary>
        /// Ensures someone doesn't attempt to close something unsaved, try to save, then cancel saving and lose everything.
        /// </summary>
        //private bool saved;

        private ConnectDialog connector;

        /// <summary>
        /// Constructs an empty spreadsheet
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SetSelection(1, 2); // select B3
            spreadsheet = new Spreadsheet(s => Regex.IsMatch(s, "^[a-zA-Z]{1}[1-9]{1}[0-9]?$"), s => s.ToUpper(), "cs3505"); // ensure nothing bigger than Z99 is accepted, sets z1 -> Z1
            spreadsheetName = "UntitledSpreadsheet";
            cellValueHover = new Dictionary<string, string>();
            //saved = false; // removed for CS 3505
            Text = spreadsheetName; // form title
            cellContents.Select(); // put cursor in typing area
            FormClosing += SpreadsheetForm_FormClosing;
            enableButtons(false);
        }

        private void SpreadsheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // close any open socket
            spreadsheet.Close();
        }

        /// <summary>
        /// Updates spreadsheet so display is current
        /// </summary>
        /// <param name="ss"></param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int col, row;
            ss.GetSelection(out col, out row); // get selected cell
            string cellName = getCellName(col, row); // save selected cell name
            cellSelectedDisplay.Text = cellName;
            setValueToolTip(cellName); // set appropriate value hover text

            // set value display
            string value;
            ss.GetValue(col, row, out value);
            cellValueDisplay.Text = value;

            setContentsTextBox(cellName);
        }

        /// <summary>
        /// Sets the contents of the contents textbox 
        /// </summary>
        /// <param name="cellName">name of cell for which to find contents</param>
        private void setContentsTextBox(string cellName)
        {
            object contents = spreadsheet.GetCellContents(cellName);
            if (contents is string)
                cellContents.Text = (string)contents;
            else if (contents is double)
                cellContents.Text = ((double)contents).ToString();
            else if (contents is Formula)
                cellContents.Text = "=" + ((Formula)contents).ToString();
            else
                cellContents.Text = "Cell contents must be letters, and digits.";
        }

        /// <summary>
        /// Handles keys being pressed while in contents textbox.
        /// Only acts upon 'Enter' => evaluates the contents and updates the value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cellContents_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return) // if enter
            {
                int col, row;
                spreadsheetPanel1.GetSelection(out col, out row); // get selected cell
                try
                {
                    // check format if it is a formula
                    if (Regex.IsMatch(cellContents.Text.Trim(), "^="))
                    {
                        Formula f = spreadsheet.MakeFormula(cellContents.Text);
                        spreadsheet.CheckForCircularDependency(getCellName(col, row), f);
                        spreadsheet.sendToServer("cell " + getCellName(col, row) + " " + "=" + f.ToString());
                    }

                    else
                        // send message to server 
                        // cell cell_name cell_contents
                        spreadsheet.sendToServer("cell " + getCellName(col, row) + " " + cellContents.Text);

                    // from CS 3500
                    //HashSet<string> toUpdate = spreadsheet.SetContentsOfCell(getCellName(col, row), cellContents.Text) as HashSet<string>; // update cell contents, save cells to update
                    //updatePanelValues(toUpdate); // update all cells which should change based upon change of selected cell
                }
                catch (FormulaFormatException f) // catches ineptly written formulas i.e. mismatched parens...
                {
                    MessageBox.Show(f.Message, "Error");
                }
                catch (CircularException)
                {
                    MessageBox.Show("A cell cannot be changed to depend on itself.", "Error");
                }
            }
        }

        /// <summary>
        /// Closes the current window when File->Close is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        /// Handles File->Open being clicked.

        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox("Enter spreadsheet name:", "Open dialog", "");
            if (input.Length > 0) // if user did not cancel
            {
                spreadsheetName = input;
                spreadsheet.connectToServer(spreadsheetName);
            }

        }

        /// <summary>
        /// Updates spreadsheetpanel cell values for each cell name in given IEnumerable.
        /// Names are assumed to be valid cells.
        /// </summary>
        /// <param name="cellNames">IEnumerable of cells to be updated</param>
        private void updatePanelValues(IEnumerable<string> cellNames)
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row); // get current selection
            foreach (string name in cellNames)
            {
                // translate from cell name to address e.g.
                int nameCol = name.ToCharArray()[0] - 'A';
                int nameRow;
                int.TryParse(name.Substring(1), out nameRow);
                nameRow -= 1;

                // set the new value in spreadsheetPanel and update text for value hover
                updateDisplayedValue(nameCol, nameRow, name);

                if (nameCol == col && nameRow == row) // if this cell is the selected cell, update the displayed value and contents above the grid
                {
                    string v;
                    spreadsheetPanel1.GetValue(nameCol, nameRow, out v);
                    MethodInvoker display = new MethodInvoker(() => { cellValueDisplay.Text = v; });

                    if (cellValueDisplay.InvokeRequired)
                    {
                        Invoke(display);
                    }
                    else
                    {
                        cellValueDisplay.Text = v;
                    }
                    setContentsTextBox(name);
                }
                setValueToolTip(name); // update value hover text
            }
        }

        /// <summary>
        /// Updates the saved value for the given cell.
        /// </summary>
        /// <param name="col">column of given cell</param>
        /// <param name="row">row of given cell</param>
        /// <param name="cellName">name of given cell</param>
        private void updateDisplayedValue(int col, int row, string cellName)
        {
            object value = spreadsheet.GetCellValue(cellName);
            if (value is string)
            {
                spreadsheetPanel1.SetValue(col, row, (string)value);
                if ((string)value == "") // hover will default to "Value is empty." if the name doesn't exist
                    cellValueHover.Remove(cellName);
                else
                    cellValueHover[cellName] = "Value is text.";
            }
            else if (value is double)
            {
                spreadsheetPanel1.SetValue(col, row, ((double)value).ToString());
                cellValueHover[cellName] = "Value is a number.";
            }
            else if (value is FormulaError)
            {
                spreadsheetPanel1.SetValue(col, row, "ERROR");
                cellValueHover[cellName] = ((FormulaError)value).Reason;
            }
        }

        /// <summary>
        /// Handles 'Help' clicks.
        /// Displays a message containing instructions for spreadsheet use.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sign In: \tLog in to the server and open the named spreadsheet in the" +
                            "\n\tcurrent window.  If the spreadsheet does not yet exist, a new," +
                            "\n\tblank spreadsheet is given this name.  Ctrl+S shortcut." +
                            "\nOpen: \tOpen a saved spreadsheet in current window. Ctrl+O shortcut." +
                            "\nClose: \tClose current window. Ctrl+W shortcut." +
                            "\n\nAverage: \tComputes the average of whichever column contains the currently" +
                            "\n\tselected cell. The column must contain only numbers. Ctrl+G shortcut" +
                            "\nMedian: \tComputes the median of whichever column contains the currently" +
                            "\n\tselected cell. The column must contain only numbers. Ctrl+M shortcut" +
                            "\n\nSelected: \tShows selected cell name. Change selection by clicking a rectangle on" +
                                "\n\tthe spreadsheet." +
                            "\nValue: \tShows value of the selected cell's contents. Hover over value for info." +
                            "\nContents: \tShows and allows editing of the selected cell's contents. Press 'Enter' to" +
                                "\n\tsubmit edited contents. Cells can hold numbers, text, or formulas. " +
                                "\n\tEnter a formula by preceeding numbers, cell names, and operators with" +
                                "\n\tan equals sign.\n" +
                                "\tE.g. =A1+2  or  =2e9/d23", "Help");
        }

        /// <summary>
        /// Returns cell name
        /// e.g. col = 0, row = 0, returns A1
        /// </summary>
        /// <param name="col">column</param>
        /// <param name="row">row</param>
        /// <returns></returns>
        private string getCellName(int col, int row)
        {
            return (char)(col + 'A') + "" + (row + 1);
        }

        /// <summary>
        /// Sets the value tooltip (hover text) for the given cell
        /// </summary>
        /// <param name="cellName">name of cell for which to set the tooltip</param>
        private void setValueToolTip(string cellName)
        {
            string hover;
            if (cellValueHover.TryGetValue(cellName, out hover)) // if the value exists, the appropriate hover is set on the value label and display box
            {
                toolTip1.SetToolTip(cellValueDisplay, hover);
                toolTip1.SetToolTip(valueLabel, hover);
            }
            else // if the cell is empty, set the value hover accordingly
            {
                toolTip1.SetToolTip(cellValueDisplay, "Value is empty.");
                toolTip1.SetToolTip(valueLabel, "Value is empty.");
            }
        }


        /// <summary>
        /// Computes the average of whichever column contains the selected cell.
        /// Will only compute if the column contains only numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void averageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get selected column
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            string cellName = getCellName(col, row);
            string cellColumn = cellName.Substring(0, 1); // column is the first letter of the name

            // compute average
            List<string> nonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells() as List<string>;
            double total = 0;
            int numberOfCellsInColumn = 0;
            foreach (string name in nonEmptyCells)
            {
                if (name.StartsWith(cellColumn))
                {
                    object value = spreadsheet.GetCellValue(name);
                    if (value is double)
                    {
                        total += (double)value;
                        numberOfCellsInColumn++;
                    }
                    else
                    {
                        MessageBox.Show("Average can only be computed on a column containing only numbers. \n" +
                            name + " does not contain a number.", "Error");
                        return;
                    }
                }
            }
            double average = 0;
            if (numberOfCellsInColumn != 0)
                average = total / numberOfCellsInColumn;

            MessageBox.Show("The average of column " + cellColumn + " is: " + average, "Average");
        }

        /// <summary>
        /// Computes the median of whichever column contains the selected cell.
        /// Will only compute if the column contains only numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get selected column
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            string cellName = getCellName(col, row);
            string cellColumn = cellName.Substring(0, 1); // column is the first letter of the name

            // compute median
            List<string> nonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells() as List<string>;
            int numberOfCellsInColumn = 0;
            List<double> values = new List<double>();
            foreach (string name in nonEmptyCells)
            {
                if (name.StartsWith(cellColumn))
                {
                    object value = spreadsheet.GetCellValue(name);
                    if (value is double)
                    {
                        values.Add((double)value);
                        numberOfCellsInColumn++;
                    }
                    else
                    {
                        MessageBox.Show("Median can only be computed on a column containing only numbers. \n" +
                            name + " does not contain a number.", "Error");
                        return;
                    }
                }
            }
            double median = 0;
            values.Sort();
            if (numberOfCellsInColumn != 0)
            {
                if (values.Count % 2 == 0) // if even, median is average of middle two numbers
                {
                    double first = values[values.Count / 2];
                    double second = values[values.Count / 2 - 1];
                    median = (first + second) / 2;
                }
                else                      // if odd, median is middle number
                    median = values[values.Count / 2];
            }

            MessageBox.Show("The median of column " + cellColumn + " is: " + median, "Median");
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get info from user
            connector = new ConnectDialog(submittedConnectionInfo);
            connector.Show();
        }

        public bool submittedConnectionInfo(string username, string server, string ssName, string port)
        {
            spreadsheetName = ssName;
            return spreadsheet.connectToServer(username, server, ssName, port, fromModel, invokeUpdatePanel);
        }

        private void invokeUpdatePanel(IEnumerable<string> cellNames)
        {
            Invoke(new MethodInvoker(() => { updatePanelValues(cellNames); }));
        }

        private void fromModel(int option)
        {
            if (option == 1)
            {
                MethodInvoker close = new MethodInvoker(() => { connector.Close(); });

                if (connector.InvokeRequired)
                {
                    Invoke(close);
                }
                else
                {
                    connector.Close();
                }
            }
            else if (option == 3)
                MessageBox.Show("Permanent failure: server does not have sysadmin registered.");
            if (option < 3)
                Invoke(new MethodInvoker(() =>
                {
                    enableButtons(true);
                    Text = spreadsheetName;
                    // clear any residual info
                    spreadsheet.Open();
                    spreadsheetPanel1.Clear();
                    cellValueDisplay.Text = "";
                    cellContents.Text = "";
                }));
            else if (option == 4)
            {
                MessageBox.Show("Server shut down unexpectedly. This spreadsheet will now close.");
                Invoke(new MethodInvoker(() => { Close(); }));
            }
        }

        private void enableButtons(bool enabled)
        {
            openToolStripMenuItem.Enabled = enabled;
            undoMenuItem.Enabled = enabled;
            averageToolStripMenuItem.Enabled = enabled;
            medianToolStripMenuItem.Enabled = enabled;
            cellContents.Enabled = enabled;
        }

        private void undoClick(object sender, EventArgs e)
        {
            //MessageBox.Show("Undo");
            spreadsheet.sendToServer("undo");
        }

        private void mathToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
