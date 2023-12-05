using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace ZLCEditor.ZLCCoding
{
    /// <summary>
    /// ZLC格式文件的解析器
    /// </summary>
    public class ZLCParse
    {
        private Regex ParameterRegex = new Regex(@"\$(.+?)\$"); // 1:参数名
        private Regex forRegex = new Regex(@"\#for\s*?\$(.*?)\$\s*?#\s*?\r\n(.*?)\r\n\s*?\#end"); // 1:参数名 2:代码语句
        private Regex ifRegex = new Regex(@"\#if\s*?\$(.*?)\$\s*?#\s*?\r\n(.*?)\r\n\s*?\#end");

        private Dictionary<string, object> kvs;
        public string Parse(ZLCCode zlcCode)
        {
            this.kvs = zlcCode.kvs;
            return ParseCode(zlcCode.code);
        }

        string ParseCode(string input)
        {
            string code = input;
            // 使用正则表达式解析变量和控制语句
            code = ParseFor(code);
            code = ParseIf(code);
            code = ParseParameter(code);

            return code;
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        private string ParseParameter(string code)
        {
            foreach (Match match in ParameterRegex.Matches(code)) {
                if (kvs.ContainsKey(match.Groups[1].Value))
                    code = code.Replace(match.Groups[0].Value, kvs[match.Groups[1].Value].ToString());
                else {
                    Debug.LogError($"未设置{match.Groups[1].Value}的值");
                }
            }
            return code;
        }

        /// <summary>
        /// 提供参数和值的情况下解析代码
        /// </summary>
        /// <returns></returns>
        private string ParseParameterDirectly(string code, string paramter, string value)
        {
            return code.Replace($"${paramter}$", value);
        }

        /// <summary>
        /// 解析for语句块
        /// 默认会使用数组的每个数据生成一条语句，并使用数组的数据替换与for参数相同的参数
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string ParseFor(string code)
        {
            var resultCode = code;
            foreach (Match match in forRegex.Matches(code)) {
                var tempResultCode = "";
                var forCode = match.Groups[0].Value;
                var forParameterCode = match.Groups[1].Value;
                var contentCode = match.Groups[2].Value;
                var parameter = GetRealParameter(forParameterCode);
                if (parameter is string[] values) {
                    if (values.Length > 0) {
                        foreach (var value in values) {
                            var tempCode = ParseParameterDirectly(contentCode, forParameterCode, value);
                            tempResultCode += ParseParameter(tempCode);
                            tempResultCode += "\r\n";
                        }
                        tempResultCode = tempResultCode.Substring(0, tempResultCode.Length - 2);
                    }
                    resultCode = resultCode.Replace(forCode, tempResultCode);
                } else {
                    Debug.LogError($"未设置循环语句的参数:{forParameterCode}");
                }
            }
            return resultCode;
        }

        /// <summary> 
        /// 解析if语句块
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string ParseIf(string code)
        {
            var resultCode = string.Empty;
            foreach (Match match in ifRegex.Matches(code)) {
                var ifCode = match.Groups[0].Value;
                var ifParameterCode = match.Groups[1].Value;
                var contentCode = match.Groups[2].Value;
                var parameter = GetRealParameter(ifParameterCode);
                if ((bool)parameter) {
                    resultCode += ParseParameter(contentCode);
                    resultCode = code.Replace(ifCode, resultCode);
                } else {
                    Debug.LogWarning($"未设置循环语句的参数:{ifParameterCode}");
                }
            }
            return resultCode == string.Empty ? code : resultCode;
        }

        /// <summary>
        /// 获取实际的参数
        /// 如：
        ///     for语句块的循环参数
        ///     if语句块的条件参数
        /// </summary>
        /// <returns></returns>
        private object GetRealParameter(string key)
        {
            if (kvs.ContainsKey(key)) {
                return kvs[key];
            } else {
                return default;
            }
        }
    }
}