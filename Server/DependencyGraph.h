/* File:  DependencyGraph.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * Tracks relationships between cells in a spreadsheet, resulting from
 * formulas entered into the spreadsheet.
 */

#ifndef DEPENDENCYGRAPH_H
#define DEPENDENCYGRAPH_H

#include <map>
#include <vector>
#include <string>
#include <iterator>
#include <iostream>
#include <boost/regex.hpp>

class DependencyGraph
{
public:
  DependencyGraph();
  ~DependencyGraph();
  void addDependency(std::string s, std::string t);
  void removeDependency(std::string s, std::string t);
  std::set<std::string> getDependees(std::string s);
  std::set<std::string> getDependents(std::string s);
  void replaceDependents(std::string s, std::set<std::string> newDependents);
  void replaceDependees(std::string s, std::set<std::string> newDependents);
  void changeCell(std::string cell_name, std::string cell_contents);
  void clearDependencies(std::string s);
private:
  std::map<std::string, std::set<std::string> > dependents;
  std::map<std::string, std::set<std::string> > dependees;
  
};

#endif
