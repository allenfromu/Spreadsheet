/* File:  Cell.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * Represents a cell in a spreadsheet
 */

#include "Cell.h"

Cell::Cell()
{
	
}

Cell::Cell(std::string contents)
{
	cell_contents = contents;
} 

Cell::~Cell()
{
	
}

void Cell::setContents(std::string contents)
{
	cell_contents = contents;
}

std::string Cell::getContents()
{
	return cell_contents;
}
