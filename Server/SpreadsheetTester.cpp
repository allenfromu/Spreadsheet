#include "SpreadsheetTester.h"
#include "Spreadsheet.h"

#include "Cell.h"
#include <iostream>
#include <string>

int main()
{
  SpreadsheetTester test;
  test.test();
}

void SpreadsheetTester::test()
{
  Spreadsheet sheet("Test");
  
  sheet.updateCell("A3", "Test1");
  sheet.updateCell("A4", "Test2");
  sheet.updateCell("A4", "Test1");
  
  //void (*callback)(std::string, std::string);
  //callback = &(const SpreadsheetTester::testCallback);
  
  
  
  sheet.registerUser((SpreadsheetCallbackInterface *)this);
  sheet.requestUndo();
  
  std::string sheetString = sheet.toString();
  
  std::cout << sheet.toString();
  
  sheet.requestSave();
  
  return;
}

void SpreadsheetTester::cellChangedCallback(std::string cell_name, std::string cell_value)
{
  std::cout << "Got the Callback for " << cell_name << " with value " << cell_value << std::endl;
}