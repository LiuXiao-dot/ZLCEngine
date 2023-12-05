namespace ZLCEditor.FormatSystem
{
    /// <summary>
    /// 将F类型转换为T类型数据的转换器
    /// </summary>
    public interface IFormatConverter<in F, out T>
    {
        /// <summary>
        /// 将F类型的数据转换为T类型的数据
        /// </summary>
        /// <param name="from"><typeparamref name="F"/>类型的数据</typeparam></param>
        /// <returns></returns>
        T Convert(F from);
    }
}