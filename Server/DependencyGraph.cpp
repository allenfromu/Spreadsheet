/* File:  DependencyGraph.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * Tracks relationships between cells in a spreadsheet, resulting from
 * formulas entered into the spreadsheet.
 */

#include "DependencyGraph.h"

DependencyGraph::DependencyGraph()
{
  
}

DependencyGraph::~DependencyGraph()
{
  
}

void DependencyGraph::addDependency(std::string s, std::string t)
{
	dependents[s].insert(t);
  dependees[t].insert(s);
}

void DependencyGraph::removeDependency(std::string s, std::string t)
{
	dependents[s].erase(t);
  dependees[t].erase(s);
}

std::set<std::string> DependencyGraph::getDependees(std::string s)
{
  return dependees[s];
}

std::set<std::string> DependencyGraph::getDependents(std::string s)
{
	return dependents[s];
}

void DependencyGraph::clearDependencies(std::string s)
{
  for(std::set<std::string>::iterator dep_it = dependents[s].begin(); dep_it != dependents[s].end(); ++dep_it)
  {
    dependees[*dep_it].erase(s);
  }
  dependents[s].clear();
}

void DependencyGraph::replaceDependents(std::string s, std::set<std::string> newDependents)
{
  for(std::set<std::string>::iterator dep_it = dependents[s].begin(); dep_it != dependents[s].end(); ++dep_it)
  {
    dependees[*dep_it].erase(s);
  }
  dependents[s].clear();
  for(std::set<std::string>::iterator dep_it = newDependents.begin(); dep_it != newDependents.end(); ++dep_it)
  {
    addDependency(s, *dep_it);
  }
}

void DependencyGraph::replaceDependees(std::string s, std::set<std::string> newDependents)
{
  for(std::set<std::string>::iterator dep_it = dependents[s].begin(); dep_it != dependents[s].end(); ++dep_it)
  {
    dependees[*dep_it].erase(s);
  }
  dependents[s].clear();
  for(std::set<std::string>::iterator dep_it = newDependents.begin(); dep_it != newDependents.end(); ++dep_it)
  {
    addDependency(*dep_it, s);
  }
}

// bool regex_callback(const boost::match_results<std::string::const_iterator>& match)
// {
//   std::string match_string = match[1];
//   addDependency()
//   return true;
// }

void DependencyGraph::changeCell(std::string cell_name, std::string cell_contents)
{
  clearDependencies(cell_name);
  
  boost::regex expression("[a-zA-Z]+[0-9]+");
  boost::sregex_iterator cellName_begin(cell_contents.begin(), cell_contents.end(), expression);
  boost::sregex_iterator cellName_end;
  
  int results = std::distance(cellName_begin, cellName_end);
  
  std::cout << "Found " << results << " Matches" << std::endl;
  
  if(results == 0)
  {
    return;
  }
  int processed = 0;
  for(boost::sregex_iterator i = cellName_begin; i != cellName_end; ++i)
  {
    boost::smatch match = *i;
    std::string match_str = match[0];
    std::cout << "Found Cell For Dependency " << match_str << std::endl; 
    addDependency(cell_name, match_str);
    std::cout << "Processed " << processed << " Results" << std::endl;
  }
}
