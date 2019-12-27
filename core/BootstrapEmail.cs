using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using SharpScss;

namespace bootstrap_email
{
    public class BootstrapEmail
    {
        /// <summary>
        /// Takes a full HTML Page and parses it to be E-Mail Programm Compatible
        /// </summary>
        /// <param name="inHtml"></param>
        /// <param name="inCustomAdditionalCss"></param>
        /// <returns></returns>
        public static string Parse(string inHtml, string inCustomAdditionalCss = null)
        {
            var tmpContainer = new BootstrapEmail(inHtml);
            tmpContainer.CustomCss = inCustomAdditionalCss;
            return tmpContainer.ParseEmailSourceAndReturn();
        }

        private string EmailSource { get; set; }

        private IBrowsingContext Context => BrowsingContext.New(Configuration.Default);

        private IDocument EmailHtml { get; set; }

        private IElement DocumentBody => EmailHtml.QuerySelector("body");
        public BootstrapEmail(string inEmailHtml)
        {
            EmailSource = inEmailHtml;
        }

        private IDocument GenerateDocument(string inHtml)
        {
            var tmpTask = Context.OpenAsync(req => req.Content(inHtml));
            tmpTask.Wait();
            return tmpTask.Result;
        }

        public string CustomCss { get; set; } = "";

        public string ParseEmailSourceAndReturn()
        {
            EmailHtml = GenerateDocument(EmailSource);
            compile_html();
            inline_css();
            inject_head();

            //update_mailer(); //This methode is not required in C#

            return EmailHtml.ToHtml();
        }

        /// <summary>
        /// HTML des geparsten E-Mail HTML anpassen (ersetzungen)
        /// </summary>
        private void compile_html()
        {
            ReplaceNodesByClass("btn", "table");                //button   
            ReplaceNodesByClass("badge", "table-left");         //badge    
            ReplaceNodesByClass("alert", "table");              //alert    
            ReplaceNodesByClass("card", "table");               //card     
            ReplaceNodesByClass("card-body", "table");
            ReplaceNodesByTag("hr", "hr");                      //hr       
            ReplaceNodesByClass("container", "container");      //container
            ReplaceNodesByClass("container-fluid", "table");
            ReplaceNodesByClass("row", "row");                  //grid     
            ReplaceNodesByClassWithStartsWith("col", "col");
            ReplaceNodesByClassByAlign("float-left", "left");   //align    
            ReplaceNodesByClassByAlign("mx-auto", "center");
            ReplaceNodesByClassByAlign("float-right", "right");
            ReplacePadding("table");                            //padding  
            ReplaceMargin("table");                             //margin   
            Spacer();                                           //spacer   
            FinishTableNodes();                                 //table    
            FinishBodyNode();                                   //body     
        }

        private void inline_css()
        {
            var tmpCss = GetFullInlineCss();
            var tmpMailHtml = EmailHtml.ToHtml();

            CustomCss = CustomCss ?? "";
            var tmpResult = PreMailer.Net.PreMailer.MoveCssInline(tmpMailHtml, false, null,  CustomCss);

            tmpMailHtml = tmpResult.Html;

            EmailHtml = GenerateDocument(tmpMailHtml);
        }

        private string GetFullInlineCss()
        {
            var tmpStringBuilder = new StringBuilder();

            tmpStringBuilder.AppendLine(LoadFile("sass", "_functions.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_variables.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_reboot_email.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_button.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_display.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_grid.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_card.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_container.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_badge.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_table.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_hr.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_alert.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_image.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_typography.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_color.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_preview.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_spacing.scss"));
            tmpStringBuilder.AppendLine(LoadFile("sass", "_border.scss"));

            return Scss.ConvertToCss(tmpStringBuilder.ToString()).Css;
        }

        private void inject_head()
        {
            EmailHtml.QuerySelector("head").AppendChild(bootstrap_email_head().FirstChild);
        }

        private INode bootstrap_email_head()
        {
            var tmpNode = EmailHtml.CreateElement("div");
            tmpNode.InnerHtml = "<div><title></title></div>";
            var tmpcss = Scss.ConvertToCss(LoadFile("", "head.scss"), new ScssOptions() { OutputStyle = ScssOutputStyle.Compressed });

            tmpNode.InnerHtml = $"<style type=\"text/css\">" + tmpcss.Css + "</style>";
            return tmpNode;
        }
        /// <summary>
        /// Replace all nodes with the Type and the spezified Template
        /// </summary>
        /// <param name="inTemplateFileName"></param>
        private void ReplaceNodesByTag(string inTag, string inTemplateFileName)
        {
            inTag = inTag.ToLower();
            var tmpChildNodes = DocumentBody.Descendents().Where(inItem => (inItem as IElement)?.LocalName == inTag);
            foreach (IElement tmpNode in tmpChildNodes.Reverse().ToList())
            {
                tmpNode.ClassList.Add("hr");
                ReplaceNodeWithTemplateNode(tmpNode, inTemplateFileName);
            }
        }

        /// <summary>
        /// Replace all nodes with the Type and the spezified Template
        /// </summary>
        /// <param name="inNodeClass"></param>
        /// <param name="inTemplateFileName"></param>
        private void ReplaceNodesByClassWithStartsWith(string inNodeClass, string inTemplateFileName)
        {
            var tmpChildNodes = GetNodesByFunction((INode inNode) =>
            {
                if (inNode is IElement)
                {
                    return (inNode as IElement).ClassList.Any(inItem => inItem.StartsWith(inNodeClass));
                }
                return false;
            }, DocumentBody);

            foreach (IElement tmpNode in tmpChildNodes.ToList())
            {
                ReplaceNodeWithTemplateNode(tmpNode, inTemplateFileName);
            }
        }

        /// <summary>
        /// Return Nodes by Function (Leaf first)
        /// </summary>
        /// <param name="inFunction"></param>
        /// <param name="inParentNode"></param>
        /// <returns></returns>
        private IEnumerable<INode> GetNodesByFunction(Func<INode, bool> inFunction, INode inParentNode)
        {
            foreach (var tmpChild in inParentNode.ChildNodes)
            {
                foreach (var tmpNodeResult in GetNodesByFunction(inFunction, tmpChild))
                {
                    yield return tmpNodeResult;
                }
                if (inFunction(tmpChild))
                {
                    yield return tmpChild;
                }
            }
        }

        /// <summary>
        /// Replace all nodes with the Type and the spezified Template
        /// </summary>
        /// <param name="inNodeClass"></param>
        /// <param name="inTemplateFileName"></param>
        private void ReplaceNodesByClass(string inNodeClass, string inTemplateFileName)
        {
            var tmpChildNodes = GetNodesByFunction((inNode) => (inNode as IElement)?.ClassList.Contains(inNodeClass) == true, DocumentBody);
            foreach (IElement tmpNode in tmpChildNodes.Reverse().ToList())
            {
                ReplaceNodeWithTemplateNode(tmpNode, inTemplateFileName);
            }
        }

        /// <summary>
        /// Replace all nodes with the Type and the spezified Template
        /// </summary>
        /// <param name="inNodeClass"></param>
        /// <param name="inAlignName"></param>
        private void ReplaceNodesByClassByAlign(string inNodeClass, string inAlignName)
        {
            var tmpChildNodes = GetNodesByFunction((inNode) => (inNode as IElement)?.ClassList.Contains(inNodeClass) == true, DocumentBody);
            foreach (IElement tmpNode in tmpChildNodes.Reverse().ToList())
            {
                if (tmpNode.LocalName == "table")
                {
                    tmpNode.ClassList.Remove(inNodeClass);
                    tmpNode.SetAttribute("align", inAlignName);
                }
                else
                {
                    ReplaceNodeWithTemplateNode(tmpNode, "align-" + inAlignName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inAlignName"></param>
        private void ReplacePadding(string inAlignName)
        {
            var inNodeClassList = new List<string> { "p-", "pt-", "pr-", "pb-", "pl-", "px-", "py-" };
            var tmpChildNodes = GetNodesByFunction((inNode) => FoundClassesWithStart(inNode, inNodeClassList).Any(), DocumentBody);
            foreach (IElement tmpNode in tmpChildNodes.ToList())
            {
                if (tmpNode.LocalName != "table")
                {
                    var tmpClasses = FoundClassesWithStart(tmpNode, inNodeClassList);
                    foreach (var tmpClass in tmpClasses)
                    {
                        tmpNode.ClassList.Remove(tmpClass);
                    }
                    var tmpDict = new Dictionary<string, string>()
                    {
                        {"classes", string.Join(" ", tmpNode.ClassList)},
                        {"contents", tmpNode.OuterHtml},
                    };

                    var tmpNewNodeHtml = Template("table", tmpDict);
                    tmpNode.Replace(EmailHtml.CreateElement(tmpNewNodeHtml), tmpNode);
                }
            }
        }

        /// <summary>
        /// Replace all nodes with the Type and the spezified Template
        /// </summary>
        private void Spacer()
        {
            var spacers = new Dictionary<string, string>
            {
                {"0", "0"},
                {"1", "4"},
                {"2", "8"},
                {"3", "16"},
                {"4", "24"},
                {"5", "48"}
            };
            var tmpChildNodes = GetNodesByFunction((inNode) => (inNode as IElement)?.ClassList.Any(inItem => inItem.StartsWith("s-")) == true, DocumentBody);
            foreach (IElement tmpNode in tmpChildNodes.ToList())
            {
                var tmpDict = new Dictionary<string, string>()
                {
                    {"classes", string.Join(" ", tmpNode.ClassList)+" w-100"},
                    {"contents", tmpNode.OuterHtml},
                };
                var tmpHtml = Template("table", tmpDict);

                tmpNode.Replace(CreateFromHtml(tmpHtml, tmpNode.Parent as IElement));
            }

        }

        /// <summary>
        /// Replace all nodes with the Type and the spezified Template
        /// </summary>
        /// <param name="inNodeClass"></param>
        /// <param name="inAlignName"></param>
        private void ReplaceMargin(string inAlignName)
        {
            var inNodeClassList = new List<string> { "my-", "mt-", "mb-" };
            var tmpChildNodes = GetNodesByFunction((inNode) => FoundClassesWithStart(inNode, inNodeClassList).Any(), DocumentBody);
            foreach (IElement tmpNode in tmpChildNodes.Reverse().ToList())
            {
                var tmpClasses = FoundClassesWithStart(tmpNode, inNodeClassList);
                var tmpHtml = "";
                if (tmpClasses.Any(inItem => inItem.StartsWith("my-") || inItem.StartsWith("mt-")))
                {
                    var tmpTempDict = new Dictionary<string, string>(){{"classes",string.Join(" ",
                        tmpClasses.Where(inItem => inItem.StartsWith("my-") || inItem.StartsWith("mt-"))
                            .Select(inItem=> $"s-"+inItem.Substring(3)))}};

                    tmpHtml += Template("div", tmpTempDict);
                }

                tmpHtml += tmpNode.OuterHtml;

                if (tmpClasses.Any(inItem => inItem.StartsWith("my-") || inItem.StartsWith("mb-")))
                {
                    var tmpTempDict = new Dictionary<string, string>(){{"classes",string.Join(" ",
                        tmpClasses.Where(inItem => inItem.StartsWith("my-") || inItem.StartsWith("mb-"))
                            .Select(inItem=> $"s-"+inItem.Substring(3)))}};

                    tmpHtml += Template("div", tmpTempDict);
                }

                tmpNode.Replace(CreateFromHtml($"<div>" + tmpHtml + "</div>", tmpNode.Parent as IElement));
            }
        }

        /// <summary>
        /// Find Class by Name (StartsWith)
        /// </summary>
        /// <param name="inNode"></param>
        /// <param name="inClassListStart"></param>
        /// <returns></returns>
        private IEnumerable<string> FoundClassesWithStart(INode inNode, List<string> inClassListStart)
        {
            if (inNode is IElement)
            {
                foreach (var tmpClass in (inNode as IElement).ClassList)
                {
                    foreach (var tmpClassStart in inClassListStart)
                    {
                        if (tmpClass.StartsWith(tmpClassStart))
                        {
                            yield return tmpClass;
                        }
                    }
                }
            }
        }

        private void ReplaceNodeWithTemplateNode(IElement inNode, string inTemplateFileName)
        {
            //Clone node and get required content out of it
            var tmpNewNode = (IElement)inNode.Clone(true);
            foreach (var tmpClass in tmpNewNode.ClassList.ToList())
            {
                tmpNewNode.ClassList.Remove(tmpClass);
            }
            var tmpDict = new Dictionary<string, string>()
            {
                {"classes", string.Join(" ", inNode.ClassList)},
                {"contents", tmpNewNode.OuterHtml},
            };

            var tmpNewNodeHtml = Template(inTemplateFileName, tmpDict);

            inNode.Replace(CreateFromHtml(tmpNewNodeHtml, inNode.Ancestors().First() as IElement));
        }

        private void FinishTableNodes()
        {
            var tmpChildNodes = DocumentBody.Descendents().Where(inItem => (inItem as IElement)?.LocalName == "table");
            foreach (IElement tmpChild in tmpChildNodes)
            {
                tmpChild.SetAttribute("border", "0");
                tmpChild.SetAttribute("cellpadding", "0");
                tmpChild.SetAttribute("cellspacing", "0");
            }
        }

        private IElement CreateFromHtml(string inHtml, IElement inParent)
        {
            return new HtmlParser().ParseFragment(inHtml, inParent).First() as IElement;
        }

        /// <summary>
        /// Replace Body Tag.
        /// </summary>
        private void FinishBodyNode()
        {
            var tmpDict = new Dictionary<string, string>()
            {
                {"classes", string.Join(" ", DocumentBody.ClassList)},
                {"contents", DocumentBody.InnerHtml},
            };
            DocumentBody.Replace(CreateFromHtml(Template("body", tmpDict), DocumentBody as IElement));
        }

        /// <summary>
        /// Load a Template and replace the Spezific parts in it
        /// </summary>
        /// <param name="inTemplateName"></param>
        /// <param name="inBinding"></param>
        /// <returns></returns>
        private string Template(string inTemplateName, IDictionary<string, string> inBinding)
        {
            var tmpErb = LoadFile("templates", $"{inTemplateName}.html.erb");
            var tmpFindRegex = new Regex("<%= *(?<Key>\\w+) *%>");
            foreach (Match tmpMatch in tmpFindRegex.Matches(tmpErb))
            {
                var tmpKey = tmpMatch.Groups["Key"].Value;
                var tmpRegex = new Regex($"<%= {tmpKey} %>");

                //Auslesen und setzen
                inBinding.TryGetValue(tmpKey, out var tmpValue);
                tmpErb = tmpRegex.Replace(tmpErb, tmpValue ?? "");
            }
            return tmpErb;
        }

        /// <summary>
        /// Load File. Every File load goes via this methode
        /// </summary>
        /// <param name="inFolder"></param>
        /// <param name="inName"></param>
        /// <returns></returns>
        private static string LoadFile(string inFolder, string inName)
        {
            var tmpResourceName = inName;
            if (!string.IsNullOrWhiteSpace(inFolder))
            {
                tmpResourceName = $"{inFolder}.{tmpResourceName}";
            }
            tmpResourceName = "BootstrapE_Mail." + tmpResourceName;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(tmpResourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }
}
