Xuld.RazorEngine
===========

A templating engine built on Microsoft's Razor parsing engine written in C#, 
Xuld.RazorEngine allows you to use Razor syntax to build dynamic templates:

	string template = "Hello @Model.Name, welcome to RazorEngine!";
	string result = Razor.Parse(template, new { Name = "World" });

Features
===========
Xuld.RazorEngine is much smaller and more efficient then 
[https://github.com/Antaris/RazorEngine](https://github.com/Antaris/RazorEngine), which has only 8 source fiels.
In spite of this, Xuld.RazorEngine meets well for all requirements of templating.

[For more information, please visit our wiki](https://github.com/xuld/RazorEngine/wiki)


-------------------------------

Xuld.RazorEngine 是一个 C# 编写的基于微软官方 Razor 解析引擎的模板引擎。它允许作者使用 Razor 语法创建动态模板：

	string template = "Hello @Model.Name, welcome to RazorEngine!";
	string result = Razor.Parse(template, new { Name = "World" });

Features
===========
Xuld.RazorEngine 仅有 8 个源文件，比 [https://github.com/Antaris/RazorEngine](https://github.com/Antaris/RazorEngine) 更小更高效。
尽管如此，Xuld.RazorEngine 还是一个比较完善的模板引擎。


[更多使用说明请访问我们的 wiki](https://github.com/xuld/RazorEngine/wiki)

