
using ST.Procesess.Models;
using System;

namespace ST.Procesess
{
    public static class XElementType
    {
        public static XString Process = "process";
        public static XString Lane = "lane";
        public static XString SequenceFlow = "sequenceFlow";
        public static XString StartEvent = "startEvent";
        public static XString EndEvent = "endEvent";
        public static XString IntermediateThrowEvent = "intermediateThrowEvent";
        public static XString SendTask = "sendTask";
        public static XString ReceiveTask = "receiveTask";
        public static XString UserTask = "userTask";
        public static XString ManualTask = "manualTask";
        public static XString BusinessRuleTask = "businessRuleTask";
        public static XString ServiceTask = "serviceTask";
        public static XString ScriptTask = "scriptTask";
        public static XString CallActivity = "callActivity";
        public static XString SubProcess = "subProcess";
        public static XString Transaction = "transaction";
        public static XString DataStoreReference = "dataStoreReference";
        public static XString Participant = "participant";
        public static XString Collaboration = "collaboration";
    }

    public static class XStringExtension
    {
        /// <summary>
        /// To upper first char of XString
        /// </summary>
        /// <param name="xString"></param>
        /// <returns></returns>
        public static XString ToUpperFirstString(this XString xString)
        {
            var str = xString.ToString();
            return $"{str[0].ToString().ToUpper()}{str.Substring(1, str.Length - 1)}";
        }

        public static string ToUpperFirstString(this string xString)
        {
            var str = xString.ToString();
            return $"{str[0].ToString().ToUpper()}{str.Substring(1, str.Length - 1)}";
        }

        /// <summary>
        /// Get transition type from XString
        /// </summary>
        /// <param name="xString"></param>
        /// <returns></returns>
        public static TransitionType GetTransitionType(this XString xString)
        {
            try
            {
                return Enum.Parse<TransitionType>(xString.ToString());
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Get transition type from string value of XString
        /// </summary>
        /// <param name="xString"></param>
        /// <returns></returns>
        public static TransitionType GetTransitionType(this string xString)
        {
            return new XString(xString).GetTransitionType();
        }
    }

    public class XString
    {
        readonly string _value;
        public XString(string value)
        {
            this._value = value;
        }
        public static implicit operator string(XString d)
        {
            return d._value;
        }
        public static implicit operator XString(string d)
        {
            return new XString(d);
        }

        /// <summary>
        /// Get Value as string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }
    }
}
