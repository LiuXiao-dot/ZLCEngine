namespace ZLCEngine.Exceptions
{
    /// <summary>
    /// 代表出现了与程序意图相违背的错误
    /// </summary>
    public class UnexpectedException : ZLCException
    {
        private string _condition;
        private string _expected;
        private string _unexpected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="expected">程序意图</param>
        /// <param name="unexpected">结果</param>
        public UnexpectedException(string condition, string expected, string unexpected)
        {
            this._condition = condition;
            this._expected = expected;
            this._unexpected = unexpected;
        }


        protected override string GetExceptionMessage()
        {
            return $"在{this._condition}时,期望{this._expected},但是{this._unexpected}";
        }
    }
}