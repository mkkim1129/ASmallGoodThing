// Example.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <vector>
#include <list>
#include <unordered_map>
#include <map>
#include <string>

struct Foo {
	Foo() : index_(-1){}
	Foo(int index) :index_(index){}

	int index_;
	std::vector<int> vec_;
};

int _tmain(int argc, _TCHAR* argv[]) {
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

	Foo foo;
	foo.vec_ = { 110, 111, 112, 113, 114 };

	std::vector<Foo> vec_foo = { Foo(100), Foo(200), Foo(300) };
	vec_foo[0].vec_ = { 100, 101, 102 };
	vec_foo[1].vec_ = { 200, 201, 202 };
	vec_foo[2].vec_ = { 300, 301, 302 };
		
	std::vector<Foo*> vec_foo_ptr = { new Foo(10), new Foo(11), new Foo(12) };

	return 0;
}
