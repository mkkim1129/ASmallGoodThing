from AsDebuggerExtension import *

def LoadContainer(expression, iterateFunction, convertFunction):
	result = []
	if convertFunction is None:
		iterateFunction(expression, lambda elem: result.append(elem))
	else:
		iterateFunction(expression, lambda elem: result.append(convertFunction(elem)))
	return result

# std::vector
def IterateVector(expression, func):
	stackFrame = AsDebugger.Instance.GetCurrentStackFrame()
	sizeExpression = '(' + expression + ')._Mylast - (' + expression + ')._Myfirst' 
	size = int(stackFrame.EvaluateExpression(sizeExpression).Value)
	for i in range(size):
		elementExpression = '((' + expression + ')._Myfirst)[' + str(i) + ']'
		func(stackFrame.EvaluateExpression(elementExpression))

def LoadVector(expression):
	return LoadContainer(expression, IterateVector, None)
	
def LoadVectorValue(expression):
	return LoadContainer(expression, IterateVector, lambda x : x.Value)
	
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

def LoadList(expression):
	return LoadContainer(expression, IterateList, None)
	
def LoadListValue(expression):
	return LoadContainer(expression, IterateList, lambda x : x.Value)

# std::unordered_map
def IterateUMap(expression, func):
	unorderedMap = AsVariable(expression)
	list = unorderedMap['_List']
	for child in list.Children:
		func(child)

def LoadUMap(expression):
	result = {}
	IterateUMap(expression, lambda elem: result.update({elem['first']:elem['second']}))
	return result