from AsDebuggerExtension import *

# std::vector
def IterateVector(stdVector, func):
	objectType = stdVector['_Myfirst'].Type
	sizeExpression = '(' + stdVector['_Mylast'].Value + '-' + stdVector['_Myfirst'].Value + ') / sizeof(' + objectType[:-2] + ')'	
	size = int(AsVariable(sizeExpression).Value)

	for i in range(size):
		elementExpression = '*((' + objectType + ')' + stdVector['_Myfirst'].Value + '+' + str(i) + ')'
		func(AsVariable(elementExpression))

def LoadVector(stdVector, convertFunction):
	result = []
	if convertFunction is None:
		IterateVector(stdVector, lambda elem: result.append(elem))
	else:
		IterateVector(stdVector, lambda elem: result.append(convertFunction(elem)))
	return result
		
# std::list
def IterateList(stdList, func):
	def IterateNode(node):
		func(node[indexMyval])
		next = node[indexNext]
		if next.Value != head.Value:
			IterateNode(next)

	def FindChildIndex(childName):
		for i in range(head.Children.Length):
			if head.Children[i].Name == childName:
				return i

	head = stdList['_Myhead']
	
	indexMyval = FindChildIndex('_Myval')
	indexNext = FindChildIndex('_Next')

	IterateNode(head[indexNext])

def LoadList(stdList, convertFunction):
	result = []
	if convertFunction is None:
		IterateList(stdList, lambda elem: result.append(elem))
	else:
		IterateList(stdList, lambda elem: result.append(convertFunction(elem)))
	return result
	
# std::unordered_map
def IterateUMap(unorderedMap, func):
	list = unorderedMap['_List']
	for child in list.Children:
		func(child)

def LoadUMap(unorderedMap, keyConvertFunction, valueConvertFunction):
	result = {}
	if keyConvertFunction is None:
		keyConvertFunction = lambda x : x
	if valueConvertFunction is None:
		valueConvertFunction = lambda x : x
		
	IterateUMap(unorderedMap, lambda elem: result.update({ keyConvertFunction(elem['first']) : valueConvertFunction(elem['second']) }))
	return result
