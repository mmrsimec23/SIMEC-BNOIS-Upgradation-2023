using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;
using Infinity.Bnois.Api.Web.Areas.HelpPage.ModelDescriptions;

namespace Infinity.Bnois.Api.Web.Areas.HelpPage
{
#pragma warning disable CS0419 // Ambiguous reference in cref attribute: 'IDocumentationProvider'. Assuming 'IDocumentationProvider', but could have also matched other overloads including 'IDocumentationProvider'.
    /// <summary>
    /// A custom <see cref="IDocumentationProvider"/> that reads the API documentation from an XML documentation file.
    /// </summary>
    public class XmlDocumentationProvider : IDocumentationProvider, IModelDocumentationProvider
#pragma warning restore CS0419 // Ambiguous reference in cref attribute: 'IDocumentationProvider'. Assuming 'IDocumentationProvider', but could have also matched other overloads including 'IDocumentationProvider'.
    {
        private XPathNavigator _documentNavigator;
        private const string TypeExpression = "/doc/members/member[@name='T:{0}']";
        private const string MethodExpression = "/doc/members/member[@name='M:{0}']";
        private const string PropertyExpression = "/doc/members/member[@name='P:{0}']";
        private const string FieldExpression = "/doc/members/member[@name='F:{0}']";
        private const string ParameterExpression = "param[@name='{0}']";

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDocumentationProvider"/> class.
        /// </summary>
        /// <param name="documentPath">The physical path to XML document.</param>
        public XmlDocumentationProvider(string documentPath)
        {
            if (documentPath == null)
            {
                throw new ArgumentNullException("documentPath");
            }
            XPathDocument xpath = new XPathDocument(documentPath);
            _documentNavigator = xpath.CreateNavigator();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(HttpControllerDescriptor)'
        public string GetDocumentation(HttpControllerDescriptor controllerDescriptor)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(HttpControllerDescriptor)'
        {
            XPathNavigator typeNode = GetTypeNode(controllerDescriptor.ControllerType);
            return GetTagValue(typeNode, "summary");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(HttpActionDescriptor)'
        public virtual string GetDocumentation(HttpActionDescriptor actionDescriptor)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(HttpActionDescriptor)'
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "summary");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(HttpParameterDescriptor)'
        public virtual string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(HttpParameterDescriptor)'
        {
            ReflectedHttpParameterDescriptor reflectedParameterDescriptor = parameterDescriptor as ReflectedHttpParameterDescriptor;
            if (reflectedParameterDescriptor != null)
            {
                XPathNavigator methodNode = GetMethodNode(reflectedParameterDescriptor.ActionDescriptor);
                if (methodNode != null)
                {
                    string parameterName = reflectedParameterDescriptor.ParameterInfo.Name;
                    XPathNavigator parameterNode = methodNode.SelectSingleNode(String.Format(CultureInfo.InvariantCulture, ParameterExpression, parameterName));
                    if (parameterNode != null)
                    {
                        return parameterNode.Value.Trim();
                    }
                }
            }

            return null;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetResponseDocumentation(HttpActionDescriptor)'
        public string GetResponseDocumentation(HttpActionDescriptor actionDescriptor)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetResponseDocumentation(HttpActionDescriptor)'
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "returns");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(MemberInfo)'
        public string GetDocumentation(MemberInfo member)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(MemberInfo)'
        {
            string memberName = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetTypeName(member.DeclaringType), member.Name);
            string expression = member.MemberType == MemberTypes.Field ? FieldExpression : PropertyExpression;
            string selectExpression = String.Format(CultureInfo.InvariantCulture, expression, memberName);
            XPathNavigator propertyNode = _documentNavigator.SelectSingleNode(selectExpression);
            return GetTagValue(propertyNode, "summary");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(Type)'
        public string GetDocumentation(Type type)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'XmlDocumentationProvider.GetDocumentation(Type)'
        {
            XPathNavigator typeNode = GetTypeNode(type);
            return GetTagValue(typeNode, "summary");
        }

        private XPathNavigator GetMethodNode(HttpActionDescriptor actionDescriptor)
        {
            ReflectedHttpActionDescriptor reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                string selectExpression = String.Format(CultureInfo.InvariantCulture, MethodExpression, GetMemberName(reflectedActionDescriptor.MethodInfo));
                return _documentNavigator.SelectSingleNode(selectExpression);
            }

            return null;
        }

        private static string GetMemberName(MethodInfo method)
        {
            string name = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetTypeName(method.DeclaringType), method.Name);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => GetTypeName(param.ParameterType)).ToArray();
                name += String.Format(CultureInfo.InvariantCulture, "({0})", String.Join(",", parameterTypeNames));
            }

            return name;
        }

        private static string GetTagValue(XPathNavigator parentNode, string tagName)
        {
            if (parentNode != null)
            {
                XPathNavigator node = parentNode.SelectSingleNode(tagName);
                if (node != null)
                {
                    return node.Value.Trim();
                }
            }

            return null;
        }

        private XPathNavigator GetTypeNode(Type type)
        {
            string controllerTypeName = GetTypeName(type);
            string selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, controllerTypeName);
            return _documentNavigator.SelectSingleNode(selectExpression);
        }

        private static string GetTypeName(Type type)
        {
            string name = type.FullName;
            if (type.IsGenericType)
            {
                // Format the generic type name to something like: Generic{System.Int32,System.String}
                Type genericType = type.GetGenericTypeDefinition();
                Type[] genericArguments = type.GetGenericArguments();
                string genericTypeName = genericType.FullName;

                // Trim the generic parameter counts from the name
                genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
                string[] argumentTypeNames = genericArguments.Select(t => GetTypeName(t)).ToArray();
                name = String.Format(CultureInfo.InvariantCulture, "{0}{{{1}}}", genericTypeName, String.Join(",", argumentTypeNames));
            }
            if (type.IsNested)
            {
                // Changing the nested type name from OuterType+InnerType to OuterType.InnerType to match the XML documentation syntax.
                name = name.Replace("+", ".");
            }

            return name;
        }
    }
}
