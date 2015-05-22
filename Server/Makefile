
server: server_main.cpp SpreadsheetManager.o Spreadsheet.o Connection.o Users.o DependencyGraph.o
	g++ -o server -pthread -g server_main.cpp SpreadsheetManager.o Spreadsheet.o Cell.o Connection.o Users.o DependencyGraph.o /usr/local/lib/libboost_regex.a
	-mkdir spreadsheets

SpreadsheetTester: Spreadsheet.o SpreadsheetTester.cpp SpreadsheetTester.h
	g++ -g -o SpreadsheetTester SpreadsheetTester.cpp Spreadsheet.o Cell.o

SpreadsheetManager.o: SpreadsheetManager.cpp SpreadsheetManager.h
	g++ -c -g SpreadsheetManager.cpp

Spreadsheet.o: Cell.o Spreadsheet.cpp Spreadsheet.h
	g++ -c -g Spreadsheet.cpp

Cell.o: Cell.cpp Cell.h
	g++ -c -g Cell.cpp

Connection.o: Connection.cpp Connection.h
	g++ -c -g Connection.cpp

Users.o: Users.cpp Users.h
	g++ -c -g Users.cpp

DependencyGraph.o: DependencyGraph.cpp DependencyGraph.h
	g++ -c -g DependencyGraph.cpp

clean: 
	-rm *.o
	-rm *.d
	-rm a.out
	-rm SpreadsheetTester
	-rm -R *.dSYM/
	-rm server
	-rm *.o
	-rm *.d
	-rm *#
	-rm spreadsheets/*
