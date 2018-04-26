using System.Text;

namespace PanGu.Dict
{
    /// <summary>
    /// 简繁转换字典,源码来自 https://github.com/toolgood/ToolGood.Words
    /// </summary>
    public class WordHelper
    {
        /// <summary>转换成简体
        /// </summary>
        public static string ToSimplifiedChinese(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c >= 0x4e00 && c <= 0x9fa5)
                {
                    var k = WordDict.Simplified[c - 0x4e00];
                    if (k != c)
                    {
                        sb[i] = k;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
