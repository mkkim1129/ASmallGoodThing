# A Small, Good Thing
## 별것 아닌 것 같지만, 도움이 되는
"별것 아닌 것 같지만, 도움이 되는"은 디버깅 할 때 조사식을 스크립팅 할 수 있도록 도와주는 Visual Studio Extension 입니다.

## 설치
* [A Small, Good Thing 설치파일](https://github.com/mkkim1129/ASmallGoodThing/blob/master/Install/ASmallGoodThing.vsix?raw=true)
* [IronePython](http://ironpython.net/)

정상적으로 설치되면 Visual Studio 메뉴-"Tools"-"Script - A Small, Good Thing"이 나타납니다.

## 사용환경
* Visual Studio 2013
* Visual Studio 2015

## 예제
### 예제1 - 기본
C++
```c++
int n = 123;
```
Python
```python
>>> n = AsVariable('n')
>>> print n
AsDebuggerExtension.AsVariable
	Name : n
	Value : 123
	Type : int
>>> print n.Name
n
>>> print n.Value
123
>>> print n.Type
int

```

### 예제2 - 객체
C++
```c++
enum Gender {
	kMale,
	kFemale
};

struct Person {
	int age;
	Gender gender;
};

Person p = { 2, Gender::kFemale };
```
Python
```python
>>> p = AsVariable('p')
>>> print p
AsDebuggerExtension.AsVariable
	Name : p
	Value : {age=2 gender=kFemale (1) }
	Type : Person
>>> for c in p.Children: print c
AsDebuggerExtension.AsVariable
	Name : age
	Value : 2
	Type : int
AsDebuggerExtension.AsVariable
	Name : gender
	Value : kFemale
	Type : Gender
>>> print p[0]
AsDebuggerExtension.AsVariable
	Name : age
	Value : 2
	Type : int
>>> print p[1]
AsDebuggerExtension.AsVariable
	Name : gender
	Value : kFemale
	Type : Gender
>>> print p['age']
AsDebuggerExtension.AsVariable
	Name : age
	Value : 2
	Type : int
>>> print p['gender']
AsDebuggerExtension.AsVariable
	Name : gender
	Value : kFemale
	Type : Gender

```

### 예제3 - stl컨테이너
C++
```c++
std::vector<int> v = { 10, 11, 12, 13, 14 };
```
Python
```python
>>> v = AsVariable('v')
>>> print v
AsDebuggerExtension.AsVariable
	Name : v
	Value : { size=5 }
	Type : std::vector<int,std::allocator<int> >
>>> from vs2015 import *
>>> pyList = LoadVector(AsVariable('v'), lambda x : int(x.Value))
>>> print pyList
[10, 11, 12, 13, 14]

```
STL 컨테이너 관련 함수
* std::vector - IterateVector, LoadVector
* std::list - IterateList, LoadList
* std::unordered_map - IterateUMap, LoadUMap
* std::map - IterateMap, LoadMap

문자열 관련 함수
* std::string - LoadString
* std::wstring - LoadWString
* char array - LoadCharArray
* wchar_t array - LoadWCharArray


