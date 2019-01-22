//===============================================================================================================
// System  : ASP.NET Common Web Page Classes
// File    : AssemblyInfo.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 01/14/2019
// Note    : Copyright 2002-2019, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// ASP.NET Common Web Page Classes developed by Eric Woodruff
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
// 1.0.0.0  10/03/2002  EFW  Created the code
// 2.0.0.0  02/18/2006  EFW  Updated for use with .NET 2.0
// 3.0.0.0  09/30/2010  EFW  Updated for use with .NET 4.0
//===============================================================================================================

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.UI;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyProduct("EWSoftware ASP.NET Common Web Page Classes")]
[assembly: AssemblyTitle("ASP.NET Common Web Page Classes")]
[assembly: AssemblyDescription("A collection of useful ASP.NET web page classes")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Eric Woodruff (Eric@EWoodruff.us)")]
[assembly: AssemblyCopyright("Copyright \xA9 2002-2019, Eric Woodruff, All Rights Reserved")]
[assembly: AssemblyTrademark("Eric Woodruff, All Rights Reserved")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// Always specify the version value
[assembly: AssemblyVersion("3.0.0.2")]

// Define the embedded resources
[assembly: WebResource(EWSoftware.Web.BasePage.ScriptsPath + "DataChange.js", "text/javascript")]

[assembly: WebResource(EWSoftware.Web.BasePage.ScriptsPath + "SetFocus.js", "text/javascript")]
