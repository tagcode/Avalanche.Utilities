// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Reflection;
using System.Xml;
using Avalanche.Utilities.Provider;

/// <summary>Provides xml document for assemblies.</summary>
/// <example>
///     <![CDATA[AssemblyDocumentProvider.Field.Cached.TryGetValue(fieldInfo, out string description);]]>
///     <![CDATA[AssemblyDocumentProvider.Type.Cached.TryGetValue(type, out string description);]]>
///     <![CDATA[AssemblyDocumentProvider.Property.Cached[propertyInfo];]]>
///     <![CDATA[AssemblyDocumentProvider.Method.Cached[methodInfo];]]>
/// </example>
public class AssemblyDocumentProvider : ProviderBase<Assembly, XmlDocument>
{
    /// <summary>Assembly .xml comment document provider</summary>
    static readonly IProvider<Assembly, XmlDocument> instance = new AssemblyDocumentProvider();
    /// <summary>Assembly .xml comment document provider</summary>
    static readonly IProvider<Assembly, XmlDocument> cached = instance.Cached();
    /// <summary>Assembly .xml comment document provider</summary>
    public static IProvider<Assembly, XmlDocument> Instance => instance;
    /// <summary>Assembly .xml comment document provider</summary>
    public static IProvider<Assembly, XmlDocument> Cached => cached;

    /// <summary></summary>
    public override bool TryGetValue(Assembly assembly, out XmlDocument doc)
    {
        // No assembly
        if (assembly == null) { doc = null!; return false; }
        // .dll path
        string? dll = assembly?.Location;
        // .xml path
        string? xml = Path.ChangeExtension(dll, ".xml");
        // Does not exist
        if (xml == null || !File.Exists(xml)) { doc = null!; return false; }
        // Create document
        doc = new XmlDocument();
        // Read .xml
        doc.Load(xml);
        // Got document
        return true;
    }

    /// <summary>Summary provider for <see cref="System.Type"/></summary>
    public class Type : ProviderBase<System.Type, string>
    {
        /// <summary></summary>
        static readonly IProvider<System.Type, string> instance = new Type(AssemblyDocumentProvider.Instance);
        /// <summary></summary>
        static readonly IProvider<System.Type, string> cached = new Type(AssemblyDocumentProvider.Cached).Cached();
        /// <summary></summary>
        public static IProvider<System.Type, string> Instance => instance;
        /// <summary></summary>
        public static IProvider<System.Type, string> Cached => cached;

        /// <summary></summary>
        protected IProvider<Assembly, XmlDocument> documentProvider;

        /// <summary></summary>
        public Type(IProvider<Assembly, XmlDocument> documentProvider)
        {
            this.documentProvider = documentProvider ?? throw new ArgumentNullException(nameof(documentProvider));
        }

        /// <summary>Get summary element</summary>
        public override bool TryGetValue(System.Type type, out string summary)
        {
            // Get assembly
            Assembly? assembly = type?.Assembly;
            // Get document
            if (assembly == null || !documentProvider.TryGetValue(assembly, out XmlDocument doc)) { summary = null!; return false; }
            // Create type reference
            string typeReference = type!.FullName!.Replace('+', '.');
            // Get node
            XmlNode? node = doc.SelectSingleNode($"//member[starts-with(@name, 'T:{typeReference}')]/summary");
            // Get content
            summary = node?.InnerXml!;
            // Return
            return summary != null;
        }
    }

    /// <summary>Summary provider for <see cref="PropertyInfo"/></summary>
    public class Property : ProviderBase<PropertyInfo, string>
    {
        /// <summary></summary>
        static readonly IProvider<PropertyInfo, string> instance = new Property(AssemblyDocumentProvider.Instance);
        /// <summary></summary>
        static readonly IProvider<PropertyInfo, string> cached = new Property(AssemblyDocumentProvider.Cached).Cached();
        /// <summary></summary>
        public static IProvider<PropertyInfo, string> Instance => instance;
        /// <summary></summary>
        public static IProvider<PropertyInfo, string> Cached => cached;

        /// <summary></summary>
        protected IProvider<Assembly, XmlDocument> documentProvider;

        /// <summary></summary>
        public Property(IProvider<Assembly, XmlDocument> documentProvider)
        {
            this.documentProvider = documentProvider ?? throw new ArgumentNullException(nameof(documentProvider));
        }

        /// <summary>Get summary element</summary>
        public override bool TryGetValue(PropertyInfo pi, out string summary)
        {
            // Get type
            System.Type? type = pi?.DeclaringType ?? pi?.ReflectedType;
            // Get assembly
            Assembly? assembly = type?.Assembly;
            // Get document
            if (assembly == null || !documentProvider.TryGetValue(assembly, out XmlDocument doc)) { summary = null!; return false; }
            // Create type reference
            string typeReference = type!.FullName!.Replace('+', '.');
            // Get node
            XmlNode? node = doc.SelectSingleNode($"//member[starts-with(@name, 'P:{typeReference}.{pi!.Name}')]/summary");
            // Get content
            summary = node?.InnerXml!;
            // Return
            return summary != null;
        }
    }

    /// <summary>Summary provider for <see cref="FieldInfo"/></summary>
    public class Field : ProviderBase<FieldInfo, string>
    {
        /// <summary></summary>
        static readonly IProvider<FieldInfo, string> instance = new Field(AssemblyDocumentProvider.Instance);
        /// <summary></summary>
        static readonly IProvider<FieldInfo, string> cached = new Field(AssemblyDocumentProvider.Cached).Cached();
        /// <summary></summary>
        public static IProvider<FieldInfo, string> Instance => instance;
        /// <summary></summary>
        public static IProvider<FieldInfo, string> Cached => cached;

        /// <summary></summary>
        protected IProvider<Assembly, XmlDocument> documentProvider;

        /// <summary></summary>
        public Field(IProvider<Assembly, XmlDocument> documentProvider)
        {
            this.documentProvider = documentProvider ?? throw new ArgumentNullException(nameof(documentProvider));
        }

        /// <summary>Get summary element</summary>
        public override bool TryGetValue(FieldInfo fi, out string summary)
        {
            // Get type
            System.Type? type = fi?.DeclaringType ?? fi?.ReflectedType;
            // Get assembly
            Assembly? assembly = type?.Assembly;
            // Get document
            if (assembly == null || !documentProvider.TryGetValue(assembly, out XmlDocument doc)) { summary = null!; return false; }
            // Create type reference
            string typeReference = type!.FullName!.Replace('+', '.');
            // Get node
            XmlNode? node = doc.SelectSingleNode($"//member[starts-with(@name, 'F:{typeReference}.{fi!.Name}')]/summary");
            // Get content
            summary = node?.InnerXml!;
            // Return
            return summary != null;
        }
    }

    /// <summary>Summary provider for <see cref="MethodInfo"/></summary>
    public class Method : ProviderBase<MethodInfo, string>
    {
        /// <summary></summary>
        static readonly IProvider<MethodInfo, string> instance = new Method(AssemblyDocumentProvider.Instance);
        /// <summary></summary>
        static readonly IProvider<MethodInfo, string> cached = new Method(AssemblyDocumentProvider.Cached).Cached();
        /// <summary></summary>
        public static IProvider<MethodInfo, string> Instance => instance;
        /// <summary></summary>
        public static IProvider<MethodInfo, string> Cached => cached;

        /// <summary></summary>
        protected IProvider<Assembly, XmlDocument> documentProvider;

        /// <summary></summary>
        public Method(IProvider<Assembly, XmlDocument> documentProvider)
        {
            this.documentProvider = documentProvider ?? throw new ArgumentNullException(nameof(documentProvider));
        }

        /// <summary>Get summary element</summary>
        public override bool TryGetValue(MethodInfo mi, out string summary)
        {
            // Get type
            System.Type? type = mi?.DeclaringType ?? mi?.ReflectedType;
            // Get assembly
            Assembly? assembly = type?.Assembly;
            // Get document
            if (assembly == null || !documentProvider.TryGetValue(assembly, out XmlDocument doc)) { summary = null!; return false; }
            // Formulate parameter string
            string paramsString = mi!.GetParameters().Length == 0 ? "" : "(" + String.Join(',', mi.GetParameters().Select(pi => pi.ParameterType.FullName)) + ")";
            // Create type reference
            string typeReference = type!.FullName!.Replace('+', '.');
            // Formulate xpath
            string xpath = $"//member[starts-with(@name, 'M:{typeReference}.{mi.Name}{paramsString}')]/summary";
            // Get node
            XmlNode? node = doc.SelectSingleNode(xpath);
            // Get content
            summary = node?.InnerXml!;
            // Return
            return summary != null;
        }
    }
}
