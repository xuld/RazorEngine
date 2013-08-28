using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Xuld.RazorEngine {

    /// <summary>
    /// 为在网页中呈现 HTML 窗体控件和执行窗体验证提供支持。
    /// </summary>
    public class HtmlHelper {
        
        /// <summary>通过使用最小编码，返回表示指定对象的 HTML 编码的字符串，该最小编码仅适用于由引号引起来的 HTML 特性。</summary>
        /// <returns>表示该对象的 HTML 编码的字符串。</returns>
        /// <param name="value">要编码的对象。</param>
        public string AttributeEncode(object value) {
            return AttributeEncode(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        /// <summary>通过使用最小编码，返回表示指定字符串的 HTML 编码的字符串，该最小编码仅适用于由引号引起来的 HTML 特性。</summary>
        /// <returns>表示原始字符串的 HTML 编码的字符串。</returns>
        /// <param name="value">要编码的字符串。</param>
        public string AttributeEncode(string value) {
            if (string.IsNullOrEmpty(value)) {
                return string.Empty;
            }
            return HttpUtility.HtmlAttributeEncode(value);
        }

        /// <summary>通过使用适用于任意 HTML 的完整编码返回一个表示指定对象的 HTML 编码的字符串。</summary>
        /// <returns>表示该对象的 HTML 编码的字符串。</returns>
        /// <param name="value">要编码的对象。</param>
        public string Encode(object value) {
            return this.Encode(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        /// <summary>通过使用适用于任意 HTML 的完整编码返回一个表示指定字符串的 HTML 编码的字符串。</summary>
        /// <returns>表示原始字符串的 HTML 编码的字符串。</returns>
        /// <param name="value">要编码的字符串。</param>
        public string Encode(string value) {
            if (string.IsNullOrEmpty(value)) {
                return string.Empty;
            }
            return HttpUtility.HtmlEncode(value);
        }

        /// <summary>包装 <see cref="T:System.Web.HtmlString" /> 实例中的 HTML 标记，以便将其解释为 HTML 标记。</summary>
        /// <returns>未编码的 HTML。</returns>
        /// <param name="value">要呈现其 HTML 的对象。</param>
        public Action<TextWriter> Raw(object value) {
            return (writer) => {
                writer.Write(value);
            };
        }

        /// <summary>包装 <see cref="T:System.Web.HtmlString" /> 实例中的 HTML 标记，以便将其解释为 HTML 标记。</summary>
        /// <returns>未编码的 HTML。</returns>
        /// <param name="value">要解释为 HTML 标记而不是进行 HTML 编码的字符串。</param>
        public HtmlString Raw(string value) {
            return new HtmlString(value);
        }

        internal readonly static HtmlHelper Instance = new HtmlHelper();

    }


}
