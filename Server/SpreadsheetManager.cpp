/* File:  SpreadsheetManager.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * This class is designed to create a singleton object that manages all of
 * the spreadsheets used by the Collaborative Spreadsheet Project.
 */

#include "SpreadsheetManager.h"

SpreadsheetManager *SpreadsheetManager::sm = NULL;

Spreadsheet * SpreadsheetManager::getSpreadsheet(std::string filename, SpreadsheetCallbackInterface * user)
{
  std::map<std::string, Spreadsheet *>::iterator sheet = sheets.find(filename);
  
  if(sheet != sheets.end())
  {
    Spreadsheet * sh = sheet->second;
    sh->registerUser(user);
    return sheet->second;
  }
  else
  {
    //add it to the map.
    Spreadsheet * s = new Spreadsheet(filename);
    s->registerUser(user);
    sheets[filename] = s;
    return sheets[filename];
  }
}

SpreadsheetManager * SpreadsheetManager::getSpreadsheetManager()
{
  if(sm == NULL)
  {
    sm = new SpreadsheetManager();
  }
  return sm;
}

void SpreadsheetManager::saveAll()
{
  for(std::map<std::string, Spreadsheet *>::iterator sheet_it = sheets.begin(); sheet_it != sheets.end(); sheet_it++)
  {
    sheet_it->second->requestSave();
  }
}
