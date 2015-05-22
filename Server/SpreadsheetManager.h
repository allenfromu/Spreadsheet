/* File:  SpreadsheetManager.h
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * This class is designed to create a singleton object that manages all of
 * the spreadsheets used by the Collaborative Spreadsheet Project.
 */

#ifndef SPREADSHEETMANAGER_H
#define SPREADSHEETMANAGER_H
#include <map>
#include <string>
#include "SpreadsheetCallbackInterface.h"
#include "Spreadsheet.h"

class SpreadsheetManager
{
public:
  static SpreadsheetManager * getSpreadsheetManager();
  
  Spreadsheet * getSpreadsheet(std::string filename, SpreadsheetCallbackInterface * user);
  
  void saveAll();
  
private:
  
  static SpreadsheetManager * sm;
  
  SpreadsheetManager() {};
  
  SpreadsheetManager(SpreadsheetManager const&) {};//don't implement
  void operator=(SpreadsheetManager const&) {};//don't implement
  ~SpreadsheetManager() {};//want to be private...we'll see what happens.
  
  std::map<std::string, Spreadsheet *> sheets;
  
};

#endif
