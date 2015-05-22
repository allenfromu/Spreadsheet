/* File:  server_main.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Ella Ortega and Jack Stafford
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * This will set up a server for a collaborative spreadsheet, using a port 
 * number sent from command line.  To start the server, must prepare with 
 * "Make server" and then the command to start it is "./server <port number>"
 * This server conforms to the Official Protocol by team Axis of Ignorance.  
 */

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
#include <set>
#include <signal.h>
#include "SpreadsheetManager.h"
#include "SpreadsheetCallbackInterface.h"
#include "Connection.h"
#include "Users.h"

using namespace std;


void * client(void *c);
void * listen(void * pn);
void SIGPIPE_Handler(int s);

static set<Connection> connections;

int main(int argc, char *argv[])
{
  //port number should be passed from argv
  if(argc > 2 || argc < 1){
    std::cout << "Spreadsheet Server (Version 0.9) Usage:" << std::endl;
    std::cout << "./server <optional port number (Default 2000)>" << std::endl;
    return 1;
  }
  
  int portno;
  
  if(argc == 1)
  {
    //protocol compliant default port
    portno = 2000;
  }
  else
  {
    //shown above logically to be here...
    //convert port number to int
    portno = atoi(argv[1]);
  }
  
  //necessary setup
  Users * usrs = Users::getUsers();
  usrs->load();
  
  SpreadsheetManager * sm = SpreadsheetManager::getSpreadsheetManager();
  
  int i = 0;
  
  pthread_t threads[5];
  signal(SIGPIPE, SIGPIPE_Handler);
  
  int rc = pthread_create(&threads[i],NULL,listen,(void *)&portno);
  if(rc){
    cout<<"thread fail"<<endl;
    return 0;
  }
  
  //listen(portno);
  std::string request = "";
  while (true)
  {
    getline(cin,request);
    if(request.compare("quit") == 0)
    {
      std::cout << "Requested Quit" << std::endl;
      //std::cout << "==================================" << std::endl;
      //std::cout << "           Please Note:           " << std::endl;
      //std::cout << "  The following messages may be   " << std::endl;
      //std::cout << "  unreadable because cout is not  " << std::endl;
      //std::cout << "  thread-safe. The server is      " << std::endl;
      //std::cout << "  working correctly.              " << std::endl;
      //std::cout << "==================================" << std::endl;
      break;
    }
    else if(request.compare("save") == 0)
    {
      std::cout << "Requested Save" << std::endl;
      //implement saving code
      usrs->save();
      sm->saveAll();
      std::cout << "Saved" << std::endl;
    }
    else if (request.compare("kickall") == 0)
    {
      std::cout << "Requested Kickall" << std::endl;
      
      std::vector<Connection> connsToClose;
  
      //this is a truly awful way to take care of this but it works.
  
      for (std::set<Connection>::iterator conn_it = connections.begin(); conn_it != connections.end(); conn_it++)
      {
        connsToClose.push_back(*conn_it);
      }
      while (connsToClose.size() > 0)
      {
        connsToClose.back().requestCloseSocket();
        connsToClose.pop_back();
        //reduce corruption to output log
        usleep(1000);
      }
      
      std::cout << "Kicked All" << std::endl;
      
    }
  }
  
  //save everything  
  //close all active connections
  
  std::vector<Connection> connsToClose;
  
  //this is a truly awful way to take care of this but it works.
  
  for (std::set<Connection>::iterator conn_it = connections.begin(); conn_it != connections.end(); conn_it++)
  {
    connsToClose.push_back(*conn_it);
  }
  while (connsToClose.size() > 0)
  {
    connsToClose.back().requestCloseSocket();
    connsToClose.pop_back();
    //reduce corruption to output log
    usleep(1000);
  }
  
  usrs->save();
  sm->saveAll();
  
  std::cout << "Program Now Quitting." << std::endl;
  
  return 0;
}

void SIGPIPE_Handler(int s)
{
  std::cout << "Caught SIGPIPE" << std::endl;
}


void * listen(void * pn)
{
  int portno = *(int *)pn;
  //create socket
  int sockfd = socket(AF_INET, SOCK_STREAM, 0);
  
  if(sockfd < 0)
  {
    cout << "socket server fail - most likely due to a used port"<<endl;
    exit(1);
  }
  struct sockaddr_in serv_addr, cli_addr;
  socklen_t clilen;
  
  //set struct buffer to all 0
  bzero((char *) &serv_addr, sizeof(serv_addr));
  
  //configure sockaddr_in struct
  serv_addr.sin_family = AF_INET;
  serv_addr.sin_addr.s_addr = INADDR_ANY;
  serv_addr.sin_port = htons(portno);
  
  if (bind(sockfd, (struct sockaddr *) &serv_addr,sizeof(serv_addr)) < 0)
  {
    cout<<"ERROR on binding"<<endl;
    exit(1);
  }
  //set number of requests in the queue
  listen(sockfd,5);
  
  clilen = sizeof(cli_addr);
  pthread_t threads[5];
  int i = 0;
  while(1)
  {
    cout<<"Waiting for connection"<<endl;
    int newsockfd = accept(sockfd,(struct sockaddr *) &cli_addr,&clilen);
   
    if (newsockfd < 0)
    {   
      cout<<"ERROR on accept"<<endl;
      break;
    }
    int rc = pthread_create(&threads[i],NULL,client,(void *)&newsockfd);
    if(rc){
      cout<<"thread fail"<<endl;
      break;
    }
  }
  
  close(sockfd);
  return NULL; 
}


void *client(void *clisock)
{
  int newsockfd = *(int *)clisock;
  
  //make a new connection
  Connection c(clisock);
  //add the connection
  connections.insert(c);
  //listen on connection
  c.listen();
  //remove the connection
  connections.erase(c);
  return NULL;  
}
