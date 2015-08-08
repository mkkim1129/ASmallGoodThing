// TestCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <vector>
#include <list>
#include <unordered_map>
#include <map>
#include <string>

enum Gender {
	kMale,
	kFemale
};

struct Person {
	int age;
	Gender gender;
};

int wmain(int argc, wchar_t* argv[]) {
  int n = 123;
	Person p = { 2, Gender::kFemale };

  std::vector<int> v = { 10, 11, 12, 13, 14 };
  std::list<int> l = { 10, 11, 12, 13, 14 };
  std::unordered_map<int, int> u = { { 1, 11 }, { 2, 22 }, { 3, 33 } };
  std::map<int, int> m = { { 1, 11 }, { 2, 22 }, { 3, 33 } };

  std::string s1 = "hello";
  std::wstring s2 = L"world";

  char * s3 = "a small";
  wchar_t * s4 = L"good thing";

  char s5[] = "python";
  wchar_t s6[] = L"c sharp";

  return 0;
}

