#pragma checksum "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\Execution\Execute.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "11c5232ba5cd6a8405ad35b14227ba86c9eb2fb5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Execution_Execute), @"mvc.1.0.view", @"/Views/Execution/Execute.cshtml")]
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
#line 1 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\_ViewImports.cshtml"
using XAutomateMVC;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\_ViewImports.cshtml"
using XAutomateMVC.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"11c5232ba5cd6a8405ad35b14227ba86c9eb2fb5", @"/Views/Execution/Execute.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b3eb3a5bd4bb195597699a36c34031f9ee5ad7c0", @"/Views/_ViewImports.cshtml")]
    public class Views_Execution_Execute : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ProductViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("form-control chosen"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("multiple", new global::Microsoft.AspNetCore.Html.HtmlString("true"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("form-control "), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("form-control"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/vendor/bootstrap/js/bootstrap.bundle.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.SelectTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\Execution\Execute.cshtml"
  
  Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<!DOCTYPE html>\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "11c5232ba5cd6a8405ad35b14227ba86c9eb2fb55508", async() => {
                WriteLiteral(@"
    <script>
        /* Loop through all dropdown buttons to toggle between hiding and showing its dropdown content - This allows the user to have multiple dropdowns without any conflict */

        $(document).ready(function ($) {
            var screens = sessionStorage.getItem(""Screen"");
            if (screens == null || screens == """" || screens == undefined) {
                window.location.href = ""/Home/Index"";
            }
            var RoleName = sessionStorage.getItem(""RoleName"");
            var Screensplit = screens.split(""|"");
            for (var i = 0; i < Screensplit.length; i++) {
                var val = Screensplit[i];
                var show = val.split("","");
                var val2 = show[""0""];
                var val1 = show[""1""];
                switch (val2) {
                    case ""Connection"":
                        if (val1 == ""Show"") {
                            $(""#Menuconnection"").removeClass(""hidden"");
                        }
                  ");
                WriteLiteral(@"      else {
                            $(""#Menuconnection"").addClass(""hidden"");
                        }
                        break;
                    case ""Suite"":
                        if (val1 == ""Show"") {
                            $(""#MenuSuites"").removeClass(""hidden"");
                        }
                        else {
                            $(""#MenuSuites"").addClass(""hidden"");
                        }
                        break;
                    case ""Approach"":
                        if (val1 == ""Show"") {
                            $(""#MenuTestApproach"").removeClass(""hidden"");
                        }
                        else {
                            $(""#MenuTestApproach"").addClass(""hidden"");
                        }
                        break;
                    case ""Testcases"":
                        if (val1 == ""Show"") {
                            $(""#MenuTestcases"").removeClass(""hidden"");
                        }
           ");
                WriteLiteral(@"             else {
                            $(""#MenuTestcases"").addClass(""hidden"");
                        }
                        break;
                    case ""etldb"":
                        if (val1 == ""Show"") {
                            $(""#MenuEtlrules"").removeClass(""hidden"");
                        }
                        else {
                            $(""#MenuEtlrules"").addClass(""hidden"");
                        }
                        break;
                    case ""Release"":
                        if (val1 == ""Show"") {
                            $(""#MenuRelease"").removeClass(""hidden"");
                        }
                        else {
                            $(""#MenuRelease"").addClass(""hidden"");
                        }
                        break;
                    case ""Executions"":
                        if (val1 == ""Show"") {
                            $(""#MenuExecution"").removeClass(""hidden"");
                        }
           ");
                WriteLiteral(@"             else {
                            $(""#MenuExecution"").addClass(""hidden"");
                        }
                        break;
                    case ""TestReport"":
                        if (val1 == ""Show"") {
                            $(""#MenuTestReport"").removeClass(""hidden"");
                        }
                        else {
                            $(""#MenuTestReport"").addClass(""hidden"");
                        }
                        break;
                    case ""Analytics"":
                        if (val1 == ""Show"") {
                            $(""#MenuAnaltics"").removeClass(""hidden"");
                        }
                        else {
                            $(""#MenuAnaltics"").addClass(""hidden"");
                        }
                        break;
                    case ""Usermanage"":
                        if (val1 == ""Show"") {
                            $(""#MenuUserMangment"").removeClass(""hidden"");
                      ");
                WriteLiteral(@"  }
                        else {
                            $(""#MenuUserMangment"").addClass(""hidden"");
                        }
                        break;
                }

            }
            debugger;
            var dropdown = document.getElementsByClassName(""dropdown-btn"");
            var i;

            for (i = 0; i < dropdown.length; i++) {
                dropdown[i].addEventListener(""click"", function () {
                    this.classList.toggle(""active"");
                    var dropdownContent = this.nextElementSibling;
                    if (dropdownContent.style.display === ""block"") {
                        dropdownContent.style.display = ""none"";
                    } else {
                        dropdownContent.style.display = ""block"";
                    }
                });
            }
           var dropdown = document.getElementsByClassName(""dropdown-btn1"");
            var i;

            for (i = 0; i < dropdown.length; i++) {
            ");
                WriteLiteral(@"    dropdown[i].addEventListener(""click"", function () {
                    this.classList.toggle(""active"");
                    var dropdownContent = this.nextElementSibling;
                    if (dropdownContent.style.display === ""block"") {
                        dropdownContent.style.display = ""none"";
                    } else {
                        dropdownContent.style.display = ""block"";
                    }
                });
            }
        $(document).on(""change"", ""#selectrule"", function () {
                $(""#Error"").html("""");
            });
            $(document).on(""change"", ""#TestsuidId"", function () {
                
                $(""#Error"").html("""");
            });
            $(document).on(""change"", ""#ReleaseId"", function () {
                $(""#Error"").html("""");
            });
            $(document).on(""change"", ""#tagid"", function () {
                $(""#Error"").html("""");
            });
            $(document).on(""click"", ""#Trigger"", functio");
                WriteLiteral(@"n () {
                $(""#Trigger"").attr(""disabled"", true);
                debugger;
                var name = """";
                var approach1 = """";
                var connection1 = """";
                var SuiteName = $(""#Multisuite"").val();
                for (var i = 0; i < SuiteName.length; i++) {
                    if (name == """") {
                        name = SuiteName[i];
                    }
                    else {
                        name += "","" + SuiteName[i];
                    }
                }
                if (SuiteName == null || SuiteName == """") {
                    $(""#Error"").html(""Please Select Suite Name"");
         $(""#Trigger"").attr(""disabled"", false);
                    return false;
                }
                var approach = $('#selectrule').val();
                for (var i = 0; i < approach.length; i++) {
                    if (approach1 == """") {
                        approach1 = approach[i];
                    }
           ");
                WriteLiteral(@"         else {
                        approach1 += "","" + approach[i];
                    }
                }
                if (approach == null || approach == """") {
                    $(""#Trigger"").attr(""disabled"", false);
                    $(""#Error"").html(""Please Select Test Approach"");
                    return false;
                }
                var connection1 = $('#TestsuidId').val();
                //for (var i = 0; i < connection.length; i++) {
                //    if (connection1 == """") {
                //        connection1 = connection[i];
                //    }
                //    else {
                //        connection1 += "","" + connection[i];
                //    }
                //}
                if (connection1 == null || connection1 == """") {
                    $(""#Error"").html(""Please Select Connections"");
                    $(""#Trigger"").attr(""disabled"", false);
                    return false;
                }
                var releas");
                WriteLiteral(@"eno = $('#ReleaseId').val();
                if (releaseno == null || releaseno == """") {
                    $(""#Trigger"").attr(""disabled"", false);

                    $(""#Error"").html(""Please Select Release No"");
                    return false;
                }
                var tag = $(""#tagid"").val();
                if (tag == null || tag == """") {
                    $(""#Trigger"").attr(""disabled"", false);
                    $(""#Error"").html(""Please Enter Tag"");
                    return false;
                }
                var inValid = /\s/;
                if ((inValid.test(tag))) {
                    $(""#Trigger"").attr(""disabled"", false);
                    $(""#Error"").html('Tag should not contain whitespaces');
                    return false;
                }
               
               
                
                $.ajax(
                    {
                        type: ""GET"", //HTTP POST Method
                        url: ""/Execution/Trigger"", //");
                WriteLiteral(@" Controller/View
                        datatype: ""json"",
                        data: {
                            SuiteId: name,
                            tagno: tag,
                            approachname: approach1,
                            dbconnection: connection1,
                            releaseno: releaseno,
                        },
                        headers: {
                            ""Content-Type"": JSON,
                            'Authorization': localStorage.getItem(""AuthoToken""),
                        },
                        success: function (data) {
                            if (data == ""Auth Fail"") {
                                $(""#Error"").html(""Authentication Failed,Your Session Failed"");
                                $(""#Trigger"").attr(""disabled"", false);
                                return false;
                            }
                            else
                                if (data.includes(""Rules"")) {
        ");
                WriteLiteral(@"                            $(""#Error"").html(data);
                                    $(""#Trigger"").attr(""disabled"", false);
                                    return false;

                                }
                                else if (data.includes(""Testcase"")) {
                                    $(""#Error"").html(data);
                                    $(""#Trigger"").attr(""disabled"", false);
                                    return false;
                                }
                            debugger;
                            $(""#Multisuite"").val('').trigger('liszt:updated');;
                            $('#TestsuidId').val('').trigger('liszt:updated');;
                            $('#ReleaseId').val('');
                            $('#selectrule').empty();;
                            $('#tagid').val('');
                            $(""#Trigger"").attr(""disabled"", false);

                            $(""#selectrule"").trigger(""chosen:updated"");
         ");
                WriteLiteral(@"                   $('.chosen').chosen(""destroy"").chosen();
                            setTimeout(function () {
                                clearChosen()
                            }, 200);
                            var statsus = ""Passed  "" + data[""0""].passed + "",Failed  "" + data[""0""].failed + "",Total Tests "" + data[""0""].total_test;
                           // var tes = ""~/"" + SuiteName + ""/"" + SuiteName + """" + Version + "".html"";
                            $(""progress"").hide();
                            alert(""Test Cases Execute Successfully"");
                            $(""#Success"").html(statsus);
                        },
                        xhr: function () {
                            var fileXhr = $.ajaxSettings.xhr();
                            if (fileXhr.upload) {
                                $(""progress"").show();
                                fileXhr.upload.addEventListener(""progress"", function (e) {
                                    if (e.lengthComputable)");
                WriteLiteral(@" {
                                        $(""#fileProgress"").attr({
                                            value: e.loaded,
                                            max: e.total
                                        });
                                    }
                                }, false);
                            }
                            return fileXhr;
                        },
                        error: function (Result) {
                            $(""#Trigger"").attr(""disabled"", false);
                           debugger;
                        }

                    });

            });

            var option;
            option = $('<option/>');
            option.append(""<option value=''>Select Test Approach</option>"");
            var option1;
            option1 = $('<option/>');
            option1.append(""<option value=''>Select  Test Approach</option>"");

          //  $('#RulesId').append(option1);
           // $('#selectrule').app");
                WriteLiteral(@"end(option);
            var a = new Array();
            $(""#selectrule"").children(""option"").each(function (x) {
                test = false;
                b = a[x] = $(this).val();
                for (i = 0; i < a.length - 1; i++) {
                    if (b == a[i]) test = true;
                }
                if (test) $(this).remove();
            });
            $(document).on(""change"", ""#Multisuite"", function (event, params ) {
                $(""#Error"").html("""");
                debugger;
                var select = """";
                var name = """";
                if (params != undefined) {
                    select = params.selected;
                    if (select == undefined) {
                        name = params.deselected;
                    }
                    else {
                        name = params.selected;
                    }
                }
                else {
                    name = $(this).val();
                }
             
   ");
                WriteLiteral(@"           
                $.ajax(
                    {
                        type: ""GET"", //HTTP POST Method
                        url: ""/Execution/testapproach"", // Controller/View
                        contentType: ""application/json; charset=utf-8"",
                        dataType: ""json"",
                        data: {
                            SuiteId: name,
                        },
                        headers: {
                            ""Content-Type"": JSON,
                            'Authorization': localStorage.getItem(""AuthoToken""),
                        },
                        success: function (data) {
                            debugger;
                            if (data == ""Auth Fail"") {
                                $(""#Error"").html(""Authentication Failed,Your Session Failed"");
                                return false;
                            }
                            var option;
                          
                      ");
                WriteLiteral(@"      for (var i = 0; i < data.length; i++) {
                                var values = """";

                                $(""#selectrule option"").each(function () {
                                    debugger;
                                    var value = $(this).val();
                                    if (value == data[i].testApproachName) {
                                        values = data[i].testApproachName;
                                    }
                                    // Add $(this).val() to your list
                                });
                                if (values == """") {
                                    $('#selectrule').append(""<option value="" + data[i].testApproachName + "">"" + data[i].testApproachName + ""</option>"");
                                }
                                debugger;
                                var deselect = params.deselected;
                               
                                    if (params.desel");
                WriteLiteral(@"ected != undefined) {
                                        $(""option[value="" + data[i].testApproachName + ""]"").remove();
                                    }
                                
                            }
                            $(""#selectrule"").trigger(""chosen:updated"");
                            $('.chosen').chosen(""destroy"").chosen();
                            var a = new Array();
                            $(""#selectrule"").children(""option"").each(function (x) {
                                test = false;
                                b = a[x] = $(this).val();
                                for (i = 0; i < a.length - 1; i++) {
                                    if (b == a[i]) test = true;
                                }
                                if (test) $(this).remove();
                            });
                        },
                        error: function (Result) {
                            swal(""Error!"", ""Error."", ""error"");
  ");
                WriteLiteral(@"                      }

                    });
            });



        });
        function clearChosen() {
            debugger;
            $('select#Multisuite').trigger('chosen:updated');
            $('select#TestsuidId').trigger('chosen:updated');
        }
     


    </script>
    <div>

                <h3 style=""        color: #ba9122; padding: 14px; margin-left: 42px;"">Execution</h3>
                <div class=""container"" style="" background-color: white;  padding: 10px; padding-bottom: 100px;"">
                    <div class=""content"">
                        <div class="" container-scroller"">

                            <div class=""row"">
                                <div class="" col-lg-3"">
                                    <label><span style=""color: red;"">*</span>Test Suite</label><br />
                                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("select", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "11c5232ba5cd6a8405ad35b14227ba86c9eb2fb525115", async() => {
                    WriteLiteral("\r\n                                    ");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.SelectTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper);
#nullable restore
#line 418 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\Execution\Execute.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Multisuite);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
#nullable restore
#line 420 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\Execution\Execute.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.Items = (new SelectList(Model.Listofproducts,"Value", "Text"));

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-items", __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.Items, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"

                                </div>
                                <div class="" col-lg-3"">
                                    <label><span style=""color: red;"">*</span>Test Approach</label><br />
                                    <select class=""form-control chosen"" name=""Rules"" id=""selectrule"" multiple=""true"">
                                    </select>

                                </div>
                                <div class="" col-lg-3"">
                                    <label><span style=""color: red;"">*</span>Connections</label><br />

                                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("select", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "11c5232ba5cd6a8405ad35b14227ba86c9eb2fb528047", async() => {
                    WriteLiteral("\r\n                                    ");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.SelectTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper);
#nullable restore
#line 433 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\Execution\Execute.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.TestsuidId);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
#nullable restore
#line 435 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\Execution\Execute.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.Items = (new SelectList(Model.TestSuiteList,"Value", "Text"));

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-items", __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.Items, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n\r\n                                </div>\r\n                                <div class=\"col-lg-3\">\r\n                                    <label><span style=\"color: red;\">*</span>Release No</label><br />\r\n                                    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("select", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "11c5232ba5cd6a8405ad35b14227ba86c9eb2fb530513", async() => {
                    WriteLiteral("\r\n                                    ");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.SelectTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper);
#nullable restore
#line 441 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\Execution\Execute.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.ReleaseId);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
#nullable restore
#line 443 "D:\Abinesh\Xautomatedocker\Xautomate_docker\XAutomateMVC\XAutomateMVC\Views\Execution\Execute.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.Items = (new SelectList(Model.ReleaseList,"Value", "Text"));

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-items", __Microsoft_AspNetCore_Mvc_TagHelpers_SelectTagHelper.Items, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
                                </div>
                                <div class="" col-lg-3"" style=""padding-top:20px"">
                                    <label><span style=""color: red;"">*</span>Tag</label><br />
                                    <input type=""text"" name=""tag"" id=""tagid"" class=""form-control"" />
                                </div>
                                <div class=""col-md-12"" style=""margin-top:25px;right:0px;text-align:right"">
                                    <div class=""col-md-12 col-lg-12 col-sm-12"">
                                        <label style=""text-align:center;color:red"" id=""Error""></label>
                                        <label style=""text-align:center;color:green"" id=""Success""></label>
                                    </div>
                                    <progress id=""fileProgress"" style=""display: none""></progress>

                                    <input type=""button"" class=""text-center center-block"" style=""margin-top:20px;   ");
                WriteLiteral("     color: white;background-color: #c9a642\" id=\"Trigger\" value=\"Execute\">\r\n\r\n                                </div>\r\n                              \r\n");
                WriteLiteral("                            </div>\r\n\r\n");
                WriteLiteral(@"                        </div>
                        <!-- partial:../../partials/_footer.html -->

                    </div>

                </div>
            </div>
            <!-- /#page-content-wrapper -->

        </div>
        <!-- /#wrapper -->
        <!-- Bootstrap core JavaScript -->
        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "11c5232ba5cd6a8405ad35b14227ba86c9eb2fb534417", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"

        <!-- Menu Toggle Script -->
        <script>
            $(""#menu-toggle"").click(function (e) {
                e.preventDefault();
                $(""#wrapper"").toggleClass(""toggled"");
            });
            $(document).ready(function () {
                debugger;
                //$('.chosen').chosen(""destroy"").chosen();
                $("".chosen"").chosen();
                //jQuery("".chosen"").chosen();

            });
        </script>
    </div>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ProductViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
