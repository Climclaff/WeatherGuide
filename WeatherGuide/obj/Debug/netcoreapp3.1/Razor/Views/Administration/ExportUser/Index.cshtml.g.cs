#pragma checksum "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e7fde603dfa274c8b34bfe094e26123dd8dd7dec"
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
#line 1 "F:\work\WeatherGuide\WeatherGuide\Views\_ViewImports.cshtml"
using WeatherGuide;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "F:\work\WeatherGuide\WeatherGuide\Views\_ViewImports.cshtml"
using WeatherGuide.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
using Microsoft.AspNetCore.Mvc.Localization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e7fde603dfa274c8b34bfe094e26123dd8dd7dec", @"/Views/Administration/ExportUser/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d5fc2c3cde2cc7213e76b89a68f41131b7dda5e8", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Administration_ExportUser_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
  
	ViewData["Title"] = "Index";

#line default
#line hidden
#nullable disable
            WriteLiteral("<div style =\"margin-top:50px;\">\r\n <div align=\"left\">\r\n\t <a href = \"/ExportUser/ExporttoExcel\">");
#nullable restore
#line 8 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                                      Write(SharedLocalizer["ExportToExcel"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a>\r\n </div>\r\n <table class=\"table table-boarded table-responsive\">\r\n\t <thead>\r\n\t  <tr>\r\n\t\t  <th>Id</th> \r\n\t\t  <th>Username</th>\r\n\t\t  <th>CountryId</th>\r\n\t\t  <th>StateId</th>\r\n\t </tr>\r\n  </thead>\r\n");
#nullable restore
#line 19 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
    
			foreach(System.Data.DataRow dr in ViewBag.details.Rows)
			{

#line default
#line hidden
#nullable disable
            WriteLiteral("\t\t\t\t<tr>\r\n\t\t\t\t\t<td>");
#nullable restore
#line 23 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                   Write(dr["Id"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </td>\r\n\t\t\t\t\t<td>");
#nullable restore
#line 24 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                   Write(dr["UserName"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </td>\r\n\t\t\t\t\t<td>");
#nullable restore
#line 25 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                   Write(dr["CountryId"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </td>\r\n\t\t\t\t\t<td>");
#nullable restore
#line 26 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
                   Write(dr["StateId"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </td>\r\n\t\t\t\t</tr>\r\n");
#nullable restore
#line 28 "F:\work\WeatherGuide\WeatherGuide\Views\Administration\ExportUser\Index.cshtml"
			}
  

#line default
#line hidden
#nullable disable
            WriteLiteral(" </table>\r\n\r\n</div>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IHtmlLocalizer<SharedResource> SharedLocalizer { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
