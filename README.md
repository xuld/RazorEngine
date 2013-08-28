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
