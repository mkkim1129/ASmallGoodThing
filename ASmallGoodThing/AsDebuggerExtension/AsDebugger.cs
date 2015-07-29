using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace AsDebuggerExtension
{
    public sealed class AsDebugger
    {
        #region Private Fileds
        private static volatile AsDebugger instance_;
        private static object syncObject_ = new Object();

        private IDebuggerInternal11 debuggerServiceInternal_;
        private DTE dte_;
        #endregion Private Fileds

        #region Public Properties
        public static AsDebugger Instance
        {
            get
            {
                if(instance_ == null)
                {
                    lock(syncObject_)
                    {
                        if(instance_ == null)
                        {
                            instance_ = new AsDebugger();
                        }
                    }
                }
                return instance_;
            }
        }
        #endregion Public Properties

        #region Public Methods
        public AsStackFrame[] GetCallStack()
        {
            FRAMEINFO[] frameInfo = GetCallStackInternal();
            AsStackFrame[] callstack = Array.ConvertAll(frameInfo, x => new AsStackFrame(x));
            return callstack;
        }

        public AsStackFrame GetCurrentStackFrame()
        {
            if(dte_.Debugger.CurrentStackFrame == null)
            {
                throw new Exception("AsDebugger : No current stack frame");
            }

            EnvDTE90a.StackFrame2 currentFrame2 = dte_.Debugger.CurrentStackFrame as EnvDTE90a.StackFrame2;
            if(currentFrame2 == null)
            {
                throw new Exception("AsDebugger : CurrentStackFrame is not a StackFrame2");
            }

            string currentFunctionName = currentFrame2.FunctionName;

            FRAMEINFO[] callstack = GetCallStackInternal();

            FRAMEINFO frameInfo = Array.Find(callstack, x => (x.m_bstrFuncName == currentFunctionName));
            if (frameInfo.m_pFrame == null)
            {
                throw new Exception("AsDebugger : Current stack frame is null");
            }

            return new AsStackFrame(frameInfo);
        }
        #endregion Public Methods

        #region Private Methods
        private AsDebugger()
        {
            debuggerServiceInternal_ = Package.GetGlobalService(typeof(SVsShellDebugger)) as IDebuggerInternal11;
            if (debuggerServiceInternal_ == null)
            {
                throw new Exception("AsDebugger : Could not get SVsShellDebugger service");
            }

            dte_ = (DTE)Package.GetGlobalService(typeof(DTE));
            if (dte_ == null)
            {
                throw new Exception("AsDebugger : Could not get DTE service");
            }
        }

        private FRAMEINFO[] GetCallStackInternal()
        {
            int hr = VSConstants.S_OK;

            IDebugThread2 thread = debuggerServiceInternal_.CurrentThread;
            if (thread == null)
            {
                throw new Exception("AsDebugger : Could not get CurrentThread");
            }

            IEnumDebugFrameInfo2 enumDebugFrameInfo2;
            hr = thread.EnumFrameInfo(
                enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO | 
                enum_FRAMEINFO_FLAGS.FIF_MODULE |
                enum_FRAMEINFO_FLAGS.FIF_FRAME |
                enum_FRAMEINFO_FLAGS.FIF_FUNCNAME,
                0,
                out enumDebugFrameInfo2);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsDebugger : Could not enumerate stack frames.");
            }

            enumDebugFrameInfo2.Reset();

            uint pcelt = 0;
            enumDebugFrameInfo2.GetCount(out pcelt);
            FRAMEINFO[] frameInfo = new FRAMEINFO[pcelt];
            uint fetched = 0;
            hr = enumDebugFrameInfo2.Next(pcelt, frameInfo, ref fetched);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsDebugger : Could not get FRAMEINFO");
            }

            return frameInfo;
        }
        #endregion Private Methods
    }
}
