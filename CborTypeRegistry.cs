// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborTypeRegistry.cs" company="">
//   
// </copyright>
// <summary>
//   The cbor property template.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naphaso.Cbor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using DiverseWorlds.Logic.Network.Cbor.Exception;

    using Naphaso.Cbor.Attribute;

    /// <summary>
    /// The cbor property template.
    /// </summary>
    public class CborPropertyTemplate
    {
        #region Fields

        /// <summary>
        /// The name.
        /// </summary>
        public string name;

        /// <summary>
        /// The property.
        /// </summary>
        public PropertyInfo property;

        #endregion
    }

    /// <summary>
    /// The cbor type template.
    /// </summary>
    public class CborTypeTemplate
    {
        #region Fields

        /// <summary>
        /// The properties.
        /// </summary>
        public CborPropertyTemplate[] properties;

        /// <summary>
        /// The properties by name.
        /// </summary>
        public Dictionary<string, CborPropertyTemplate> propertiesByName;

        /// <summary>
        /// The tag.
        /// </summary>
        public uint tag;

        /// <summary>
        /// The type.
        /// </summary>
        public Type type;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get property type.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public Type GetPropertyType(string property)
        {
            return this.propertiesByName[property].property.PropertyType;
        }

        /// <summary>
        /// The set value.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="CborException">
        /// </exception>
        public void SetValue(object obj, string property, object value)
        {
            if (property == null)
            {
                throw new CborException("invalid object representation");
            }

            CborPropertyTemplate propertyTemplate;
            if (!this.propertiesByName.TryGetValue(property, out propertyTemplate))
            {
                throw new CborException("type " + this.type.Name + " not contains property " + property);
            }

            Type ptype = propertyTemplate.property.PropertyType;

            // bool nullable = false;
            if (ptype.IsGenericType && ptype.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                ptype = ptype.GetGenericArguments()[0];
            }

            if (value is int)
            {
                if (ptype == typeof(UInt64))
                {
                    // TODO: fix typecasts
                    propertyTemplate.property.SetValue(obj, (ulong)(int)value, null);
                    return;
                }

                if (ptype.IsEnum)
                {
                    propertyTemplate.property.SetValue(obj, Enum.ToObject(ptype, value), null);

                    return;
                }
            }

            propertyTemplate.property.SetValue(obj, value, null);
        }

        #endregion
    }

    /// <summary>
    /// The cbor type registry.
    /// </summary>
    public class CborTypeRegistry
    {
        #region Static Fields

        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly CborTypeRegistry Instance = new CborTypeRegistry();

        /// <summary>
        /// The namespaces.
        /// </summary>
        private static readonly string[] namespaces = { "DiverseWorlds.Logic.Network.Model" };

        #endregion

        #region Fields

        /// <summary>
        /// The _tags.
        /// </summary>
        private readonly Dictionary<uint, CborTypeTemplate> _tags = new Dictionary<uint, CborTypeTemplate>();

        /// <summary>
        /// The _types.
        /// </summary>
        private readonly Dictionary<Type, CborTypeTemplate> _types = new Dictionary<Type, CborTypeTemplate>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="CborTypeRegistry"/> class from being created.
        /// </summary>
        private CborTypeRegistry()
        {
            this.ProcessModels();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get template.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="CborTypeTemplate"/>.
        /// </returns>
        public CborTypeTemplate GetTemplate(uint tag)
        {
            CborTypeTemplate template;
            return this._tags.TryGetValue(tag, out template) ? template : null;
        }

        /// <summary>
        /// The get template.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="CborTypeTemplate"/>.
        /// </returns>
        public CborTypeTemplate GetTemplate(Type type)
        {
            CborTypeTemplate template;
            return this._types.TryGetValue(type, out template) ? template : null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get types in namespace.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        /// <param name="ns">
        /// The ns.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string ns)
        {
            return assembly.GetTypes().Where(t => ns.Equals(t.Namespace, StringComparison.Ordinal));
        }

        /// <summary>
        /// The process models.
        /// </summary>
        private void ProcessModels()
        {
            foreach (
                Type modelType in
                    from type in Assembly.GetExecutingAssembly().GetTypes() where this.TypeCheck(type) select type)
            {
                System.Attribute objectAttribute = System.Attribute.GetCustomAttribute(modelType, typeof(CborObject));
                if (!(objectAttribute is CborObject))
                {
                    continue;
                }

                var attr = (CborObject)objectAttribute;
                var template = new CborTypeTemplate
                                   {
                                       tag = attr.Tag, 
                                       type = modelType, 
                                       propertiesByName = new Dictionary<string, CborPropertyTemplate>()
                                   };
                PropertyInfo[] props = modelType.GetProperties();
                var propertyTemplatesList = new List<CborPropertyTemplate>();

                foreach (PropertyInfo propertyInfo in props)
                {
                    var attr1 = (CborField)System.Attribute.GetCustomAttribute(propertyInfo, typeof(CborField));
                    if (attr1 != null)
                    {
                        var propertyTemplate = new CborPropertyTemplate { name = attr1.Name, property = propertyInfo };
                        propertyTemplatesList.Add(propertyTemplate);
                        template.propertiesByName.Add(propertyTemplate.name, propertyTemplate);
                    }
                }

                template.properties = propertyTemplatesList.ToArray();

                this._tags.Add(template.tag, template);
                this._types.Add(template.type, template);
            }
        }

        /// <summary>
        /// The type check.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TypeCheck(Type type)
        {
            if (type == null || type.Namespace == null)
            {
                return false;
            }

            // foreach (var ns in namespaces) {
            if ("DiverseWorlds.Logic.Network.Model".Equals(type.Namespace, StringComparison.Ordinal))
            {
                return true;
            }

            // }
            return false;
        }

        #endregion
    }
}