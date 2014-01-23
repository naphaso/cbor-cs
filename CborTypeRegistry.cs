using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Reflection;
using Assets.DiverseWorlds.Cbor.Attribute;
using Assets.DiverseWorlds.Cbor.Exception;

namespace Assets.DiverseWorlds.Cbor {

    public class CborPropertyTemplate {
        public string name;
        public PropertyInfo property;


    }

    public class CborTypeTemplate {
        public CborPropertyTemplate[] properties;
        public Dictionary<string, CborPropertyTemplate> propertiesByName;
        public uint tag;
        public Type type;

        public void SetValue(object obj, string property, object value) {
            if(property == null) {
                throw new CborException("invalid object representation");
            }

            

            CborPropertyTemplate propertyTemplate;
            if(!propertiesByName.TryGetValue(property, out propertyTemplate)) {
                throw new CborException("type " + type.Name + " not contains property " + property);
            }

            Type ptype = propertyTemplate.property.PropertyType;
            //bool nullable = false;
            if (ptype.IsGenericType && ptype.GetGenericTypeDefinition() == typeof (Nullable<>)) {
                ptype = ptype.GetGenericArguments()[0];
            }

            if (value is int){
                if(ptype == typeof (UInt64)) { // TODO: fix typecasts
                    propertyTemplate.property.SetValue(obj, (ulong)(int)value, null);
                    return;
                }

                if (ptype.IsEnum) {
                    propertyTemplate.property.SetValue(obj, Enum.ToObject(ptype, value), null); 

                    return;
                }
            }

            /*if (value is IList && propertyTemplate.property.PropertyType.IsArray) {
                IList list = value as IList;
                Array array = (Array) Activator.CreateInstance(propertyTemplate.property.PropertyType.GetElementType(),
                    list.Count);
                for (int i = 0; i < list.Count; i++) {
                    array.SetValue(list[i], i);
                }
                propertyTemplate.property.SetValue(obj, array, null);
                return;
            }*/

            propertyTemplate.property.SetValue(obj, value, null);
        }

        public Type GetPropertyType(string property) {
            return propertiesByName[property].property.PropertyType;
        }
    }

    public class CborTypeRegistry {
        public static readonly CborTypeRegistry Instance = new CborTypeRegistry();

        private readonly Dictionary<uint, CborTypeTemplate> _tags = new Dictionary<uint, CborTypeTemplate>();
        private readonly Dictionary<Type, CborTypeTemplate> _types = new Dictionary<Type, CborTypeTemplate>();

        private CborTypeRegistry() {
            ProcessModels();
        }

        public CborTypeTemplate GetTemplate(uint tag) {
            CborTypeTemplate template;
            return _tags.TryGetValue(tag, out template) ? template : null;
        }

        public CborTypeTemplate GetTemplate(Type type) {
            CborTypeTemplate template;
            return _types.TryGetValue(type, out template) ? template : null;
        }

        private void ProcessModels() {
            foreach(var modelType in from type in Assembly.GetExecutingAssembly().GetTypes()
                                     where type.Namespace != null && type.Namespace.Equals("TestServer.Model", StringComparison.Ordinal)
                                     select type) {
                var objectAttribute = System.Attribute.GetCustomAttribute(modelType, typeof(CborObject));
                if(!(objectAttribute is CborObject)) {
                    continue;
                }

                var attr = (CborObject) objectAttribute;
                var template = new CborTypeTemplate {tag = attr.Tag, type = modelType, propertiesByName = new Dictionary<string, CborPropertyTemplate>()};
                var props = modelType.GetProperties();
                var propertyTemplatesList = new List<CborPropertyTemplate>();
                foreach(var propertyTemplate in
                    from propertyInfo in props
                    let fieldAttribute = System.Attribute.GetCustomAttribute(propertyInfo, typeof(CborField))
                    where fieldAttribute is CborField
                    let fieldAttr = (CborField) fieldAttribute
                    select new CborPropertyTemplate {name = fieldAttr.Name, property = propertyInfo}) {
                    propertyTemplatesList.Add(propertyTemplate);
                    template.propertiesByName.Add(propertyTemplate.name, propertyTemplate);
                }

                template.properties = propertyTemplatesList.ToArray();

                _tags.Add(template.tag, template);
                _types.Add(template.type, template);
            }
        }

        private IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string ns) {
            return assembly.GetTypes().Where(t => ns.Equals(t.Namespace, StringComparison.Ordinal));
        }
    }
}