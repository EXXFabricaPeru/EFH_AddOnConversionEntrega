﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AddOnUpdPrice.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AddOnUpdPrice.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT &quot;ItemCode&quot;,&quot;ItemName&quot; FROM OITM.
        /// </summary>
        internal static string BF_ObtenerArticulos {
            get {
                return ResourceManager.GetString("BF_ObtenerArticulos", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT &quot;ItemName&quot; FROM &quot;OITM&quot;
        ///WHERE &quot;ItemCode&quot; = $[$0_U_G.C_0_1.0].
        /// </summary>
        internal static string BF_ObtenerArticulosDesc {
            get {
                return ResourceManager.GetString("BF_ObtenerArticulosDesc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT &quot;CurrCode&quot;,&quot;CurrName&quot; FROM OCRN.
        /// </summary>
        internal static string BF_ObtenerMonedas {
            get {
                return ResourceManager.GetString("BF_ObtenerMonedas", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM &quot;@EXX_SETUP&quot; where &quot;U_EXX_ADDN&quot; =.
        /// </summary>
        internal static string DeleteSetup {
            get {
                return ResourceManager.GetString("DeleteSetup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT IFNULL(MAX(CAST(&quot;Code&quot; AS numeric)), 999) + 1 AS Numero FROM &quot;@EXX_SETUP&quot;.
        /// </summary>
        internal static string HanaCorrInstall {
            get {
                return ResourceManager.GetString("HanaCorrInstall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO &quot;@EXX_SETUP&quot; VALUES.
        /// </summary>
        internal static string HanaInsertSetup {
            get {
                return ResourceManager.GetString("HanaInsertSetup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select top 1 &quot;AName&quot;,&quot;AddOnVer&quot; from &quot;SBOCOMMON&quot;.&quot;SARI&quot; where &quot;AName&quot; = &apos;AddOnUpdPrice&apos; order by &quot;AddOnVer&quot; desc.
        /// </summary>
        internal static string HanaSari {
            get {
                return ResourceManager.GetString("HanaSari", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT ISNULL(MAX(CAST(&quot;Code&quot; AS numeric)), 999) + 1 AS Numero FROM &quot;@EXX_SETUP&quot;.
        /// </summary>
        internal static string SQLCorrInstall {
            get {
                return ResourceManager.GetString("SQLCorrInstall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select top 1 &quot;AName&quot;,&quot;AddOnVer&quot; from &quot;SBO-COMMON&quot;.&quot;dbo&quot;.&quot;SARI&quot; where &quot;AName&quot; = &apos;AddOnUpdPrice&apos; order by &quot;AddOnVer&quot; desc.
        /// </summary>
        internal static string SQLSari {
            get {
                return ResourceManager.GetString("SQLSari", resourceCulture);
            }
        }
    }
}
