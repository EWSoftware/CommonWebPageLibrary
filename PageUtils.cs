//===============================================================================================================
// System  : ASP.NET Common Web Page Classes
// File    : PageUtils.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 09/13/2013
// Note    : Copyright 2002-2013, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a sealed class containing some utility functions used by the other classes in the
// EWSoftware.Web namespaces.  All members are static (shared) so just call them directly.
//
// This code may be used in compiled form in any way you desire.  This file may be redistributed unmodified by
// any means PROVIDING it is not sold for profit without the author's written consent, and providing that this
// notice and the author's name and all copyright notices remain intact.
//
// This code is provided "as is" with no warranty either express or implied.  The author accepts no liability
// for any damage or loss of business that this product may cause.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.0.0.0  09/19/2002  EFW  Created the code
// 2.0.0.0  02/18/2006  EFW  Updated for use with .NET 2.0
//===============================================================================================================

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// All classes go in the EWSoftware.Web namespace
namespace EWSoftware.Web
{
	/// <summary>
	/// This class contains a set of common utility functions.  All members
    /// are static (shared) so just call them directly.
	/// </summary>
	public sealed class PageUtils
	{
        //=====================================================================
        // Private class members

        // Regular expression objects for link searches
        private static Regex reURL, reUNC, reEMail, reTSURL, reTSUNC;
        private static MatchEvaluator urlMatchEvaluator, uncMatchEvaluator;

        // Expanded tab size for HTML encoding
        private static int tabSize = 4;
        private static string expandTabs;

        //=====================================================================
        // Properties

        /// <summary>
        /// This property can be used to alter the size for tab expansion
        /// when using <see cref="HtmlEncode"/>.  The default is to expand
        /// tab characters to four non-breaking spaces.
        /// </summary>
        public static int TabSize
        {
            get { return tabSize; }
            set
            {
                tabSize = value;
                expandTabs = null;
            }
        }

        //=====================================================================
        // Methods, etc

        /// <summary>
        /// Private constructor.  This class cannot be instantiated.
        /// </summary>
		private PageUtils()
		{
		}

        /// <summary>
        /// This can be called to encode an object for output to an HTML page.
	    /// </summary>
		/// <param name="objText">The object containing text to encode</param>
		/// <param name="encodeLinks">Convert URLs, UNCs and e-mail
        /// addresses to links if set to true.</param>
        /// <returns>The object as an HTML-encoded text string.</returns>
	    /// <remarks>It can be called to encode an object for output to an HTML
        /// page so that it renders any HTML special characters as literals
        /// instead of letting the browser interpret them. It encodes the text
        /// and replaces multiple spaces, tabs, and line breaks with their HTML
        /// equivalents. The size of expanded tab characters can be altered
        /// using the <see cref="TabSize"/> property.  The default is four
        /// non-breaking spaces.  If the object is null/Nothing, results in an
        /// empty string, or is a single space, a non-breaking space is
        /// returned.
        /// <p/>This is useful for displaying database fields that contain
        /// HTML special characters or nulls. If the <b>encodeLinks</b>
        /// parameters is true, URLs, UNCs, and e-mail addresses are converted
        /// to hyperlinks whenever possible using the <see cref="EncodeLinks"/>
        /// method. If false, they are not converted.</remarks>
        public static string HtmlEncode(Object objText, bool encodeLinks)
        {
            StringBuilder sb;
            string text;

            if(objText != null)
            {
                text = objText.ToString();

                if(text.Length != 0)
                {
                    // Create tab expansion string if not done already
                    if(expandTabs == null)
                        expandTabs = new String(' ',
                            PageUtils.TabSize).Replace(" ", "&nbsp;");

                    // Encode the string
                    sb = new StringBuilder(HttpUtility.HtmlEncode(text), 256);

                    sb.Replace("  ", "&nbsp;&nbsp;");  // Two spaces
                    sb.Replace("\t", expandTabs);
                    sb.Replace("\r", "");
                    sb.Replace("\n", "<br>");

                    text = sb.ToString();

                    if(text.Length > 1)
                    {
                        if(!encodeLinks)
                            return text;

                        // Try to convert URLs, UNCs, and e-mail addresses
                        // to links.
                        return PageUtils.EncodeLinks(text);
                    }

                    if(text.Length == 1 && text[0] != ' ')
                        return text;
                }
            }

            return "&nbsp;";
        }

        /// <summary>
        /// This takes the passed string and finds all URLs, UNCs, and e-mail addresses and converts them to
        /// clickable hyperlinks suitable for rendering in an HTML page.
        /// </summary>
        /// <param name="text">The text to search for links</param>
        /// <returns>The string with HTML hyperlinks.</returns>
        /// <remarks>This is called by the <see cref="HtmlEncode"/> method to handle the conversion during its
        /// encoding operation if needed. For UNC paths, it will include any text up to the first whitespace
        /// character. If the path contains spaces, you can enclose the entire path in angle brackets (i.e.
        /// &lt;\\Server\Folder\Name With Spaces&gt;) and the encoder will include all text between the angle
        /// brackets in the hyperlink. The angle brackets will not appear in the encoded hyperlink.</remarks>
        public static string EncodeLinks(string text)
        {
            // We'll create these on first use and keep them around
            // for subsequent calls to save resources.
            if(reURL == null)
            {
                reURL = new Regex(@"(((file|news|(ht|f|nn)tp(s?))://)|" +
                    @"(www\.))+[\w()*\-!_%]+.[\w()*\-/.!_#%]+[\w()*\-/.!_#%]" +
                    @"*((\?\w+(\=[\w()*\-/.!_#%]*)?)(((&amp;|&(?!\w+;))" +
                    @"(\w+(\=[\w()*\-/.!_#%]*)?))+)?)?",
                    RegexOptions.IgnoreCase);
                reUNC = new Regex(@"(\\{2}\w+(\.\w{1,3})?(\\((&.{2,8};|" +
                    @"[\w\-\.,@?^=%&:/~\+#\$])*[\w\-\@?^=%&/~\+#\$])?)*)|" +
                    @"((\<|\&lt;)\\{2}\w+(\.\w{1,3})?(\\((&.{2,8};|" +
                    @"[\w\-\.,@?^=%&:/~\+#\$ ])*)?)*(\>|\&gt;))",
                    RegexOptions.IgnoreCase);
                reEMail = new Regex(@"([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]" +
                    @"*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.)" +
                    @"{3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4]" +
                    @"[0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])",
                    RegexOptions.IgnoreCase);
                reTSURL = new Regex(@"[\-.!]$");
                reTSUNC = new Regex(@"\.?((&\#\d{1,3}|&\w{2,8});((&\#\d{1,3}|&\w{2,8}))?)+\w*$");

                urlMatchEvaluator = new MatchEvaluator(OnUrlMatch);
                uncMatchEvaluator = new MatchEvaluator(OnUncMatch);
            }

            // Do the replacements
            text = reURL.Replace(text, urlMatchEvaluator);
            text = reUNC.Replace(text, uncMatchEvaluator);
            text = reEMail.Replace(text, @"<a href='mailto:$&'>$&</a>");

            return text;
        }

        // Replace a URL with a link to the URL.  This checks for a
        // missing protocol and adds it if necessary.
        private static string OnUrlMatch(Match match)
        {
            StringBuilder sb = new StringBuilder("<a href='", 256);
            string url = match.Value;

            // Use default HTTP protocol if one wasn't specified
            if(url.IndexOf("://", StringComparison.Ordinal) == -1)
                sb.Append("http://");

            // Move trailing special characters outside the link
            Match m = reTSURL.Match(url);
            if(m.Success == true)
                url = reTSURL.Replace(url, "");

            sb.Append(url);
            sb.Append("' target='_BLANK'>");
            sb.Append(url);
            sb.Append("</a>");

            if(m.Success == true)
                sb.Append(m.Value);

            return sb.ToString();
        }

        // Replace a UNC with a link to the UNC.  This strips off any
        // containing brackets (plain or encoded) and flips the slashes.
        private static string OnUncMatch(Match match)
        {
            StringBuilder sb = new StringBuilder("<a href='file:", 256);
            string unc = match.Value;

            // Strip brackets if found.  If it has encoded brackets,
            // strip them too.
            if(unc[0] == '<')
                unc = unc.Substring(1, unc.Length - 2);
            else
                if(unc.StartsWith("&lt;", StringComparison.OrdinalIgnoreCase))
                    unc = unc.Substring(4, unc.Length - 8);

            // Move trailing special characters outside the link
            Match m = reTSUNC.Match(unc);
            if(m.Success == true)
                unc = reTSUNC.Replace(unc, "");

            sb.Append(unc);
            sb.Append("' target='_BLANK'>");

            // Replace backslashes with forward slashes
            sb.Replace('\\', '/');

            sb.Append(unc);
            sb.Append("</a>");

            if(m.Success == true)
                sb.Append(m.Value);

            return sb.ToString();
        }
	}
}
