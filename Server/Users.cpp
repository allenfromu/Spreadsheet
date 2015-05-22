/* File:  Users.cpp
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * Represents authorized/registered spreadsheet users.  Allows user names
 * to be added, removed, checked/verified and saved. Can get a list of
 * authorized users.
 */

#include "Users.h"

Users *Users::users = NULL;

bool Users::checkUser(std::string name)
{
  std::cout << "checking " << name << std::endl;
  std::set<std::string>::iterator user_it = user_list.find(name);
  
  if(user_it != user_list.end())
  {
    return true;
  }
  else
  {
    return false;
  }
}
void Users::addUser(std::string name)
{
  std::cout << "adding " << name << std::endl;
  user_list.insert(name);
  
}
bool Users::removeUser(std::string name)
{
  std::set<std::string>::iterator user_it = user_list.find(name);
  if(user_it != user_list.end())
  {
    //remove it
    user_list.erase(user_it);
    return true;
  }
  else
  {
    return false;
  }
}

Users * Users::getUsers()
{
  if(users == NULL)
  {
    users = new Users();
  }
  return users;
}

void Users::load(){
  ifstream userf;
  userf.open("users.txt");
  if(userf.is_open()){
    string line;
    while(getline(userf,line)){
      this->addUser(line);
    }
    userf.close();
  }else{
    this->addUser("sysadmin");
  }
}

void Users::save(){
  ofstream userf;
  userf.open("users.txt");
  for(std::set<std::string>::iterator user_it = user_list.begin();user_it!=user_list.end();user_it++){
    userf << *user_it<<endl;
  }
  userf.close();
}
