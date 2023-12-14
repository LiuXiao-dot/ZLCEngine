namespace ZLCEditor.FormatSystem
{
    /// <summary>
    ///     转换器管理器
    /// </summary>
    public class FormatManager
    {
        /// <summary>
        ///     将F数据转换为T数据
        /// </summary>
        /// <param name="from">源数据</param>
        /// <typeparam name="F">源数据类型</typeparam>
        /// <typeparam name="T">目标数据类型</typeparam>
        /// <returns>目标数据</returns>
        public static T Convert<F, T>(F from)
        {
            using (FormaterFactory factory = new FormaterFactory()) {
                IFormatConverter<F, T> converter = factory.GetConverter<F, T>();
                return converter.Convert(from);
            }
        }
    }
}