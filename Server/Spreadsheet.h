/* File:  Spreadsheet.h
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * Creates a new spreadsheet or opens one from a file.
 */

#ifndef SPREADSHEET_H
#define SPREADSHEET_H

#include "Cell.h"
#include <string>
#include <map>
#include <vector> //for undo
#include <iostream>
#include <set>
#include <pthread.h>
#include <fstream>
#include "SpreadsheetCallbackInterface.h"
#include "DependencyGraph.h"


//Note on design: this is a singleton class.

class Spreadsheet
{
public:
  //these are so that only special classes may create spreadsheet objects
  //mainly because spreadsheets are pseudo-singleton objects, and should be
  //managed by a singleton. (like spreadsheetmanager)
  friend class SpreadsheetTester;
  friend class SpreadsheetManager;  

  void requestSave();
  void updateCell(std::string cell_name, std::string cell_contents, SpreadsheetCallbackInterface * caller);
  void registerUser(SpreadsheetCallbackInterface * user); //callback for update broadcast message
  void removeUser(SpreadsheetCallbackInterface *user);
  void requestUndo();
  std::string toString(); //for testing
  
  Spreadsheet();  //new, empty spreadsheet
  Spreadsheet(std::string filename); //load spreadsheet
  ~Spreadsheet(); //save and quit spreadsheet
  int getSize();
  
private:
  std::string toUpper(std::string src);
  void Save();
  bool Load(std::string filename);
  void notifyUsers(std::string cell, std::string value);
  bool checkForCircularDependency(std::string cell_name);
  bool Visit(std::string start, std::string name, std::set<std::string> * visted); //Visits cells, checking for circular dependency
  
  pthread_mutex_t lock1;
  pthread_mutex_t lock2;


  std::map<std::string, Cell> cells;
  std::string name;
  std::set<SpreadsheetCallbackInterface *> users;
  std::vector<std::pair<std::string, std::string> > undoStack;
  DependencyGraph dependencies;
};

#endif
