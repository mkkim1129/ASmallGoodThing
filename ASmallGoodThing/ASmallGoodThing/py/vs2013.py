from AsDebuggerExtension import *

# std::vector #################################################################
def IterateVector(stdVector, func):
	objectType = stdVector['_Myfirst'].Type
	sizeExpression = '(' + stdVector['_Mylast'].Value + '-' + stdVector['_Myfirst'].Value + ') / sizeof(' + objectType[:-2] + ')'	
	size = int(AsVariable(sizeExpression).Value)
	myfirstValue = stdVector['_Myfirst'].Value
	for i in range(size):
		elementExpression = '*((' + objectType + ')' + myfirstValue + '+' + str(i) + ')'
		func(AsVariable(elementExpression))

def LoadVector(stdVector, convertFunction):
	result = []
	if convertFunction is None:
		IterateVector(stdVector, lambda elem: result.append(elem))
	else:
		IterateVector(stdVector, lambda elem: result.append(convertFunction(elem)))
	return result
		
# std::list ###################################################################
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
	
# std::unordered_map ##########################################################
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

# std::map ####################################################################
def IterateMap(stdMap, func):
	def IterateNode(node):
		func(node[indexMyval])
		left = node[indexLeft]
		if left.Value != headValue:
			IterateNode(left)
		right = node[indexRight]
		if right.Value != headValue:
			IterateNode(right)

	def FindChildIndex(childName):
		for i in range(head.Children.Length):
			if head.Children[i].Name == childName:
				return i

	head = stdMap['_Myhead']
	headValue = head.Value

	indexMyval = FindChildIndex('_Myval')
	indexLeft = FindChildIndex('_Left')
	indexRight = FindChildIndex('_Right')

	IterateNode(head['_Parent'])

def LoadMap(stdMap, keyConvertFunction, valueConvertFunction):
	result = {}
	if keyConvertFunction is None:
		keyConvertFunction = lambda x : x
	if valueConvertFunction is None:
		valueConvertFunction = lambda x : x
		
	IterateMap(stdMap, lambda elem: result.update({ keyConvertFunction(elem['first']) : valueConvertFunction(elem['second']) }))
	return result

# std::string #################################################################
def LoadString(variable):
	size = int(variable['_Mysize'].Value)
	bufName = '_Buf'
	if 16<= size:
		bufName = '_Ptr'
	memoryBlock = variable['_Bx'][bufName].ReadMemory(size)
	return memoryBlock.ConvertToAsciiString()

def LoadWString(variable):
	size = int(variable['_Mysize'].Value)
	bufName = '_Buf'
	if 16<= size:
		bufName = '_Ptr'
	memoryBlock = variable['_Bx'][bufName].ReadMemory(size*2)
	return memoryBlock.ConvertToUnicodeString()

# Null Terminated String ######################################################
def LoadCharArray(variable, maxBytes):
	memoryBlock = variable.ReadMemory(maxBytes)
	return memoryBlock.ConvertToNullTerminatedAsciiString()

def LoadWCharArray(variable, maxBytes):
	memoryBlock = variable.ReadMemory(maxBytes)
	return memoryBlock.ConvertToNullTerminatedUnicodeString()
