/* File:  Spreadsheet.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * Represents a single cell in a spreadsheet
 */

#ifndef CELL_H
#define CELL_H

#include <string>

class Cell
{
public:
  Cell(); //blank cell...probably just going to be used for data structures
  Cell(std::string contents); //this is string for right now, it'll need to be changed to a formula object later to be able to deal with circular dependencies.
  ~Cell();
  void setContents(std::string contents);
  std::string getContents();
  
private:
  std::string cell_contents;
  
};

#endif
