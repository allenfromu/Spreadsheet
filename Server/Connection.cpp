/* File:  Connection.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 */

#include "Connection.h"

using namespace std;

// Method to get the next "token" (command or info) by returning front of buffer string, up to next delimiter
std::string Connection::getNextToken(std::string * buff, std::string delimiter)
{ // Find the position of the next delimiter in the message (in a buffer string pointed to by buff)
  int pos = buff->find(delimiter);
  if(pos != std::string::npos) // if a matching delimeter was found...
    {
      std::string ret = buff->substr(0, pos); // store the front portion of the message to return
      (*buff) = buff->substr(pos+1); // reset the buffer string to the remainder of the message
      return ret;
    }
  else
    {
      return "";
    }
}

// Removes all instances of a certain character in a string
// We use it in the Collaborative Spreadsheet to remove spaces from formulas
void Connection::removeAllOfChar(std::string * buff, char rem)
{
  int pos = 0;
  while (buff->find(rem) != std::string::npos)
    {
      pos = buff->find(rem);
      (*buff) = buff->substr(0, pos) + buff->substr(pos+1);
    }
}

// Create a socket connection
Connection::Connection(void * sock)
{
  
  socknum = *(int *)sock;
  
}

// Listen for messages on the connection
void Connection::listen()
{
  int n;
  int e = 1;
  bool got_connected = false;
  char buffer[256];
  std::string command = "";
  bool got_command = false;
  
  std::string command_buffer = "";
  std::string spreadsheet_name;
  
  std::string err_string;
  
  //get the users and the spreadsheet manager
  SpreadsheetManager * sm = SpreadsheetManager::getSpreadsheetManager();
  Users * usrs = Users::getUsers();
  
  std::cout << "Socket Number is " << socknum << std::endl;
  while(!got_connected && e){
    while (command_buffer.find("\n") == std::string::npos)
      {
	bzero(buffer,256);
	n = read(socknum,buffer,255);
	if (n < 0 || buffer[0] == 0) {
	  cout<<"ERROR reading from socket"<<endl;
	  e = 0;
	  break;
	}
	command_buffer = command_buffer + buffer;
	removeAllOfChar(&command_buffer, '\r');
      }
    if(!e)
      {
	break;
      }
    
    std::cout << "Command Buffer is " << command_buffer << std::endl;
    
    
    
    command = getNextToken(&command_buffer, " ");
    got_command = command.compare("");
    
    if(!got_command)
      {
	//try to get to end of line
	command = getNextToken(&command_buffer, "\n");
	//connect is multi-word, and this means it is faulty
	if(command.compare("undo") == 0)
	  {
	    errorCallback(3, command);
	  }
	else
	  {
	    errorCallback(2, command);
	  }
	continue;
	//got_command = command.compare("");
      }
    
    std::cout << "Command is " << command << std::endl;
    
    if((!got_command) || !(command.compare("connect")==0)){
      
      err_string = command + " " + getNextToken(&command_buffer, "\n");
      if(command.compare("undo") == 0 || command.compare("register") == 0 || command.compare("cell") == 0)
	{
	  errorCallback(3, err_string);
	}
      else
	{
	  errorCallback(2, err_string);
	}     
      cout<<"Wrong initialization message"<<endl;     
    }
    else if(command.compare("connect")==0)
    {
      string login_name = getNextToken(&command_buffer, " ");
      bool got_name = login_name.compare("");
      
      if((!(got_name)) || !usrs->checkUser(login_name))
      {        
        errorCallback(4, "username empty or not registered.");
        getNextToken(&command_buffer, "\n");
        //e = 0;
      }
      else
	{ 
	  username = login_name;
	  spreadsheet_name = getNextToken(&command_buffer, "\n");
	  bool got_spreadsheet = command.compare("");
	  if(!(got_spreadsheet))
	    {
	      errorCallback(2, "spreadsheet name not included.");
	      //e = 0;
	    }
	  got_connected = true;
	}
    }
  }
  
  if(!e)
    {
      close(socknum);
      return;
    }
  std::cout << "Considering spreadsheet " << spreadsheet_name << endl;
  sheet = sm->getSpreadsheet(spreadsheet_name, this);
  
  
  
  while(e)
  {    
    while (command_buffer.find("\n") == std::string::npos)
      {
	bzero(buffer,256);
	n = read(socknum,buffer,255);
	if (n < 0 || buffer[0] == 0){
	  std::cout<<"ERROR reading from socket " << socknum << std::endl;
	  perror("Socket Error: ");
	  e = 0;
        
	  break;
	}
	command_buffer = command_buffer + buffer;
	removeAllOfChar(&command_buffer, '\r');
      }
    
    if (!e)
      {
	//something went wrong with the socket
	break;
      }
    
    string command = "";
    
    if(command_buffer.substr(0,5) == "undo\n")
      {
	//undo needs to be processed separately because it does not contain spaces
	command = getNextToken(&command_buffer, "\n");
      
	if(command.compare("undo") != 0)
	  {
	    std::cout << "Undo Command Weirdness...should not be possible to get here." << std::endl;
	  }
	std::cout << "undo requested by " << username << std::endl;
	sheet->requestUndo();
	continue;
      }
    
    command = getNextToken(&command_buffer, " ");
    got_command = command.compare("");
    if(!got_command)
      {
	//try to get to end of line
	command = getNextToken(&command_buffer, "\n");
	got_command = command.compare("");
      }
    std::cout << "Command at start is " << command << std::endl ;
    if(got_command && command.compare("register") == 0){
      std::string user = getNextToken(&command_buffer, "\n");
      bool got_user = user.compare("");
      
      if(!(got_user)|| usrs->checkUser(user))
	{
	  errorCallback(4, user);
	  cout<<"username empty or taken."<<endl;
	}
      else{
        //ofstream log("users.txt",ios_base::app|ios_base::out);
        usrs->addUser(user);
      }
    }
    else if (got_command && command.compare("cell") == 0) 
    {      
      std::cout << "processing a cell." << std::endl;
      std::string cellName = getNextToken(&command_buffer, " ");
      bool got_cell_name = cellName.compare("");
      
      std::string cellContents = getNextToken(&command_buffer, "\n");
           
      if(cellContents.substr(0,1).compare("=") == 0 )
	{
	  cellContents = toUpper(cellContents);
	  removeAllOfChar(&cellContents, ' ');
	}
      
      if(!(got_cell_name)) 
      {
        std::cout << "CellName Not Recognized: " << cellName << std::endl; 
        this->sendString("error 2 "+command+" "+cellName+" "+cellContents);
        continue;
      }
      /*
	if(!(got_cell_contents)) {
        this->sendString("Cell Contents Not Recognized");
        continue;
	}
      */
      //reserve space to check cell name content and circular error.
      
      std::cout << "CellName: " << cellName << ", CellContents: " << cellContents << std::endl;
      //otherwise, send on to spreadsheet
      sheet->updateCell(cellName, cellContents, this);
    }
    else if(got_command)
      {
	cout << "I am here to break, command is " << command <<endl;
      
	err_string = command + " " + getNextToken(&command_buffer, "\n");
      
	if(command.compare("connect") == 0)
	  {
	    errorCallback(3, err_string);
	  }
	else
	  {
	    errorCallback(2, err_string);
	  }
      
	//break;
	continue;
      }
  }
  sheet->removeUser(this);
  close(socknum);  
}

void Connection::connectedCallback(int num_cells)
{
  stringstream ss;
  ss << "connected "<<num_cells;
  this->sendString(ss.str());
}

void Connection::cellChangedCallback(std::string cell_name, std::string cell_value)
{
  std::cout << "Notifying " << username << std::endl;
  
  stringstream cellResponse;
  cellResponse << "cell " << cell_name << " " << cell_value;
  std::string response = cellResponse.str();
  
  this->sendString(response);
}

// Send a string over the socket connection
int Connection::sendString(std::string to_send)
{  
  std::cout << "sending <" << to_send << ">" << std::endl;
  to_send = to_send + "\n";
  
  char buff[to_send.size()];
  
  to_send.copy(buff, to_send.size(), 0);
  
  int n = write(socknum, &buff, to_send.size());
  return n;
  
}

void Connection::errorCallback(int errorType, std::string description)
{
  stringstream cellResponse;
  cellResponse << "error " << errorType << " " << description;
  std::string response = cellResponse.str();
  
  this->sendString(response);
}

// Change all letters in a string to upper-case 
std::string Connection::toUpper(std::string src)
{
  char buff[src.size()];
  
  src.copy(buff, src.size(), 0);
  
  int i;
  for(i = 0; i < src.size(); i++)
    {
      buff[i] = toupper(buff[i]);
    }
  buff[src.size()] = 0;
  std::string ret(buff);
  
  return ret;
  
}


void Connection::requestCloseSocket()
{
  //sheet->removeUser(this);
  close(socknum);
}


// Get the number of the socket
int Connection::getSockNum() const
{
  return socknum;
}

bool operator< (const Connection &left, const Connection &right)
{
  return left.getSockNum() < right.getSockNum();
}

