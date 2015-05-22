/* File:  Users.h
 * Written by Daniel Kenner, Zepeng Zhao, Jack Stafford and Ella Ortega
 * For CS 3505 Spring 2015 Collaborative Spreadsheet Project
 * 
 * Represents authorized/registered spreadsheet users.  Allows user names
 * to be added, removed, checked/verified and saved. Can get a list of
 * authorized users.
 */

#ifndef USERS_H
#define USERS_H

#include <set>
#include <string>
#include <cstddef>
#include <iostream>
#include <fstream>
using namespace std;
class Users
{
  public:
    static Users * getUsers();
    bool checkUser(std::string name);
    void addUser(std::string name);
    bool removeUser(std::string name);
    void save();
    void load();

  private:
    static Users * users;
       
    Users() {};
    
    Users(Users const&) {};//don't implement
    void operator=(Users const&) {};//don't implement
    ~Users() {};//want to be private...we'll see what happens.
    
    std::set<std::string> user_list;
  
};

#endif
