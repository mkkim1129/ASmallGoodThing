using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using System;

namespace AsDebuggerExtension
{
    public class AsStackFrame
    {
        #region Private Fileds
        private FRAMEINFO frameInfo_;
        #endregion Private Fileds
        
        #region Public Methods
        public AsStackFrame(FRAMEINFO frameInfo)
        {
            frameInfo_ = frameInfo;
        }

        public AsVariable EvaluateExpression(string expression)
        {
            int hr = VSConstants.S_OK;
            
            IDebugExpressionContext2 debugExpressionContext;
            hr = frameInfo_.m_pFrame.GetExpressionContext(out debugExpressionContext);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsStackFrame : Failed to get expression context.");
            }

            IDebugExpression2 debugExpression;
            string error;
            uint errorCharIndex;
            hr = debugExpressionContext.ParseText(
                expression,
                enum_PARSEFLAGS.PARSE_EXPRESSION,
                10,
                out debugExpression,
                out error,
                out errorCharIndex);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsStackFrame : Failed to parse expression.");
            }

            IDebugProperty2 debugProperty2 = null;
            hr = debugExpression.EvaluateSync(
                enum_EVALFLAGS.EVAL_NOSIDEEFFECTS,
                unchecked((uint)System.Threading.Timeout.Infinite),
                null,
                out debugProperty2);
            if (hr != VSConstants.S_OK)
            {
                throw new Exception("AsStackFrame : Failed to evaluate expression.");
            }

            IDebugProperty3 debugProperty3 = debugProperty2 as IDebugProperty3;
            if (debugProperty3 == null)
            {
                throw new Exception("AsStackFrame : Failed to cast IDebugProperty3");
            }

            return (new AsVariable(debugProperty3));
        }

        public override string ToString()
        {
            string result = base.ToString() + Environment.NewLine;
            result += "\tFuncName : " + frameInfo_.m_bstrFuncName;
            return result;
        }
        #endregion Public Methods
    }
}
