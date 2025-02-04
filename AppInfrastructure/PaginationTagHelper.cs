﻿using Fag_el_Gamous.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fag_el_Gamous.AppInfrastructure
{
    //this is specifically for divs
    [HtmlTargetElement("div", Attributes = "page-info")]
    public class PaginationTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlInfo;

        //Constructor
        public PaginationTagHelper(IUrlHelperFactory uhf)
        {
            _urlInfo = uhf;
        }

        //properties of the class
        public PageNumberingInfo PageInfo { get; set; }

        //dictionary (key value pairs) that we are creating
        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> KeyValuePairs { get; set; } = new Dictionary<string, object>();

        //these properties of solely for the beautification of the pagination
        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }
        public string PageParameters { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelp = _urlInfo.GetUrlHelper(ViewContext);

            //instantiate a new TagBuilding for div tags
            TagBuilder finishedTag = new TagBuilder("div");

            for (int i = 1; i <= PageInfo.NumPages; i++)
            {
                //instantiate a new TagBuilder for a tags
                TagBuilder individualTag = new TagBuilder("a");

                
                //append the necessary info to the individualTag
                KeyValuePairs["pageNum"] = i;
                individualTag.Attributes["href"] = urlHelp.Action("Index", KeyValuePairs);
                individualTag.Attributes["href"] = individualTag.Attributes["href"] + PageParameters;
                individualTag.InnerHtml.Append(i.ToString());

                if (PageClassesEnabled)
                {
                    individualTag.AddCssClass(PageClass);
                    individualTag.AddCssClass(i == PageInfo.CurrentPage ? PageClassSelected : PageClassNormal);
                }

                //append the individualTag to the finishedTag
                finishedTag.InnerHtml.AppendHtml(individualTag);

            }
            output.Content.AppendHtml(finishedTag.InnerHtml);
        }
    }
}
