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
	
vec = LoadVector(AsVariable('vec'), convertToInt)
print str(CompareList(vec, [10, 11, 12, 13, 14])) + " : LoadVector"

li = LoadList(AsVariable('li'), convertToInt)
print str(CompareList(li, [10, 11, 12, 13, 14])) + " : LoadListValue"

um = LoadUMap(AsVariable('um'), convertToInt, convertToInt)
print str(CompareDictionary(um, {1: 11, 2: 22, 3: 33})) + " : LoadUMapValue"
