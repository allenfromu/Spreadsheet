/* File:  Connection.h
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 */

#ifndef CONNECTION_H
#define CONNECTION_H

#include <stdio.h>
#include <iostream>
#include <stdlib.h>
#include <cstdlib>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <pthread.h>
#include <map>
#include <set>
#include <sstream>
#include <fstream>

#include "SpreadsheetCallbackInterface.h"
#include "SpreadsheetManager.h"

#include "Users.h"
#include <string>
#include <iostream>
#include "SpreadsheetCallbackInterface.h"



class Connection : public SpreadsheetCallbackInterface
{
public:
  Connection(void * sock);

  void cellChangedCallback(std::string cell_name, std::string cell_value);
  void errorCallback(int errorType, std::string description);
  void connectedCallback(int num_cells);
  void listen();
  
  int getSockNum() const;
  
  void requestCloseSocket();
  
private:
  int socknum;
  
  std::string username;
  
  std::string command_buffer;
  
  Spreadsheet * sheet;
  
  int sendString(std::string to_send);
  
  static std::string toUpper(std::string src);
  void removeAllOfChar(std::string * buff, char rem);
  
  std::string getNextToken(std::string * buff, std::string delimiter);
  

};

bool operator< (const Connection &left, const Connection &right);

#endif
