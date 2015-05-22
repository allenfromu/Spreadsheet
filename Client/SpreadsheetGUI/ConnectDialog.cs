/* Written by Jack Stafford, Daniel Kenner, Ella Ortega and Zepeng Zhao 
 * for the CS 3505 Spring 2015 Collaborative Spreadsheet Project
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS
{
    /// <summary>
    /// Creates a ConnectDialog Form (Message Box) to allow a user to connect to the server, 
    /// log in, and open a spreadsheet.  The user must enter their name, the server address,
    /// a port number, and the name of a spreadsheet to open.  If the spreadsheet exists in
    /// the server's virtual file system, it will be opened.  If it does not yet exist,
    /// a new, blank spreadsheet will be opened.
    /// </summary>
    public partial class ConnectDialog : Form
    {
        // The Connection information submitted by the user, and whether the connection succeeded         
        private Func<string, string, string, string, bool> submittedConnectionInfo;

        /// <summary>
        /// Opens a Form on which the user may submit a connection request.
        /// The information entered on the form is stored.
        /// </summary>
        /// <param name="submitted"></param>
        public ConnectDialog(Func<string, string, string, string, bool> submitted)
        {
            InitializeComponent();
            submittedConnectionInfo = submitted;
        }
        /// <summary>
        /// Closes the ConnectDialog form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Performed after the user clicks the Submit button and the
        /// information entered by the user in the form was submitted.
        /// If the connection attempt was unsuccessful, a "Connection Failed"
        /// message is shown in the box/form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitButton_Click(object sender, EventArgs e)
        {
            if (!submittedConnectionInfo(usernameBox.Text, serverBox.Text, spreadsheetBox.Text, portBox.Text))               
            {
                connectionFailed.Visible = true;
            }
        }

        private void serverBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
