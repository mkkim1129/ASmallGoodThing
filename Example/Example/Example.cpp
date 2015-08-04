// Example.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <vector>
#include <list>
#include <unordered_map>

struct Foo {
	Foo(){}
	Foo(int index) :index_(index){}

	int index_;
	std::vector<int> vec_;
};

int _tmain(int argc, _TCHAR* argv[]) {
	std::vector<int> vec = { 10, 11, 12, 13, 14 };
	std::list<int> li = { 10, 11, 12, 13, 14 };
	std::unordered_map<int, int> um = { {1, 11}, {2, 22}, {3, 33} };

	Foo foo;
	foo.vec_ = { 110, 111, 112, 113, 114 };

	std::vector<Foo> vec_foo = { Foo(100), Foo(200), Foo(300) };
	vec_foo[0].vec_ = { 100, 101, 102 };
	vec_foo[1].vec_ = { 200, 201, 202 };
	vec_foo[2].vec_ = { 300, 301, 302 };
		
	std::vector<Foo*> vec_foo_ptr = { new Foo(10), new Foo(11), new Foo(12) };

	return 0;
}
