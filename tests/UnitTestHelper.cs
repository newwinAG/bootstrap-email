using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace BootstrapEmailTests
{
    public static class UnitTestHelper
    {
        public static string LoadFile(string inFileName, string inFolderPath = null)
        {
            var tmpResourceName = inFileName;
            if (!string.IsNullOrWhiteSpace(inFolderPath))
            {
                tmpResourceName = $"{inFolderPath}.{tmpResourceName}";
            }
            tmpResourceName = "BootstrapEmailTests." + tmpResourceName;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(tmpResourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        public static bool CompareHtmlFiles(string inHtmlFile1, string inHtmlFile2)
        {
            var tmPRegex= new Regex("( ){2,20000}");
            inHtmlFile1 = tmPRegex.Replace(inHtmlFile1, "");
            inHtmlFile2 = tmPRegex.Replace(inHtmlFile2, "");
            var tmpDoc1 = new HtmlParser().ParseDocument(inHtmlFile1);
            var tmpDoc2 = new HtmlParser().ParseDocument(inHtmlFile2);

            var tmpBody1 = tmpDoc1.Body.OuterHtml;
            var tmpBody2 = tmpDoc2.Body.OuterHtml;

            var tmpResult = CompareNodes(
                //The Parsed result has some other base structure
                tmpDoc1.Body.FirstChild.ChildNodes[1].ChildNodes[1].ChildNodes[1]
                , tmpDoc2.Body);

            return tmpResult;
        }

        private static bool CompareNodes(INode inFirstNode, INode inSecondNode)
        {
            if (inFirstNode.ChildNodes.Length != inSecondNode.ChildNodes.Length)
            {
                return false;
            }
            for (int tmpChildI = 0; tmpChildI < inFirstNode.ChildNodes.Length; tmpChildI++)
            {
                if (!CompareNodes(inFirstNode.ChildNodes[tmpChildI], inSecondNode.ChildNodes[tmpChildI]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
