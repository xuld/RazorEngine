using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuld.RazorEngine {

    /// <summary>
    /// 表示模板在执行时使用的上下文对象。
    /// </summary>
    public class TemplateContext {

        /// <summary>
        /// 获取或设置用于获取模板内容的 <see cref="TextReader"/> 对象。
        /// </summary>
        public TextReader InputReader {
            get;
            set;
        }

        /// <summary>
        /// 模板的文件名或模板的别名，用于在模板互相引用时使用。模板的名字作为模板的唯一标识符，应该保证其唯一性。默认为模板文件的路径或模板字符串本身的哈希值。
        /// </summary>
        public string SourceFileName {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置项目跟地址（~）代表的实际物理位置。
        /// </summary>
        public string AppPhysicalPath {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置解析文件路径时使用的虚拟文件夹地址。
        /// </summary>
        public string AppVirtualPath {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置输出的 TextWriter 对象。
        /// </summary>
        public TextWriter OutputWriter {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置解析时使用的模型对象。
        /// </summary>
        public object Model {
            get;
            set;
        }

        dynamic _viewBag;

        /// <summary>
        /// 获取或设置解析使用的临时变量。
        /// </summary>
        public dynamic ViewBag {
            get {
                if (_viewBag == null) {
                    _viewBag = new System.Dynamic.ExpandoObject();
                }

                return _viewBag;
            }
            set {
                _viewBag = value;
            }
        }

        internal Dictionary<string, SectionWriter> Sections;

        /// <summary>
        /// 获取或设置被引用的 Razor 对象。
        /// </summary>
        public Razor Razor {
            get;
            internal set;
        }

        /// <summary>
        /// 获取或设置默认编码对象。
        /// </summary>
        public Encoding DefaultEncoding {
            get;
            set;
        }

        public TemplateContext(TextReader reader, string sourceFileName, TextWriter writer)
            : this(reader, sourceFileName, writer, Path.GetDirectoryName(sourceFileName), "/") {
        }

        public TemplateContext(TextReader reader, string sourceFileName, TextWriter writer, string appPath, string virtualPath) {
            InputReader = reader;
            OutputWriter = writer;
            SourceFileName = sourceFileName;
            AppPhysicalPath = appPath;
            AppVirtualPath = virtualPath;
            DefaultEncoding = Encoding.UTF8;
        }
    }
}
