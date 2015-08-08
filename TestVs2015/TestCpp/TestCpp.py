from vs2015 import *

def CompareList(lhs, rhs):
	if len(lhs) != len(rhs):
		return False
	for i in range(len(lhs)):
		if lhs[i] != rhs[i]:
			return False
	return True
	
def CompareDictionary(lhs, rhs):
	if len(lhs) != len(rhs):
		return False
	for k in lhs:
		if k not in rhs:
			return False
		if lhs[k] != rhs[k]:
			return False
	return True
	
convertToInt = lambda x : int(x.Value)
	
v = LoadVector(AsVariable('v'), convertToInt)
print str(CompareList(v, [10, 11, 12, 13, 14])) + " : LoadVector"

l = LoadList(AsVariable('l'), convertToInt)
print str(CompareList(l, [10, 11, 12, 13, 14])) + " : LoadList"

u = LoadUMap(AsVariable('u'), convertToInt, convertToInt)
print str(CompareDictionary(u, {1: 11, 2: 22, 3: 33})) + " : LoadUMap"

m = LoadMap(AsVariable('m'), convertToInt, convertToInt)
print str(CompareDictionary(m, {1: 11, 2: 22, 3: 33})) + " : LoadMap"

s1 = LoadString(AsVariable('s1'))
print str(s1 == 'hello') + " : LoadString"

s2 = LoadWString(AsVariable('s2'))
print str(s2 == 'world') + " : LoadWString"

s3 = LoadCharArray(AsVariable('s3'), 255)
print str(s3 == 'a small') + " : LoadCharArray"

s4 = LoadWCharArray(AsVariable('s4'), 255)
print str(s4 == 'good thing') + " : LoadWCharArray"

s5 = LoadCharArray(AsVariable('s5'), 255)
print str(s5 == 'python') + " : LoadCharArray"

s6 = LoadWCharArray(AsVariable('s6'), 255)
print str(s6 == 'c sharp') + " : LoadWCharArray"