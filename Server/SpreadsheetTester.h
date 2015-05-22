#ifndef SPREADSHEETTESTER_H
#define SPREADSHEETTESTER_H

#include <string>
#include "SpreadsheetCallbackInterface.h"

class SpreadsheetTester : public SpreadsheetCallbackInterface
{
public:
  void test();
  void cellChangedCallback(std::string cell_name, std::string cell_value);
};

#endif