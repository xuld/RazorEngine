using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Xuld.RazorEngine {

    /// <summary>
    /// 编译后的模板的默认基类。
    /// </summary>
    /// <typeparam name="T">Model 属性的类型。</typeparam>
    public abstract class TemplateBase<T> : ITemplateBase {

        /// <summary>
        /// 获取或设置当前的运行上下文。
        /// </summary>
        public TemplateContext CurrentContext {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前的模型对象。
        /// </summary>
        public T Model {
            get;
            set;
        }

        dynamic ITemplateBase.Model {
            get {
                return Model;
            }
            set {
                Model = (T)value;
            }
        }

        /// <summary>
        /// 获取当前的自定义存储对象。
        /// </summary>
        public dynamic ViewBag {
            get {
                return CurrentContext.ViewBag;
            }
        }

        /// <summary>获取页面当前的 <see cref="T:System.IO.TextWriter" /> 对象。</summary>
        /// <returns>
        /// <see cref="T:System.IO.TextWriter" /> 对象。</returns>
        public TextWriter Output {
            get {
                return CurrentContext.OutputWriter;
            }
        }

        /// <summary>获取与页关联的 <see cref="T:System.Web.WebPages.Html.HtmlHelper" /> 对象。</summary>
        /// <returns>可以在页面中呈现 HTML 窗体控件的对象。</returns>
        public HtmlHelper Html {
            get {
                return HtmlHelper.Instance;
            }
        }

        /// <summary>
        /// 在执行之前执行初始化操作。
        /// </summary>
        /// <param name="model">要初始化的模型对象。</param>
        /// <returns>返回初始化后的模型对象。</returns>
        public virtual void Init(object model) {
            Model = (T)model;
        }

        /// <summary>
        /// 执行当前的模板。
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// 在指定的作用域内执行当前的模板。
        /// </summary>
        public void Execute(TemplateContext context) {
            CurrentContext = context;
            try {
                Init(context.Model);
                Execute();
            } finally {
                CurrentContext = null;
            }
        }

        /// <summary>将指定的对象作为 HTML 编码的字符串写入。</summary>
        /// <param name="value">要编码并写入的对象。</param>
        public void Write(object value) {
            if (value != null)
                WriteTo(CurrentContext.OutputWriter, value.ToString());
        }

        /// <summary>将指定的对象作为 HTML 编码的字符串写入。</summary>
        /// <param name="value">要编码并写入的对象。</param>
        public void Write(Action<TextWriter> value) {
            if (value != null)
                value(CurrentContext.OutputWriter);
        }

        /// <summary>将指定的对象作为 HTML 编码的字符串写入。</summary>
        /// <param name="value">要编码并写入的对象。</param>
        public void Write(HtmlString value) {
            if (value != null)
                WriteLiteralTo(CurrentContext.OutputWriter, value.ToString());
        }

        /// <summary>将指定的对象作为 HTML 编码的字符串写入。</summary>
        /// <param name="value">要编码并写入的对象。</param>
        public void Write(string value) {
            WriteTo(CurrentContext.OutputWriter, System.Web.HttpUtility.HtmlEncode(value));
        }

        /// <summary>无需先对指定的对象进行 HTML 编码，即可将其写入。</summary>
        /// <param name="value">要写入的对象。</param>
        public void WriteLiteral(object value) {
            if (value != null)
                WriteLiteralTo(CurrentContext.OutputWriter, value);
        }

        /// <summary>无需先对指定的对象进行 HTML 编码，即可将其写入。</summary>
        /// <param name="value">要写入的对象。</param>
        public void WriteLiteral(string value) {
            WriteLiteralTo(CurrentContext.OutputWriter, value);
        }

        /// <summary>将指定的对象作为 HTML 编码的字符串写入指定的文本编写器。</summary>
        /// <param name="writer">文本编写器。</param>
        /// <param name="content">要编码并写入的对象。</param>
        public static void WriteTo(TextWriter writer, object content) {
            writer.Write(content);
        }

        /// <summary>将指定的对象作为 HTML 编码的字符串写入指定的文本编写器。</summary>
        /// <param name="writer">文本编写器。</param>
        /// <param name="content">要编码并写入的对象。</param>
        public static void WriteTo(TextWriter writer, Action<TextWriter> content) {
            content(writer);
        }

        /// <summary>将指定的对象作为 HTML 编码的字符串写入指定的文本编写器。</summary>
        /// <param name="writer">文本编写器。</param>
        /// <param name="content">要编码并写入的对象。</param>
        public static void WriteTo(TextWriter writer, string content) {
            writer.Write(content);
        }

        /// <summary>无需 HTML 编码即可将指定的对象写入指定的 <see cref="T:System.IO.TextWriter" /> 实例。</summary>
        /// <param name="writer">文本编写器。</param>
        /// <param name="content">要写入的对象。</param>
        public static void WriteLiteralTo(TextWriter writer, object content) {
            writer.Write(content);
        }

        /// <summary>无需 HTML 编码即可将指定的对象写入指定的 <see cref="T:System.IO.TextWriter" /> 实例。</summary>
        /// <param name="writer">文本编写器。</param>
        /// <param name="content">要写入的对象。</param>
        public static void WriteLiteralTo(TextWriter writer, string content) {
            writer.Write(content);
        }

        /// <summary>将指定的对象作为 HTML 属性写入指定的文本编写器。</summary>
        /// <param name="value">要写入的属性名。</param>
        /// <param name="value">要写入的属性值。</param>
        public void WriteAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values) {
            WriteAttributeTo(CurrentContext.OutputWriter, name, prefix, suffix, values);
        }

        /// <summary>
        /// 将指定的对象作为 HTML 属性写入指定的写入指定的 <see cref="T:System.IO.TextWriter" /> 实例。
        /// </summary>
        /// <param name="writer">文本编写器。</param>
        /// <param name="value">要写入的属性名。</param>
        /// <param name="value">要写入的属性值。</param>
        public static void WriteAttributeTo(TextWriter writer, string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values) {
            bool first = true;
            bool wroteSomething = false;
            if (values.Length == 0) {
                // Explicitly empty attribute, so write the prefix and suffix
                WriteLiteralTo(writer, prefix.Item1);
                WriteLiteralTo(writer, suffix.Item1);
            } else {
                for (int i = 0; i < values.Length; i++) {
                    AttributeValue attrVal = values[i];
                    Tuple<object, int> val = attrVal.Value;

                    bool? boolVal = null;
                    if (val.Item1 is bool) {
                        boolVal = (bool)val.Item1;
                    }

                    if (val.Item1 != null && (boolVal == null || boolVal.Value)) {
                        string valStr = val.Item1 as string;
                        if (valStr == null) {
                            valStr = val.Item1.ToString();
                        }
                        if (boolVal != null) {
                            valStr = name;
                        }

                        if (first) {
                            WriteLiteralTo(writer, prefix.Item1);
                            first = false;
                        } else {
                            WriteLiteralTo(writer, attrVal.Prefix.Item1);
                        }

                        if (attrVal.Literal) {
                            WriteLiteralTo(writer, valStr);
                        } else {
                            WriteTo(writer, valStr); // Write value
                        }
                        wroteSomething = true;
                    }
                }
                if (wroteSomething) {
                    WriteLiteralTo(writer, suffix.Item1);
                }
            }
        }

        /// <summary>由内容页调用以创建指定的内容部分。</summary>
        /// <param name="name">要创建的部分的名称。</param>
        /// <param name="action">在新部分中要执行的操作的类型。</param>
        public void DefineSection(string name, SectionWriter action) {
            if (CurrentContext.Sections == null) {
                CurrentContext.Sections = new Dictionary<string, SectionWriter>();
            }

            CurrentContext.Sections[name] = action;
        }

        /// <summary>在布局页中，将呈现指定部分的内容并指定该部分是否为必需。</summary>
        /// <returns>要呈现的 HTML 内容。</returns>
        /// <param name="name">要呈现的部分。</param>
        /// <param name="required">要指定该部分为必需，则为 true；否则为 false。</param>
        /// <exception cref="T:System.Web.HttpException">
        /// <paramref name="name" /> 部分已呈现。- 或 -<paramref name="name" /> 部分已标记为必需，但却找不到。</exception>
        public string RenderSection(string name, bool required = true) {

            SectionWriter writer;
            if (CurrentContext.Sections != null && CurrentContext.Sections.TryGetValue(name, out writer)) {
                writer();
            } else if (required) {
                throw new System.Web.HttpException(string.Format("Section not defined: \"{0}\"", name));
            }
            return null;
        }

        /// <summary>从指定的路径返回规范化路径。</summary>
        /// <returns>规范化路径。</returns>
        /// <param name="path">要规范化的路径。</param>
        public string NormalizePath(string path) {
            return UrlUtil.Combine(VirtualPath, path);
        }

        /// <summary>使用指定的参数，从应用程序相对 URL 构建绝对 URL。</summary>
        /// <returns>绝对 URL。</returns>
        /// <param name="path">要在 URL 中使用的初始路径。</param>
        /// <param name="pathParts">附加路径信息，例如文件夹和子文件夹。</param>
        public string Href(string path, params object[] pathParts) {
            return UrlUtil.BuildUrl(NormalizePath(path), pathParts);
        }

        /// <summary>获取页的虚拟路径。</summary>
        /// <returns>虚拟路径。</returns>
        public string VirtualPath {
            get {
                return UrlUtil.Combine(CurrentContext.AppVirtualPath, UrlUtil.Relative(CurrentContext.AppPhysicalPath, CurrentContext.SourceFileName));
            }
        }

        /// <summary>呈现内容页。</summary>
        /// <returns>一个可以写入页的输出的对象。</returns>
        /// <param name="path">要呈现的页的路径。</param>
        /// <param name="model">要传递给页的数据。</param>
        public string RenderPage(string path) {

            string oldSourceFileName = CurrentContext.SourceFileName;
            TextReader reader = CurrentContext.InputReader;

            // 解析相对路径返回绝对物理路径。
            CurrentContext.SourceFileName = UrlUtil.Resolve(CurrentContext.SourceFileName, path);
            CurrentContext.InputReader = new StreamReader(CurrentContext.SourceFileName, CurrentContext.DefaultEncoding);

            try {
                CurrentContext.Razor.Parse(CurrentContext);
            } finally {
                CurrentContext.InputReader.Close();
                CurrentContext.SourceFileName = oldSourceFileName;
                CurrentContext.InputReader = reader;
            }

            return null;
        }

        /// <summary>呈现内容页。</summary>
        /// <returns>一个可以写入页的输出的对象。</returns>
        /// <param name="path">要呈现的页的路径。</param>
        /// <param name="model">要传递给页的数据。</param>
        public string RenderPage(string path, object model) {

            string oldSourceFileName = CurrentContext.SourceFileName;
            TextReader reader = CurrentContext.InputReader;
            object oldModel = CurrentContext.Model;

            // 解析相对路径返回绝对物理路径。
            CurrentContext.SourceFileName = UrlUtil.Resolve(CurrentContext.SourceFileName, path);
            CurrentContext.Model = model;
            CurrentContext.InputReader = new StreamReader(CurrentContext.SourceFileName, CurrentContext.DefaultEncoding);

            try {
                CurrentContext.Razor.Parse(CurrentContext);
            } finally {
                CurrentContext.InputReader.Close();
                CurrentContext.SourceFileName = oldSourceFileName;
                CurrentContext.InputReader = reader;
                CurrentContext.Model = oldModel;
            }

            return null;
        }

        /// <summary>
        /// 将一个相对的路径地址改成基于网站虚拟路径的绝对地址。
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public string ResolveUrl(string relativeUrl) {
            return UrlUtil.Resolve(VirtualPath, relativeUrl);
        }

    }

    /// <summary>提供了用于表示写入内容部分时调用的一个或多个方法的委托。</summary>
    public delegate void SectionWriter();

    public abstract class DynamicTemplateBase : TemplateBase<dynamic>, ITemplateBase {

        class RazorDynamicObject : System.Dynamic.DynamicObject {

            readonly object _model;

            public RazorDynamicObject(object model) {
                _model = model;
            }

            /// <summary>
            /// Gets the value of the specified member.
            /// </summary>
            /// <param name="binder">The current binder.</param>
            /// <param name="result">The member result.</param>
            /// <returns>True.</returns>
            [System.Diagnostics.DebuggerStepThrough]
            public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result) {
                if (binder == null)
                    throw new ArgumentNullException("binder");

                var dynamicObject = _model as RazorDynamicObject;
                if (dynamicObject != null)
                    return dynamicObject.TryGetMember(binder, out result);

                Type modelType = _model.GetType();
                var prop = modelType.GetProperty(binder.Name);
                if (prop == null) {
                    result = null;
                    return false;
                }

                object value = prop.GetValue(_model, null);
                if (value == null) {
                    result = value;
                    return true;
                }

                Type valueType = value.GetType();
                result = (TypeHelper.IsAnonymousType(valueType))
                             ? new RazorDynamicObject(value)
                             : value;
                return true;
            }

        }

        public override void Init(object model) {
            Model = new RazorDynamicObject(model);
        }

    }

    /// <summary>
    /// 表示默认的模板对象应该实现的接口。
    /// </summary>
    public interface ITemplateBase {

        /// <summary>
        /// 获取或设置当前模板的模型对象。
        /// </summary>
        dynamic Model {
            get;
            set;
        }

        /// <summary>
        /// 获取当前的自定义存储对象。
        /// </summary>
        dynamic ViewBag {
            get;
        }

        /// <summary>获取页面当前的 <see cref="T:System.IO.TextWriter" /> 对象。</summary>
        /// <returns>
        /// <see cref="T:System.IO.TextWriter" /> 对象。</returns>
        TextWriter Output {
            get;
        }

        /// <summary>获取与页关联的 <see cref="T:System.Web.WebPages.Html.HtmlHelper" /> 对象。</summary>
        /// <returns>可以在页面中呈现 HTML 窗体控件的对象。</returns>
        HtmlHelper Html {
            get;
        }

        /// <summary>
        /// 执行当前的模板。
        /// </summary>
        void Execute();

        /// <summary>
        /// 在指定的作用域内执行当前的模板。
        /// </summary>
        /// <param name="context">执行时使用的作用域。</param>
        /// <param name="model">执行使用的模型对象。</param>
        void Execute(TemplateContext context);

        /// <summary>将指定的对象作为 HTML 编码的字符串写入。</summary>
        /// <param name="value">要编码并写入的对象。</param>
        void Write(object value);

        /// <summary>将指定的对象作为 HTML 编码的字符串写入。</summary>
        /// <param name="value">要编码并写入的对象。</param>
        void Write(Action<TextWriter> value);

        /// <summary>将指定的对象作为 HTML 编码的字符串写入。</summary>
        /// <param name="value">要编码并写入的对象。</param>
        void Write(HtmlString value);

        /// <summary>将指定的对象作为 HTML 编码的字符串写入。</summary>
        /// <param name="value">要编码并写入的对象。</param>
        void Write(string value);

        /// <summary>无需先对指定的对象进行 HTML 编码，即可将其写入。</summary>
        /// <param name="value">要写入的对象。</param>
        void WriteLiteral(object value);

        /// <summary>无需先对指定的对象进行 HTML 编码，即可将其写入。</summary>
        /// <param name="value">要写入的对象。</param>
        void WriteLiteral(string value);

        /// <summary>将指定的对象作为 HTML 属性写入指定的文本编写器。</summary>
        /// <param name="value">要写入的属性名。</param>
        /// <param name="value">要写入的属性值。</param>
        void WriteAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values);

        /// <summary>由内容页调用以创建指定的内容部分。</summary>
        /// <param name="name">要创建的部分的名称。</param>
        /// <param name="action">在新部分中要执行的操作的类型。</param>
        void DefineSection(string name, SectionWriter action);

        /// <summary>在布局页中，将呈现指定部分的内容并指定该部分是否为必需。</summary>
        /// <returns>要呈现的 HTML 内容。</returns>
        /// <param name="name">要呈现的部分。</param>
        /// <param name="required">要指定该部分为必需，则为 true；否则为 false。</param>
        /// <exception cref="T:System.Web.HttpException">
        /// <paramref name="name" /> 部分已呈现。- 或 -<paramref name="name" /> 部分已标记为必需，但却找不到。</exception>
        string RenderSection(string name, bool required = true);

        /// <summary>从指定的路径返回规范化路径。</summary>
        /// <returns>规范化路径。</returns>
        /// <param name="path">要规范化的路径。</param>
        string NormalizePath(string path);

        /// <summary>使用指定的参数，从应用程序相对 URL 构建绝对 URL。</summary>
        /// <returns>绝对 URL。</returns>
        /// <param name="path">要在 URL 中使用的初始路径。</param>
        /// <param name="pathParts">附加路径信息，例如文件夹和子文件夹。</param>
        string Href(string path, params object[] pathParts);

        /// <summary>获取页的虚拟路径。</summary>
        /// <returns>虚拟路径。</returns>
        string VirtualPath {
            get;
        }

        /// <summary>呈现内容页。</summary>
        /// <returns>一个可以写入页的输出的对象。</returns>
        /// <param name="path">要呈现的页的路径。</param>
        /// <param name="model">要传递给页的数据。</param>
        string RenderPage(string path);

        /// <summary>呈现内容页。</summary>
        /// <returns>一个可以写入页的输出的对象。</returns>
        /// <param name="path">要呈现的页的路径。</param>
        /// <param name="model">要传递给页的数据。</param>
        string RenderPage(string path, object model);

        /// <summary>
        /// 将一个相对的路径地址改成基于网站虚拟路径的绝对地址。
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        string ResolveUrl(string relativeUrl);


    }

}
