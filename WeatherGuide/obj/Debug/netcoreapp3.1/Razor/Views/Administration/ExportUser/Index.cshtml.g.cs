#pragma checksum "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "365598e4fd46be19378dd147bda40cc9ee44dc38"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Administration_ExportUser_Index), @"mvc.1.0.view", @"/Views/Administration/ExportUser/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\_ViewImports.cshtml"
using WeatherGuide;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\_ViewImports.cshtml"
using WeatherGuide.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"365598e4fd46be19378dd147bda40cc9ee44dc38", @"/Views/Administration/ExportUser/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d5fc2c3cde2cc7213e76b89a68f41131b7dda5e8", @"/Views/_ViewImports.cshtml")]
    public class Views_Administration_ExportUser_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
  
	ViewData["Title"] = "Index";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"<div style =""margin-top:50px;"">
 <div align=""left"">
	 <a href = ""/ExportUser/ExporttoExcel"">Export to excel</a>
 </div>
 <table class=""table table-boarded table-responsive"">
	 <thead>
	  <tr>
		  <th>Id</th> 
		  <th>Username</th>
		  <th>CountryId</th>
		  <th>StateId</th>
	 </tr>
  </thead>
");
#nullable restore
#line 17 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
    
			foreach(System.Data.DataRow dr in ViewBag.details.Rows)
			{

#line default
#line hidden
#nullable disable
            WriteLiteral("\t\t\t\t<tr>\r\n\t\t\t\t\t<td>");
#nullable restore
#line 21 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                   Write(dr["Id"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </td>\r\n\t\t\t\t\t<td>");
#nullable restore
#line 22 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                   Write(dr["UserName"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </td>\r\n\t\t\t\t\t<td>");
#nullable restore
#line 23 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                   Write(dr["CountryId"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </td>\r\n\t\t\t\t\t<td>");
#nullable restore
#line 24 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                   Write(dr["StateId"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </td>\r\n\t\t\t\t</tr>\r\n");
#nullable restore
#line 26 "C:\Users\Climclaff\source\repos\Works\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
			}
  

#line default
#line hidden
#nullable disable
            WriteLiteral(" </table>\r\n\r\n</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
