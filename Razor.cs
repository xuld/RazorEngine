//#define PARSER_DEBUG
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Razor;

namespace Xuld.RazorEngine {

    /// <summary>
    /// 表示一个 Razor 模板解析器。
    /// </summary>
    public class Razor :IDisposable {

        /// <summary>
        /// 存储已编译的模板类型。
        /// </summary>
        /// <remarks>
        /// sourceFileName => [TemplateType, Model]
        /// </remarks>
        Dictionary<string, Tuple<Type, Type>> _templateTypeCache = new Dictionary<string, Tuple<Type, Type>>();

        CompilerParameters _compilerParameters;

        /// <summary>
        /// 获取或设置内部用于解析 Razor 模板的 RazorTemplateEngine 的对象。
        /// </summary>
        public RazorTemplateEngine RazorTemplateEngine {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置内部用于编译代码的 CodeDomProvider 对象。
        /// </summary>
        public CodeDomProvider CodeDomProvider {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置编译 Razor 文件使用的参数。
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public CompilerParameters CompilerParameters {
            get {
                if (_compilerParameters.ReferencedAssemblies.Count == 0) {

                    foreach (string assembly in AppDomain.CurrentDomain.GetAssemblies().TakeWhile(a => !a.IsDynamic).Select(a => a.Location).Concat(new string[1] { typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly.Location }).Distinct()) {
                        _compilerParameters.ReferencedAssemblies.Add(assembly);
                    }


                    AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

                }

                return _compilerParameters;
            }
            set {
                _compilerParameters = value;
            }
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args) {
            if (!args.LoadedAssembly.IsDynamic) {
                _compilerParameters.ReferencedAssemblies.Add(args.LoadedAssembly.Location);
            }
        }
        
        /// <summary>
        /// 获取当前项目所引用的程序集。
        /// </summary>
        /// <value>一个集合，包含由源引用以进行编译的程序集名称。</value>
        public System.Collections.Specialized.StringCollection ReferencedAssemblies {
            get {
                return CompilerParameters.ReferencedAssemblies;
            }
        }

        /// <summary>
        /// 获取当前默认使用的全局名字空间列表。
        /// </summary>
        public ISet<string> NamespaceImports {
            get {
                return RazorTemplateEngine.Host.NamespaceImports;
            }
        }

        /// <summary>
        /// 获取或设置编译的模板基类。为了确保它可以正常工作，它应该实现 <see cref="ITemplateBase"/> 接口。
        /// </summary>
        public string DefaultBaseClassName {
            get;
            set;
        }

        /// <summary>
        /// 初始化 <see cref="Razor"/> 对象的新实例。
        /// </summary>
        public Razor() {
            RazorTemplateEngine = new RazorTemplateEngine(new System.Web.Razor.RazorEngineHost(new CSharpRazorCodeLanguage()) {
                GeneratedClassContext = new System.Web.Razor.Generator.GeneratedClassContext(
                    System.Web.Razor.Generator.GeneratedClassContext.DefaultExecuteMethodName,
                    System.Web.Razor.Generator.GeneratedClassContext.DefaultWriteMethodName,
                    System.Web.Razor.Generator.GeneratedClassContext.DefaultWriteLiteralMethodName,
                    "WriteTo", "WriteLiteralTo", "System.Action<System.IO.TextWriter>", "DefineSection") {
                        ResolveUrlMethodName = "ResolveUrl"
                    },
                DefaultBaseClass = "Xuld.RazorEngine.TemplateBase<object>"
            });
            NamespaceImports.Add("System");
            NamespaceImports.Add("System.Collections.Generic");
            NamespaceImports.Add("System.Linq");
            NamespaceImports.Add("Microsoft.CSharp");

            CodeDomProvider = new CSharpCodeProvider();

            _compilerParameters = new CompilerParameters {
                GenerateInMemory = true,
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                CompilerOptions = "/target:library /optimize"
            };
        }

        /// <summary>
        /// 创建指定路径的模板解析后的模板类型。
        /// </summary>
        /// <param name="sourceFileName">模板的文件名或模板的别名，用于在模板互相引用时使用。模板的名字作为模板的唯一标识符，应该保证其唯一性。默认为模板文件的路径或模板字符串本身的哈希值。</param>
        /// <param name="templateReader">用于获取模板内容的 <see cref="TextReader"/> 对象。</param>
        /// <param name="className">模板编译后的类名。默认为根据 <paramref name="sourceFileName"/> 动态生成。</param>
        /// <param name="rootNamespace">模板编译后的名字空间。默认为 RazorCompiledTemplates 。</param>
        /// <param name="baseClassName">模板编译后的默认基类。如果值为 null，则使用 <see cref="TemplateBase&lt;T&gt;"/> 作为模板基类。在模板中可以通过 @inherits 关键字来覆盖这里的设置。</param>
        /// <param name="modelType">模板编译后的模型类型。如果值为 null，则表示不使用模板对象。未来版本中，在模板中可以通过 @model 关键字来覆盖这里的设置。</param>
        /// <returns>一个 <see cref="GeneratorResults"/> 对象。</returns>
        public GeneratorResults GenerateCode(TextReader templateReader, string className, string rootNamespace, string sourceFileName, string baseClassName, Type modelType) {

            if (baseClassName == null) {
                if (modelType == null || TypeHelper.IsDynamicType(modelType)) {
                    baseClassName = "Xuld.RazorEngine.DynamicTemplateBase";
                } else {
                    baseClassName = String.Concat("Xuld.RazorEngine.TemplateBase<", TypeHelper.GetTypeName(modelType), ">");
                }
            }

            RazorTemplateEngine.Host.DefaultBaseClass = baseClassName;

            return RazorTemplateEngine.GenerateCode(templateReader, className, rootNamespace, sourceFileName);
        }

        /// <summary>
        /// 创建指定路径的模板解析后的模板类型。
        /// </summary>
        /// <param name="templateReader">用于获取模板内容的 <see cref="TextReader"/> 对象。</param>
        /// <param name="className">模板编译后的类名。默认为根据 <paramref name="sourceFileName"/> 动态生成。</param>
        /// <param name="rootNamespace">模板编译后的名字空间。默认为 RazorCompiledTemplates 。</param>
        /// <param name="sourceFileName">模板的文件名或模板的别名，用于在模板互相引用时使用。模板的名字作为模板的唯一标识符，应该保证其唯一性。默认为模板文件的路径或模板字符串本身的哈希值。</param>
        /// <param name="baseClassName">模板编译后的默认基类。如果值为 null，则使用 <see cref="TemplateBase&lt;T&gt;"/> 作为模板基类。在模板中可以通过 @inherits 关键字来覆盖这里的设置。</param>
        /// <param name="modelType">模板编译后的模型类型。如果值为 null，则表示不使用模板对象。未来版本中，在模板中可以通过 @model 关键字来覆盖这里的设置。</param>
        public Type AddTemplateType(TextReader templateReader, string className, string rootNamespace, string sourceFileName, string baseClassName, Type modelType) {

            // 1. 编译为 DOM 语法树。
            GeneratorResults generatorResult = GenerateCode(templateReader, className, rootNamespace, sourceFileName, baseClassName, modelType);

            if (!generatorResult.Success) {
                throw new RazorParseException(generatorResult.ParserErrors);
            }

            // 2. 编译为动态的类型。
            CompilerResults compilerResults = CodeDomProvider.CompileAssemblyFromDom(CompilerParameters, generatorResult.GeneratedCode);

#if PARSER_DEBUG
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            CodeDomProvider.GenerateCodeFromCompileUnit(generatorResult.GeneratedCode, writer, new CodeGeneratorOptions());
            Console.Write(sb.ToString());
          File.WriteAllText("D:\\aa.cs", sb.ToString());
#endif

            if (compilerResults.Errors.Count > 0) {
                throw new RazorComplieException(compilerResults.Errors);
            }

            // 3. 获取刚创建的类型。
            Type type = compilerResults.CompiledAssembly.GetType(String.IsNullOrEmpty(rootNamespace) ? className : String.Concat(rootNamespace, ".", className));

            // 4. 加入缓存列表。
            _templateTypeCache[sourceFileName] = new Tuple<Type, Type>(type, modelType);

            return type;

        }

        /// <summary>
        /// 创建指定路径的模板解析后的模板类型。
        /// </summary>
        /// <param name="templateReader">用于获取模板内容的 <see cref="TextReader"/> 对象。</param>
        /// <param name="sourceFileName">模板的文件名或模板的别名，用于在模板互相引用时使用。模板的名字作为模板的唯一标识符，应该保证其唯一性。默认为模板文件的路径或模板字符串本身的哈希值。</param>
        /// <param name="modelType">模板编译后的模型类型。如果值为 null，则表示不使用模板对象。未来版本中，在模板中可以通过 @model 关键字来覆盖这里的设置。</param>
        public Type AddTemplateType(TextReader templateReader, string sourceFileName, Type modelType) {
            return AddTemplateType(templateReader, System.Web.Razor.Parser.ParserHelpers.SanitizeClassName(Path.GetFileName(sourceFileName)), "RazorCompiledTemplates", sourceFileName, DefaultBaseClassName, modelType);
        }

        /// <summary>
        /// 移除一个模板类型。
        /// </summary>
        /// <param name="sourceFileName">模板的文件名或模板的别名，用于在模板互相引用时使用。模板的名字作为模板的唯一标识符，应该保证其唯一性。默认为模板文件的路径或模板字符串本身的哈希值。</param>
        /// <returns>如果成功找到并移除该元素，则为 true；否则为 false。</returns>
        public bool RemoveTemplateType(string sourceFileName) {
            return _templateTypeCache.Remove(sourceFileName);
        }

        /// <summary>
        /// 清空已缓存的模板类型列表。
        /// </summary>
        public void ClearTemplateTypes() {
            _templateTypeCache.Clear();
        }

        /// <summary>
        /// 获取或创建指定路径的模板解析后的模板类型。
        /// </summary>
        /// <param name="templateReader">用于获取模板内容的 <see cref="TextReader"/> 对象。</param>
        /// <param name="sourceFileName">模板的文件名或模板的别名，用于在模板互相引用时使用。模板的名字作为模板的唯一标识符，应该保证其唯一性。默认为模板文件的路径或模板字符串本身的哈希值。</param>
        /// <param name="modelType">模板编译后的模型类型。如果值为 null，则表示不使用模板对象。未来版本中，在模板中可以通过 @model 关键字来覆盖这里的设置。</param>
        /// <returns>指定路径的模板解析后的模板类型。</returns>
        public Type GetTemplateType(TextReader templateReader, string sourceFileName, Type modelType) {

            Tuple<Type, Type> t;
            if (_templateTypeCache.TryGetValue(sourceFileName, out t) && t.Item2 == modelType) {
                return t.Item1;
            }

            return AddTemplateType(templateReader, sourceFileName, modelType);
        }

        /// <summary>
        /// 创建指定的模板对象。
        /// </summary>
        /// <param name="context">解析执行使用的上下文。</param>
        /// <returns>返回模板对象实例。大部分情况是一个 <see cref="ITemplateBase"/> 实例。调用其 Execute 方法可执行模板生成操作。</returns>
        public object CreateTemplate(TemplateContext context) {

            // 获取对应的模板类型。
            Type templateType = GetTemplateType(context.InputReader, context.SourceFileName, context.Model == null ? null : context.Model.GetType());

            object template;

            try {

                // 创建模板类型实例。
                template = TypeHelper.CreateInstance(templateType);
            } catch (Exception e) {
                throw new RazorRuntimeException(e);
            }

            context.Razor = this;

            return template;

        }

        /// <summary>
        /// 编译指定的模板类，并使用指定的模型和上下文执行模板。
        /// </summary>
        /// <param name="context">解析执行使用的上下文。</param>
        public void Parse(TemplateContext context) {
            
            // 获取对应的模板类型。
            Type templateType = GetTemplateType(context.InputReader, context.SourceFileName, context.Model == null ? null : context.Model.GetType());

            object template;

            try {

                // 创建模板类型实例。
                template = TypeHelper.CreateInstance(templateType);
            } catch (Exception e) {
                throw new RazorRuntimeException(e);
            }

            context.Razor = this;

            // 运行模板。
            ITemplateBase t = template as ITemplateBase;
            
            if (t != null) {

                try {
                    t.Execute(context);
                } catch (Exception e) {
                    throw new RazorRuntimeException(e);
                }
            } else {
                MethodInfo m = templateType.GetMethod("Execute", new Type[1] { typeof(TemplateContext) });
                if (m != null) {
                    try {
                        m.Invoke(template, new object[1] { context });
                    } catch (Exception e) {
                        throw new RazorRuntimeException(e);
                    }
                } else {
                    throw new RazorRuntimeException("Method Not Found: void Execute(TemplateContext context);");
                }
            }

        }

        static Razor _razor;
        
        /// <summary>
        /// 提供快速解析 Razor 模板的辅助方法。
        /// </summary>
        /// <param name="template">模板内容。</param>
        /// <param name="model">解析时使用的模型对象。</param>
        /// <returns>解析后返回的字符串。</returns>
        public static string Parse(string template, object model = null) {
            if (_razor == null) {
                _razor = new Razor();
            }

            return _razor.ParseString(template, model);
        }

        /// <summary>
        /// 解析指定的模板内容。
        /// </summary>
        /// <param name="template">模板内容。</param>
        /// <param name="model">解析时使用的模型对象。</param>
        /// <param name="sourceFileName">模板的文件名或模板的别名，用于在模板互相引用时使用。模板的名字作为模板的唯一标识符，应该保证其唯一性。默认为模板文件的路径或模板字符串本身的哈希值。</param>
        /// <returns>解析后返回的字符串。</returns>
        public string ParseString(string template, object model, string sourceFileName) {
            StringBuilder sb = new StringBuilder();
            StringReader reader = new StringReader(template);
            StringWriter writer = new StringWriter(sb);
            Parse(new TemplateContext(reader, sourceFileName, writer) {
                Model = model
            });
            return sb.ToString();
        }

        /// <summary>
        /// 解析指定的模板内容。
        /// </summary>
        /// <param name="template">模板内容。</param>
        /// <param name="model">解析时使用的模型对象。</param>
        /// <returns>解析后返回的字符串。</returns>
        public string ParseString(string template, object model = null) {
            return ParseString(template, model, template.GetHashCode().ToString());
        }

        /// <summary>
        /// 解析指定的模板文件。使用 UTF-8 编码读写文件。
        /// </summary>
        /// <param name="templateFileName">模板文件路径。</param>
        /// <param name="model">解析时使用的模型对象。</param>
        /// <returns>返回解析后的内容。</returns>
        public void ParseFile(string templateFileName, Encoding encoding, object model, TextWriter writer) {
            templateFileName = Path.GetFullPath(templateFileName);
            using (StreamReader reader = new StreamReader(templateFileName, encoding)) {
                Parse(new TemplateContext(reader, templateFileName, writer) {
                    Model = model,
                    DefaultEncoding = encoding
                });
            }
        }

        /// <summary>
        /// 解析指定的模板文件。
        /// </summary>
        /// <param name="templateFileName">模板文件路径。</param>
        /// <param name="encoding">模板文件的编码。</param>
        /// <param name="model">解析时使用的模型对象。</param>
        /// <param name="saveFilePath">解析后保存的路径。</param>
        /// <param name="saveEncoding">保存的路径。</param>
        public void ParseFile(string templateFileName, Encoding encoding, object model, string saveFilePath, Encoding saveEncoding) {
            using(StreamWriter writer = new StreamWriter(saveFilePath, false, saveEncoding))
                ParseFile(templateFileName, encoding, model, writer);
        }

        /// <summary>
        /// 解析指定的模板文件。使用 UTF-8 编码读写文件。
        /// </summary>
        /// <param name="templateFileName">模板文件路径。</param>
        /// <param name="model">解析时使用的模型对象。</param>
        /// <param name="saveFilePath">解析后保存的路径。</param>
        public void ParseFile(string templateFileName, object model, string saveFilePath) {
            ParseFile(templateFileName, Encoding.UTF8, model, saveFilePath, Encoding.UTF8);
        }

        /// <summary>
        /// 解析指定的模板文件。
        /// </summary>
        /// <param name="templateFileName">模板文件路径。</param>
        /// <param name="encoding">模板文件的编码。</param>
        /// <param name="model">解析时使用的模型对象。</param>
        /// <returns>返回解析后的内容。</returns>
        public string ParseFile(string templateFileName, Encoding encoding, object model = null) {
            StringBuilder sb = new StringBuilder();
            ParseFile(templateFileName, encoding, model, new StringWriter(sb));
            return sb.ToString();
        }

        /// <summary>
        /// 解析指定的模板文件。使用 UTF-8 编码读写文件。
        /// </summary>
        /// <param name="templateFileName">模板文件路径。</param>
        /// <param name="model">解析时使用的模型对象。</param>
        /// <returns>返回解析后的内容。</returns>
        public string ParseFile(string templateFileName, object model = null) {
            return ParseFile(templateFileName, Encoding.UTF8, model);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose() {
            CodeDomProvider.Dispose();
        }
    }

}
