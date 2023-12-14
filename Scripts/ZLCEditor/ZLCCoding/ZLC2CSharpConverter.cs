using ZLCEditor.FormatSystem;
using ZLCEditor.FormatSystem.Common;
namespace ZLCEditor.ZLCCoding
{
    /// <summary>
    ///     ZLC格式文件转换为C#文件的转换器
    /// </summary>
    public class ZLC2CSharpConverter : IFormatConverter<ZLCCode, CSharpCode>
    {
        /// <inheritdoc />
        public CSharpCode Convert(ZLCCode from)
        {
            ZLCParse parse = new ZLCParse();
            return new CSharpCode
            {
                code = parse.Parse(from)
            };
        }
    }
}