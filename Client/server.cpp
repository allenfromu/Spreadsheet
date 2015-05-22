#include <stdio.h>
#include <iostream>
#include <stdlib.h>
#include <cstdlib>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <pthread.h>

using namespace std;

int equal(char *first, char* second){
    cout<<first[0]<<endl;
    cout<<second[0]<<endl;
  if(first[0] != second[0])
    return 0;
  while(first[0] != '\0' && second[0]!='\0'){
    cout<<first[0]<<endl;
    cout<<second[0]<<endl;
    first++;
    second++;
    if(first[0] + 1== 14 && second[0] + 1== 1)
      return 1;
    if(first[0] != second[0] ){
    cout<<first[0]+1<<endl;
    cout<<second[0]+1<<endl;
      return 0;}
  }
  cout<<"here"<<endl;
  return 1;
}
void error(const char *msg)
{
  perror(msg);
  exit(1);
}

void *client(void *clisock){
  int *newsockfd = (int *)clisock;
  while(1){
      char buffer[256];
      int n;
      bzero(buffer,256);
      n = read(*newsockfd,buffer,255);
      if (n < 0) error("ERROR reading from socket");
      printf("Here is the message: %s\n",buffer);
      char logout[9] ="log out\0";
      if(equal(buffer, logout))
        break;
      n = write(*newsockfd,"I got your message",18);
      if (n < 0) error("ERROR writing to socket");
  }
      close(*newsockfd);
      exit(1);
}


static int temp = 444444444;
int main(int argc, char *argv[]){
  //port number should be passed from argv
  if(argc != 2){
    cout<<"arguments should pass in host and port number."<<argc<<endl;
    return 1;
  }
  //create socket
  int sockfd = socket(AF_INET, SOCK_STREAM, 0);
  
  if(sockfd < 0)
    error("ERROR opening socket.");
  
  struct sockaddr_in serv_addr, cli_addr;
  socklen_t clilen;
  int portno, newsockfd;
  //set struct buffer to all 0
  bzero((char *) &serv_addr, sizeof(serv_addr));
  //convert port number to int
  portno = atoi(argv[1]);
  
  //configure sockadd_in struct
  serv_addr.sin_family = AF_INET;
  serv_addr.sin_addr.s_addr = INADDR_ANY;
  serv_addr.sin_port = htons(portno);
  
  if (bind(sockfd, (struct sockaddr *) &serv_addr,sizeof(serv_addr)) < 0)
    error("ERROR on binding");
  //set number of requests in the queue
  listen(sockfd,5);
  
  clilen = sizeof(cli_addr);
  pthread_t threads[5];
  int i = 0;
  while(1){
    cout<<"Waiting for connection"<<endl;
    newsockfd = accept(sockfd,(struct sockaddr *) &cli_addr,&clilen);
    if (newsockfd < 0)
      error("ERROR on accept");
    /* int rc = pthread_create(&threads[i],NULL,client,(void *)&newsockfd);
    if(rc)
    error("thread fail");*/
   
    char buffer[256];
    int n;
    bzero(buffer,256);
    n = read(newsockfd,buffer,255);
    if (n < 0) error("ERROR reading from socket");
    printf("Here is the message: %s\n",buffer);
    cout<<temp<<endl;
    n = write(newsockfd,"I got your message",18);
    if (n < 0) error("ERROR writing to socket");
    close(newsockfd);
    //exit(1);
    
  }
  close(sockfd);
  return 0; 




}
