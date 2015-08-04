// Example.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <vector>
#include <list>
#include <unordered_map>

int _tmain(int argc, _TCHAR* argv[]) {
	std::vector<int> vec = { 10, 11, 12, 13, 14 };
	std::list<int> li = { 10, 11, 12, 13, 14 };
	std::unordered_map<int, int> um = { {1, 11}, {2, 22}, {3, 33} };

	return 0;
}
