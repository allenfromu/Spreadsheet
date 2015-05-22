
﻿// Modified for the CS 3505, Spring 2015, Collaborative Spreadsheet Project
// By Jack Stafford, Daniel Kenner, Ella Ortega, and Zepeng Zhao

// Written by Jack Stafford for CS 3500, October 2014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;


namespace SS
{
    /// <summary>
    /// A spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    ///  
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Maps string cell names to the actual Cell
        /// </summary>
        private Dictionary<string, Cell> cells;
        /// <summary>
        /// Keeps track of dependency relationships.
        /// For use on cells containing formulas.
        /// </summary>
        private DependencyGraph dependencies;

        private Socket clientSocket;
        private TcpClient client;
        private byte[] receiveBuffer;
        private Action<int> toView;
        private string messageBuffer;
        private byte[] connectedBuffer;
        private string connectedMessage;
        private string username, server, ssName;
        private int portNum;
        private Action<IEnumerable<string>> updateCells;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get;
            protected set;
        }

        /// <summary>
        /// Constructs an empty spreadsheet with "default" version.
        /// </summary>
        public Spreadsheet()
            : this(s => true, s => s, "default")
        {
            //Your zero-argument constructor should create an empty spreadsheet 
            //  that imposes no extra validity conditions, 
            //    normalizes every cell name to itself, and has version "default".
        }


        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            Changed = false;
            clientSocket = null;
        }

        public void Open()
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (ReferenceEquals(content, null))
                throw new ArgumentNullException();
            if (ReferenceEquals(name, null))
                throw new InvalidNameException();
            string cellName = Normalize(name);
            if (!(Regex.IsMatch(cellName, "^[a-zA-Z]+[1-9]+[0-9]*$")) || !IsValid(cellName))
                throw new InvalidNameException();

            double d;
            HashSet<string> toReturn = new HashSet<string>();
            if (double.TryParse(content, out d))
                toReturn = SetCellContents(cellName, d) as HashSet<string>;
            else if (Regex.IsMatch(content.Trim(), "^=")) // first character besides a space is '=' => it's a formula
            {
                // updated for CS 3505
                //Func<string, bool> formulaValidator = s => (Regex.IsMatch(s, "^[a-zA-Z]+[1-9]+[0-9]*$") && IsValid(s));//nameValidator(s)); // check variable syntax and existence
                toReturn = SetCellContents(cellName, MakeFormula(content)/*new Formula(content.Trim().Substring(1), Normalize, formulaValidator)*/) as HashSet<string>;// nameNormalizer, formulaValidator)) as HashSet<string>;
            }
            else
                toReturn = SetCellContents(cellName, content) as HashSet<string>;

            Changed = true;
            EvaluateCells(toReturn);
            return toReturn;
        }

        public Formula MakeFormula(string formula)
        {
            Func<string, bool> formulaValidator = s => (Regex.IsMatch(s, "^[a-zA-Z]+[1-9]+[0-9]*$") && IsValid(s));
            String temp = formula.Trim().Substring(1);
        
            Formula f = new Formula(formula.Trim().Substring(1), Normalize, formulaValidator);
            return f;
        }

        /// <summary>
        /// Update cell values to match cell contents
        /// </summary>
        /// <param name="toEvaluate">Hashset of cell names which are to be evaluated</param>
        private void EvaluateCells(HashSet<string> toEvaluate)
        {
            foreach (string sName in toEvaluate)
            {
                if (cells.ContainsKey(sName))
                {
                    Cell cell = cells[sName];
                    if (cell.Contents is string || cell.Contents is double)
                        cell.Value = cell.Contents;
                    else
                    {
                        try
                        { cell.Value = ((Formula)cell.Contents).Evaluate(s => (double)cells[s].Value); }
                        catch (InvalidCastException)
                        { cell.Value = new FormulaError("Must depend only upon numbers"); }
                        catch (KeyNotFoundException)
                        { cell.Value = new FormulaError("This cell depends on an empty cell"); }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// The contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell());

            Cell cell = cells[name];

            if (cell.Contents is Formula)
                //remove dependencies which will be obliterated by changing this cell's contents
                foreach (string dependee in dependencies.GetDependees(name))
                    dependencies.RemoveDependency(dependee, name);

            cell.Contents = number;

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (ReferenceEquals(text, null))
                throw new ArgumentNullException();

            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell());

            Cell cell = cells[name];

            if (cell.Contents is Formula)
                //remove dependencies which will be obliterated by changing this cell's contents
                foreach (string dependee in dependencies.GetDependees(name))
                    dependencies.RemoveDependency(dependee, name);

            if (text == "")
                cells.Remove(name);
            else
                cell.Contents = text;

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (ReferenceEquals(formula, null))
                throw new ArgumentNullException();

            CheckForCircularDependency(name, formula);

            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell());
            cells[name].Contents = formula;
            dependencies.ReplaceDependees(name, formula.GetVariables());

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return new List<string>(cells.Keys);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (ReferenceEquals(name, null))
                throw new InvalidNameException();
            string cellName = Normalize(name);
            if (!(Regex.IsMatch(cellName, "^[a-zA-Z]+[1-9]+[0-9]*$")) || !IsValid(cellName))
                throw new InvalidNameException();

            if (!cells.ContainsKey(cellName))
                return "";
            return cells[cellName].Contents;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (ReferenceEquals(name, null))
                throw new InvalidNameException();
            string cellName = Normalize(name);
            if (!(Regex.IsMatch(cellName, "^[a-zA-Z]+[1-9]+[0-9]*$")) || !IsValid(cellName))
                throw new InvalidNameException();

            if (!cells.ContainsKey(cellName))
                return "";
            return cells[cellName].Value;

        }

        /// <summary>
        /// Checks to ensure a circular dependency will not be added by setting the contents
        /// of the given name to the given formula.
        /// </summary>
        /// <param name="name">string name of cell whose contents will be set</param>
        /// <param name="formula">Formula to be checked</param>
        public void CheckForCircularDependency(string name, Formula formula)
        {
            HashSet<string> dependees = new HashSet<string>();
            foreach (string cellInFormula in formula.GetVariables())
            {
                dependees.Add(cellInFormula);
                dependees.UnionWith(GetAllDependees(cellInFormula));
            }
            foreach (string existingDependee in dependees)
                if (name == existingDependee)
                    throw new CircularException();
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (ReferenceEquals(name, null))
                throw new ArgumentNullException();
            string cellName = Normalize(name);
            if (!(Regex.IsMatch(cellName, "^[a-zA-Z]+[1-9]+[0-9]*$")) || !IsValid(cellName))
                throw new InvalidNameException();
            return dependencies.GetDependents(cellName);
        }

        /// <summary>
        /// Aids in checking for circular dependencies. If name has no dependees, returns an empty set. 
        /// Otherwise, returns a set containing all of name's dependees
        /// </summary>
        /// <param name="name">string name of a cell for which dependees will be found</param>
        /// <returns>set of all dependees, direct and indirect</returns>
        private ISet<string> GetAllDependees(string name)
        {
            if (!dependencies.HasDependees(name))
                return new HashSet<string>();
            HashSet<string> names = new HashSet<string>();
            foreach (string dependee in dependencies.GetDependees(name))
            {
                names.Add(dependee);
                names.UnionWith(GetAllDependees(dependee));
            }
            return names;
        }


        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /* This method and the Save functionality are removed from the Spreadsheet for the 
         * CS 3505 Collaborative Spreadsheet Project.  All saving is to be done at the server level,
         * in order to ensure synchronicity between multiple users of the same spreadsheet.
         public override void Save(string filename)
         {
             try
             {
                 using (XmlWriter writer = XmlWriter.Create(filename))
                 {
                     writer.WriteStartDocument();
                     writer.WriteStartElement("spreadsheet"); // start spreadsheet
                     writer.WriteAttributeString("version", Version);//version);

                     foreach (string cellName in cells.Keys)
                     {
                         writer.WriteStartElement("cell"); // start cell
                         writer.WriteElementString("name", cellName); // start and end name

                         // start and end contents
                         object cellContents = cells[cellName].Contents;
                         if (cellContents is string)
                             writer.WriteElementString("contents", cellContents as string);
                         else if (cellContents is double)
                             writer.WriteElementString("contents", ((double)cellContents).ToString());
                         else if (cellContents is Formula)
                             writer.WriteElementString("contents", "=" + ((Formula)cellContents).ToString());
                         else
                             throw new SpreadsheetReadWriteException(cellName + "'s contents must be a string, double, or Formula");

                         writer.WriteEndElement(); //end cell
                     }
                     writer.WriteEndElement(); // end spreadsheet
                     writer.WriteEndDocument();
                     Changed = false;
                 }
             }
             catch (ArgumentNullException)
             {
                 throw new SpreadsheetReadWriteException("Filename cannot be null");
             }
             catch (Exception e)
             {
                 if (e is SpreadsheetReadWriteException)
                     throw e;
                 throw new SpreadsheetReadWriteException("Error during save.");
             }
         } End Save()  */

        /// <summary>
        /// This connectToServer() method was added to the Spreadsheet for the CS 3505 Collaborative
        /// Spreadsheet project.  When a spreadsheet client requests a connection to the server and
        /// a spreadsheet, a connection is attempted.  Each connection is with only one spreadsheet.
        /// To open another spreadsheet, another connection must be established.
        /// </summary>
        /// <param name="username">Name to be used to login to server</param>
        /// <param name="server">Server address</param>
        /// <param name="ssName">Name of the Spreadsheet to open</param>
        /// <param name="port">Port through which to connect to server</param>
        /// <param name="sendMessageToView">Boolean: True if connection established and signin successful</param>
        /// <returns>True if signin successful & connection established</returns>
        public bool connectToServer(string username, string server, string ssName, string port, Action<int> sendMessageToView, Action<IEnumerable<string>> updateCells)
        {
            if (clientSocket != null)
            {
                clientSocket.Close();

            }
            this.username = username;
            this.server = server;
            this.ssName = ssName;
            toView = sendMessageToView;
            this.updateCells = updateCells;
            IPAddress ip;
            // check for valid IP address or hostname and port number and names
            if ((IPAddress.TryParse(server, out ip) || Uri.CheckHostName(server) != UriHostNameType.Unknown)
                && Int32.TryParse(port, out portNum)
                && ssName.Length > 0 && username.Length > 0)
            {
                try { client = new TcpClient(server, portNum); }
                catch (SocketException) { return false; }
                clientSocket = client.Client;

                sendToServer("connect " + username + " " + ssName);
                connectedBuffer = new byte[1024];
                connectedMessage = "";
                clientSocket.BeginReceive(connectedBuffer, 0, connectedBuffer.Length, SocketFlags.None, connected, null);
                return true;
            }
            return false;
        }

        public bool connectToServer(string ssName)
        {
            this.ssName = ssName;
            if (clientSocket != null)
            {
                clientSocket.Close();

            }
            try { client = new TcpClient(server, portNum); }
            catch (SocketException) { return false; }
            clientSocket = client.Client;

            sendToServer("connect " + username + " " + ssName);
            connectedBuffer = new byte[1024];
            connectedMessage = "";
            clientSocket.BeginReceive(connectedBuffer, 0, connectedBuffer.Length, SocketFlags.None, connected, null);
            return true;
        }

        /// <summary>
        /// Added for the CS 3505 Collaborative Spreadsheet Project. Callback when a message
        /// is received from the initial connection attempt.  If it was successful, begin to receive messages.
        /// If not, connect as "sysadmin" and register the user so that they can edit spreadsheets.
        /// Each user must be registered by a previously validated user before they can log in and connect
        /// to a spreadsheet.  The "sysadmin" is guaranteed to be a valid user.
        /// </summary>
        /// <param name="ar">IAsyncResult</param>
        private void connected(IAsyncResult ar)
        {
            int read = clientSocket.EndReceive(ar);
            if (read > 0)
            {
                connectedMessage += Encoding.ASCII.GetString(connectedBuffer, 0, read);
                int messageEnd = connectedMessage.IndexOf('\n');
                if (messageEnd != -1)
                {
                    string message = connectedMessage.Substring(0, messageEnd);
                    if (message.Split(' ')[0] == "connected")
                    {
                        // Good to go; begin receiving messages from server
                        toView(1);
                        receiveBuffer = new byte[1024];
                        messageBuffer = "";
                        clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, receive, null);
                    }
                    else
                    {
                        // Close the previous socket and open a new one
                        clientSocket.Close();
                        try { client = new TcpClient(server, portNum); }
                        catch (SocketException) { }
               // Do we need to test connection here, making sure that it worked before using it?
                        clientSocket = client.Client;

                        // Connect as sysadmin in order to register the user. Each request must include a spreadsheet name to open.
                        sendToServer("connect sysadmin register_user.sprd"); // "Dummy" spreadsheet name for registration purposes.
                        connectedBuffer = new byte[1024];
                        connectedMessage = "";
                        clientSocket.BeginReceive(connectedBuffer, 0, connectedBuffer.Length, SocketFlags.None, registerUser, null);
                    }
                }
            }
            else
                Close(); // Connection failed
        }

        /// <summary>
        /// Added for the CS 3505 Collaborative Spreadsheet Project.
        /// Callback after sysadmin logs in. Attempt to register the user and then
        /// open a new socket connection and spreadsheet for the user
        /// </summary>
        /// <param name="ar">IAsyncResult</param>
        private void registerUser(IAsyncResult ar)
        {
            int read = clientSocket.EndReceive(ar);
            if (read > 0)
            {
                connectedMessage += Encoding.ASCII.GetString(connectedBuffer, 0, read);
                int messageEnd = connectedMessage.IndexOf('\n');
                if (messageEnd != -1)
                {
                    string message = connectedMessage.Substring(0, messageEnd);
                    if (message.Split(' ')[0] == "connected")
                    {
                        // If sysadmin successfully connected to the server, register the user
                        sendToServer("register " + username);
                        clientSocket.Close();

                        // Open a new connection for the user and request that server open spreadsheet
                        try { client = new TcpClient(server, portNum); }
                        catch (SocketException) { }
                        clientSocket = client.Client;
                        sendToServer("connect " + username + " " + ssName);

                        connectedBuffer = new byte[1024];
                        connectedMessage = "";
                        clientSocket.BeginReceive(connectedBuffer, 0, connectedBuffer.Length, SocketFlags.None, userRegistered, null);
                    }
                    else
                    {
                        toView(3); // Attempt to connect permanently failed.
                    }
                }
            }
        }

        /// <summary>
        /// Added for the CS 3505 Collaborative Spreadsheet Project.
        /// Callback for the attempt to connect the user and open a spreadsheet.
        /// </summary>
        /// <param name="ar">IAsyncResult</param>
        private void userRegistered(IAsyncResult ar)
        {
            int read = clientSocket.EndReceive(ar);
            if (read > 0)
            {
                connectedMessage += Encoding.ASCII.GetString(connectedBuffer, 0, read);
                int messageEnd = connectedMessage.IndexOf('\n');
                if (messageEnd != -1)
                {
                    string message = connectedMessage.Substring(0, messageEnd);
                    if (message.Split(' ')[0] == "connected")
                    {
                        // User is now connected to the server and a spreadsheet, can receive messages
                        toView(1);
                        receiveBuffer = new byte[1024];
                        messageBuffer = "";
                        clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, receive, null);
                    }
                    else
                    {
                        toView(3); // Connection attempt permanently failed
                    }
                }
            }
        }

        /// <summary>
        /// Added for the CS 3505 Collaborative Spreadsheet Project.
        /// Callback when messages are received from the server, after a valid
        /// connection is established and a spreadsheet is opened.
        /// </summary>
        /// <param name="ar"></param>
        private void receive(IAsyncResult ar)
        {
            try
            {
                int read = clientSocket.EndReceive(ar);
                if (read > 0)
                {
                    messageBuffer += Encoding.ASCII.GetString(receiveBuffer, 0, read);
                    receiveBuffer = new byte[1024];
                    int messageEnd = messageBuffer.IndexOf('\n');
                    while (messageEnd != -1) // Message is complete ('\n' received)
                    {
                        commands(messageBuffer.Substring(0, messageEnd)); // Callback to parse the command message
                        messageBuffer = messageBuffer.Substring(messageEnd + 1);
                        messageEnd = messageBuffer.IndexOf('\n');
                    }
                    // Message is incomplete (no '\n') so ask for more info
                    clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, receive, null);
                }
                else
                {
                    if (read == 0) // Server has closed
                    {
                        //deal with server closing
                        // Sorry, I know it's simple but my brain is shutting down and I can't
                        // think of how to send a call for a new message box or whatever
                        // TODO tomorrow
                        toView(4);
                    }
                    Close();
                }
            }
            catch (ObjectDisposedException e) { return; }
        }

        /// <summary>
        /// Added for the CS 3505 Collaborative Spreadsheet Project.
        /// Parse commands received from the server and take appropriate actions
        /// </summary>
        /// <param name="message">command message received from the server</param>
        private void commands(string message)
        {
            System.Diagnostics.Debug.WriteLine("*************Received: " + message);
            int first = message.IndexOf(' ');
            string command = message.Substring(0, first);
            message = message.Substring(first + 1);
            first = message.IndexOf(' ');
            string cell = message.Substring(0, first);
            string contents = message.Substring(first + 1);
            switch (command)
            {
                case "cell":
                    HashSet<string> toUpdate = SetContentsOfCell(cell, contents) as HashSet<string>; // update cell contents, save cells to update
                    updateCells(toUpdate);
                    break;
                case "error": // No action required; just ignore.
                    break;
                default:
                    break;
            }
        }

        public void Close()
        {
            if (clientSocket != null)
                clientSocket.Close();
        }

        /// <summary>
        /// Added for the CS 3505 Collaborative Spreadsheet Project.
        /// Automatically adds message ending ('\n') and sends message to server
        /// using the connection already set up
        /// </summary>
        /// <param name="message">command message to send to server</param>
        public void sendToServer(string message)
        {
            byte[] b = Encoding.ASCII.GetBytes(message + "\n");
            clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, null, null);
        }

        /// <summary>
        /// A cell has contents and a value.
        /// Contents are a string, a double, or a Formula.
        /// The value is a string, a double, or a FormulaError
        /// Unless specifically set to something else, a cell contains an empty string.
        /// </summary>
        internal class Cell
        {
            /// <summary>
            /// Contents should be a string, double, or a Formula
            /// </summary>
            internal object Contents
            { get; set; }

            /// <summary>
            /// Value should be a string, double, or a FormulaError
            /// </summary>
            internal object Value
            { get; set; }

            /// <summary>
            /// Creates an empty Cell 
            /// i.e. contains ""
            /// </summary>
            internal Cell()
            {
                Contents = "";
                Value = "";
            }
        }
    }
}

