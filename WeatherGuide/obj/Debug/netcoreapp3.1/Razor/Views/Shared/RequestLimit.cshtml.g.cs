#pragma checksum "F:\work\WeatherGuide\WeatherGuide\Views\Shared\RequestLimit.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "deb40ca91db37e39749f0a2f2ce14bbfca10c2a8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_RequestLimit), @"mvc.1.0.view", @"/Views/Shared/RequestLimit.cshtml")]
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
#line 1 "F:\work\WeatherGuide\WeatherGuide\Views\Shared\RequestLimit.cshtml"
using Microsoft.AspNetCore.Mvc.Localization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"deb40ca91db37e39749f0a2f2ce14bbfca10c2a8", @"/Views/Shared/RequestLimit.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d5fc2c3cde2cc7213e76b89a68f41131b7dda5e8", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Shared_RequestLimit : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n<h1 class=\"text-danger\">");
#nullable restore
#line 5 "F:\work\WeatherGuide\WeatherGuide\Views\Shared\RequestLimit.cshtml"
                   Write(SharedLocalizer["Error"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\r\n<h2 class=\"text-danger\">");
#nullable restore
#line 6 "F:\work\WeatherGuide\WeatherGuide\Views\Shared\RequestLimit.cshtml"
                   Write(SharedLocalizer["TooManyRequests"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\r\n\r\n<a href=\"javascript:window.location.reload(true)\">");
#nullable restore
#line 8 "F:\work\WeatherGuide\WeatherGuide\Views\Shared\RequestLimit.cshtml"
                                             Write(SharedLocalizer["Try again"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a>");
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