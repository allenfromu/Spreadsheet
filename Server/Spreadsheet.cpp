/* File:  Spreadsheet.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * To create a new spreadsheet, open one from a file, save to a file.
 */

#include "Spreadsheet.h"

void Spreadsheet::requestSave()
{
  if(users.size()==1)
    Save();
}
void Spreadsheet::updateCell(std::string cell_name, std::string cell_contents, SpreadsheetCallbackInterface * caller)
{
  
  pthread_mutex_lock(&lock1);
   
  std::map<std::string, Cell>::iterator cellContents = cells.find(cell_name);
  if(cellContents != cells.end())
  {
    //put the old stuff into the undo stack
    std::pair<std::string, std::string> undoElement(cell_name, cellContents->second.getContents());
    undoStack.push_back(undoElement);
  }
  else
  {
    //push an empty element to the stack.
    std::pair<std::string, std::string> undoElement(cell_name, "");
    undoStack.push_back(undoElement);
  }
  //will need to update when cell can take a formula...
  cells[cell_name].setContents(cell_contents);
  
  if(cell_contents.substr(0,1).compare("=") == 0 )
  {
    dependencies.changeCell(cell_name, cell_contents);
    
    bool circular = checkForCircularDependency(cell_name);
    if(circular)
    {
      //do an undo and send out an error
      pthread_mutex_unlock(&lock1);
      requestUndo();
      //send an error
      std::cout << "ERROR GOES HERE...NEED TO IMPLEMENT" << std::endl;
      caller -> errorCallback(2, "circular dependency");
    }
    else
    {
      pthread_mutex_unlock(&lock1);
      notifyUsers(cell_name, cell_contents);
    }
  }
  else
  {
    pthread_mutex_unlock(&lock1);
    notifyUsers(cell_name, cell_contents);
  }
  
}

std::string Spreadsheet::toUpper(std::string src)
{
  char buff[src.size()];
  
  src.copy(buff, src.size(), 0);
  
  int i;
  for(i = 0; i < src.size(); i++)
  {
    buff[i] = toupper(buff[i]);
  }
  
  std::string ret(buff);
  
  return ret;
  
}

bool Spreadsheet::checkForCircularDependency(std::string cell_name)
{
  std::set<std::string> visited;
  bool circular = Visit(cell_name, cell_name, &visited);
  return circular;
}

bool Spreadsheet::Visit(std::string start, std::string name, std::set<std::string> * visited)
{
  visited->insert(name);
  std::set<std::string> cell_dependents = dependencies.getDependents(name);
  bool found_cycle = false;
  for(std::set<std::string>::iterator dep_it = cell_dependents.begin(); dep_it != cell_dependents.end(); dep_it++)
  {
    std::string cell_name = *dep_it;
    if (cell_name == start)
    {
      //circular dependency detected
      found_cycle = true;
    }
    //if it hasn't been visited,
    else if(visited->find(cell_name) == visited->end())
    {
      //visit it
      bool visitReturn = Visit(start, cell_name, visited);
      if(!found_cycle)
      {
        found_cycle = visitReturn;
      }
    }
  }
  return found_cycle;
}

void Spreadsheet::registerUser(SpreadsheetCallbackInterface * user)
{
  std::cout << "user registered" << std::endl;
  pthread_mutex_lock(&lock2);
  users.insert(user);
  std::cout << "# Users Registered: " << users.size() << std::endl;
  
  //send init info for spreadsheet
  user->connectedCallback(cells.size());
  for(std::map<std::string, Cell>::iterator cells_it = cells.begin(); cells_it != cells.end(); cells_it++)
  {
    user->cellChangedCallback(cells_it->first, cells_it->second.getContents());
  }
  pthread_mutex_unlock(&lock2);
}

void Spreadsheet::removeUser(SpreadsheetCallbackInterface * user)
{
  std::cout << "user removed" << std::endl;
  pthread_mutex_lock(&lock2);
  users.erase(user);
  pthread_mutex_unlock(&lock2);
  std::cout << "# Users Registered: " << users.size() << std::endl;
  if(users.size() == 0)
  {
    Save();
  }
}



Spreadsheet::Spreadsheet()
{
  //any spreadsheet-wide info goes here...
}
Spreadsheet::Spreadsheet(std::string filename)
{
  pthread_mutex_init(&lock1, NULL);
  pthread_mutex_init(&lock2, NULL);
  pthread_mutex_unlock(&lock1);
  pthread_mutex_unlock(&lock2);
  
  Spreadsheet();
  name = filename;
  
   Load(name);
 
}
Spreadsheet::~Spreadsheet()
{
  //destructor...not entirely sure what needs to be done here other than possible
  Save();
}
void Spreadsheet::Save()
{
  std::string to_write = "";
  to_write = to_write + "FILENAME:"+ name+"\n";
  for (std::map<std::string, Cell>::iterator cells_it = cells.begin(); cells_it != cells.end(); cells_it++)
  {
    to_write = to_write + "CELL:" + cells_it->first +","+cells_it->second.getContents()+"\n";
  }
  
  std::ofstream f;
  std::string filename = "spreadsheets/"+name;
  f.open(filename.c_str());
  f << to_write;
  f.close();
  
  return;
}
bool Spreadsheet::Load(std::string filename)
{
  //load the file
  std::string line;
  int colpos, compos;
  std::string command, other, cell, value;
  filename = "spreadsheets/"+filename;
  std::ifstream f (filename.c_str());
  if(f.is_open())
  {
    while(getline(f,line))
    {
      //process lines
      //first split, at :
      colpos = line.find(":");
      if(colpos != std::string::npos && colpos > 0)
      {
        command = line.substr(0,colpos);
        other = line.substr(colpos+1);
        //should implement toUpper, will do later.
        //command = toupper(command);
        if(command == "FILENAME")
        {
          //we can skip this for now -- It's just included in case it is needed.
        }
        else if(command == "CELL")
        {
          compos = other.find(",");
          if(compos != std::string::npos) 
          {
            cell = other.substr(0,compos);
            value = other.substr(compos+1);
            //updateCell(cell, value);
            //want to load without all the other stuff...
            cells[cell].setContents(value);
            dependencies.changeCell(cell, value);
          }
        }
      }
    }
    f.close();
    return true;
  }
  else
  {
    return false;
  }
}

std::string Spreadsheet::toString()
{
  std::string ret = "";
  ret = ret + "Spreadsheet Filename: "+ name+"\n";
  for (std::map<std::string, Cell>::iterator cells_it = cells.begin(); cells_it != cells.end(); cells_it++)
  {
    ret = ret + "Cell: Name: " + cells_it->first +", Value: " +cells_it->second.getContents()+"\n";
  }
  return ret;
}


void Spreadsheet::requestUndo()
{

  //check if anything is on the stack.
  if(undoStack.size() > 0)
  {
    pthread_mutex_lock(&lock1);
    //pop the last action off the stack and apply it.
    std::pair<std::string, std::string> undoElement = undoStack.back();
    undoStack.pop_back();
    //updateCell(undoElement.first, undoElement.second);
    cells[undoElement.first].setContents(undoElement.second);
    dependencies.changeCell(undoElement.first, undoElement.second);
    pthread_mutex_unlock(&lock1);
    notifyUsers(undoElement.first, undoElement.second);
    
  }
  
}

int Spreadsheet::getSize()
{
  return cells.size();
}

void Spreadsheet::notifyUsers(std::string cell, std::string value)
{
  std::cout << "notifying users" << std::endl;
  std::cout << "users available: " << users.size() << std::endl;
  
  
  pthread_mutex_lock(&lock2);
  for (std::set<SpreadsheetCallbackInterface *>::iterator users_it = users.begin(); users_it != users.end(); users_it++)
  {
    std::cout << "notify" << std::endl;
    //void (*callback)(std::string, std::string);
    SpreadsheetCallbackInterface * callback;
    callback = *users_it;
    callback->cellChangedCallback(cell, value);
  }
  pthread_mutex_unlock(&lock2);
}
