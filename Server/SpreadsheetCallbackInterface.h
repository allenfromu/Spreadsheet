/* File:  SpreadsheetCallbackInterface.h
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * This interface declares the callbacks to be used in the Project
 * to signal changes in state.
 */

#ifndef SPREADSHEETCALLBACKINTERFACE_H
#define SPREADSHEETCALLBACKINTERFACE_H

#include <string>

class SpreadsheetCallbackInterface
{
public:
  virtual void cellChangedCallback(std::string cell_name, std::string cell_value) = 0;
  virtual void errorCallback(int errorType, std::string description) = 0;
  virtual void connectedCallback(int num_cells) = 0;
};

#endif
