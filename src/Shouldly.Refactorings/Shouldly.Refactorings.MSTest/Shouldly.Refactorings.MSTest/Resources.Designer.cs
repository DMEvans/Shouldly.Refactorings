﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shouldly.Refactorings.MSTest {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Shouldly.Refactorings.MSTest.Resources", typeof(Resources).GetTypeInfo().Assembly);
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
        ///   Looks up a localized string similar to Shouldly.
        /// </summary>
        internal static string MsTestAssertIsNullCategory {
            get {
                return ResourceManager.GetString("MsTestAssertIsNullCategory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assert.IsNull(value) should be replaced with value.ShouldBeNull().
        /// </summary>
        internal static string MsTestAssertIsNullDescription {
            get {
                return ResourceManager.GetString("MsTestAssertIsNullDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Convert to a Shouldly assertion.
        /// </summary>
        internal static string MsTestAssertIsNullFixText {
            get {
                return ResourceManager.GetString("MsTestAssertIsNullFixText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assert.IsNull({0}) should be replaced with {0}.ShouldBeNull().
        /// </summary>
        internal static string MsTestAssertIsNullMessageFormat {
            get {
                return ResourceManager.GetString("MsTestAssertIsNullMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assert.IsNull(value) should be replaced with value.ShouldBeNull().
        /// </summary>
        internal static string MsTestAssertIsNullTitle {
            get {
                return ResourceManager.GetString("MsTestAssertIsNullTitle", resourceCulture);
            }
        }
    }
}
