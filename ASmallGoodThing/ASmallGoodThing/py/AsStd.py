from AsDebuggerExtension import *

# std::vector
def IterateVector(expression, func):
	stackFrame = AsDebugger.Instance.GetCurrentStackFrame()
	sizeExpression = '(' + expression + ')._Mylast - (' + expression + ')._Myfirst' 
	size = int(stackFrame.EvaluateExpression(sizeExpression).Value)
	for i in range(size):
		elementExpression = '((' + expression + ')._Myfirst)[' + str(i) + ']'
		func(stackFrame.EvaluateExpression(elementExpression))

def LoadVector(expression, convertFunction):
	result = []
	if convertFunction is None:
		IterateVector(expression, lambda elem: result.append(elem))
	else:
		IterateVector(expression, lambda elem: result.append(convertFunction(elem)))
	return result
		
def LoadVectorValue(expression):
	return LoadVector(expression, lambda x : x.Value)
	
# std::list
def IterateList(expression, func):
	def IterateNode(node):
		func(node[indexMyval])
		next = node[indexNext]
		if next.Value != head.Value:
			IterateNode(next)

	def FindChildIndex(childName):
		for i in range(head.Children.Length):
			if head.Children[i].Name == childName:
				return i

	stdList = AsVariable(expression)
	head = stdList['_Myhead']
	
	indexMyval = FindChildIndex('_Myval')
	indexNext = FindChildIndex('_Next')

	IterateNode(head[indexNext])

def LoadList(expression, convertFunction):
	result = []
	if convertFunction is None:
		IterateList(expression, lambda elem: result.append(elem))
	else:
		IterateList(expression, lambda elem: result.append(convertFunction(elem)))
	return result
	
def LoadListValue(expression):
	return LoadList(expression, lambda x : x.Value)

# std::unordered_map
def IterateUMap(expression, func):
	unorderedMap = AsVariable(expression)
	list = unorderedMap['_List']
	for child in list.Children:
		func(child)

def LoadUMap(expression, keyConvertFunction, valueConvertFunction):
	result = {}
	if keyConvertFunction is None:
		keyConvertFunction = lambda x : x
	if valueConvertFunction is None:
		valueConvertFunction = lambda x : x
		
	IterateUMap(expression, lambda elem: result.update({ keyConvertFunction(elem['first']) : valueConvertFunction(elem['second']) }))
	return result

def LoadUMapValue(expression):
	return LoadUMap(expression, lambda x : x.Value, lambda x : x.Value)