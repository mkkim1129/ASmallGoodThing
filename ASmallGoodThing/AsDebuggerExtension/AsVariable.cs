using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;

namespace AsDebuggerExtension
{
    public class AsVariable
    {
        #region Private Fileds
        private DEBUG_PROPERTY_INFO debugPropertyInfo_;
        private AsVariable[] allChildren_ = null;
        private AsVariable[] directChildren_ = null;
        #endregion Private Fileds

        #region Public Properties
        public string Name
        {
            get
            {
                return debugPropertyInfo_.bstrName;
            }
        }

        public string Value
        {
            get
            {
                return debugPropertyInfo_.bstrValue;
            }
        }

        public string Type
        {
            get
            {
                return debugPropertyInfo_.bstrType;
            }
        }

        public AsVariable[] Children
        {
            get
            {
                if(allChildren_ == null)
                {
                    LoadAllChildren();
                }
                return allChildren_;
            }
        }

        public AsVariable this[int index]
        {
            get
            {
                if(directChildren_ == null)
                {
                    uint childCount;
                    DEBUG_PROPERTY_INFO[] childPropertyInfo = GetDirectChildrenInfo(debugPropertyInfo_, index, out childCount);
                    directChildren_ = new AsVariable[childCount];
                    directChildren_[index] = new AsVariable(childPropertyInfo[0]);
                }
                else if(directChildren_[index] == null)
                {
                    uint childCount;
                    DEBUG_PROPERTY_INFO[] childPropertyInfo = GetDirectChildrenInfo(debugPropertyInfo_, index, out childCount);
                    directChildren_[index] = new AsVariable(childPropertyInfo[0]);
                }
                return directChildren_[index];
            }
        }

        public AsVariable this[string name]
        {
            get
            {
                if(allChildren_ == null)
                {
                    LoadAllChildren();
                }

                foreach(AsVariable child in allChildren_)
                {
                    if(child.Name == name)
                    {
                        return child;
                    }
                }

                return null;
            }
        }

        #endregion Public Properties

        #region Public Methods
        public AsVariable(string expression)
        {
            AsStackFrame stackFrame = AsDebugger.Instance.GetCurrentStackFrame();
            debugPropertyInfo_ = stackFrame.EvaluateExpression(expression).debugPropertyInfo_;
        }
        
        public AsVariable(IDebugProperty3 debugProperty)
        {
            int hr = VSConstants.S_OK;

            DEBUG_PROPERTY_INFO[] propertyInfo = new DEBUG_PROPERTY_INFO[1];
            hr = debugProperty.GetPropertyInfo(
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ALL,
                100, //dwRadix
                10000, //dwTimeout
                new IDebugReference2[] { },
                0,
                propertyInfo);

            if(hr != VSConstants.S_OK)
            {
                throw new Exception("AsVariable : IDebugProperty3.GetPropertyInfo failed");
            }

            debugPropertyInfo_ = propertyInfo[0];
        }

        public AsVariable(DEBUG_PROPERTY_INFO debugPropertyInfo)
        {
            debugPropertyInfo_ = debugPropertyInfo;
        }

        public AsMemoryBlock ReadMemory(uint count)
        {
            int hr = VSConstants.S_OK;

            IDebugMemoryContext2 debugMemoryContext2;
            hr = debugPropertyInfo_.pProperty.GetMemoryContext(out debugMemoryContext2);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsVariable : GetMemoryContext failed");
            }

            IDebugMemoryBytes2 debugMemoryBytes2;
            hr = debugPropertyInfo_.pProperty.GetMemoryBytes(out debugMemoryBytes2);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsVariable : GetMemoryBytes failed");
            }

            byte[] rawMemoryBytes = new byte[count];
            uint read = 0;
            uint unreadable = 0;

            hr = debugMemoryBytes2.ReadAt(debugMemoryContext2, count, rawMemoryBytes, out read, ref unreadable);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsVariable : ReadAt failed");
            }

            return (new AsMemoryBlock(rawMemoryBytes, read));
        }

        public override string ToString()
        {
            string result = base.ToString() + Environment.NewLine;
            result += "\tName : " + this.Name + Environment.NewLine;
            result += "\tValue : " + this.Value;
            result += "\tType : " + this.Type;
            return result;
        }
        #endregion Public Methods

        #region Private Methods
        private void LoadAllChildren()
        {
            List<AsVariable> allChildren = new List<AsVariable>();
            List<AsVariable> directChildren = new List<AsVariable>();

            uint childCount;
            DEBUG_PROPERTY_INFO[] directChildrenProperty = GetDirectChildrenInfo(debugPropertyInfo_, -1, out childCount);
            foreach (DEBUG_PROPERTY_INFO childProperty in directChildrenProperty)
            {
                if (childProperty.bstrName == childProperty.bstrType)
                {
                    AsVariable parentVar = new AsVariable(childProperty);
                    AsVariable[] parentChildren = parentVar.Children;
                    allChildren.Add(parentVar);
                    allChildren.AddRange(parentChildren);
                    directChildren.Add(parentVar);
                }
                else
                {
                    AsVariable childVar = new AsVariable(childProperty);
                    allChildren.Add(childVar);
                    directChildren.Add(childVar);
                }
            }

            allChildren_ = allChildren.ToArray();
            directChildren_ = directChildren.ToArray();
        }

        private DEBUG_PROPERTY_INFO[] GetDirectChildrenInfo(DEBUG_PROPERTY_INFO debugPropertyInfo, int index, out uint childCount)
        {
            int hr = VSConstants.S_OK;
            IEnumDebugPropertyInfo2 enumDebugPropertyInfo;
            Guid guid = Guid.Empty;

            hr = debugPropertyInfo.pProperty.EnumChildren(
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME |
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE |
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE |
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP |
                enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE_RAW,
                10,
                ref guid,
                enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_CHILD_ALL,
                null,
                10000,
                out enumDebugPropertyInfo);

            if(hr != VSConstants.S_OK)
            {
                throw new Exception("AsVariable : GetDirectChildrenInfo, EnumChildren failed");
            }

            if(enumDebugPropertyInfo == null)
            {
                childCount = 0;
                return null;
            }

            uint readCount;
            if(index == -1)
            {
                hr = enumDebugPropertyInfo.GetCount(out childCount);
                if(hr != VSConstants.S_OK)
                {
                    throw new Exception("AsVariable : GetDirectChildrenInfo, GetCount failed");
                }

                readCount = childCount;
            }
            else
            {
                hr = enumDebugPropertyInfo.GetCount(out childCount);
                if(hr != VSConstants.S_OK)
                {
                    throw new Exception("AsVariable : GetDirectChildrenInfo, GetCount failed");
                }

                if(childCount <= (uint)index)
                {
                    throw new IndexOutOfRangeException();
                }

                readCount = 1;
                hr = enumDebugPropertyInfo.Skip((uint)index);
                if (hr != VSConstants.S_OK)
                {
                    throw new Exception("AsVariable : GetDirectChildrenInfo, Skip failed");
                }
            }

            DEBUG_PROPERTY_INFO[] childPropertyList = new DEBUG_PROPERTY_INFO[readCount];

            uint fetched;
            hr = enumDebugPropertyInfo.Next(readCount, childPropertyList, out fetched);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsVariable : GetDirectChildrenInfo, Next failed");
            }

            return childPropertyList;
        }
        #endregion Private Methods
    }
}
