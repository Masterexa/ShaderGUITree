using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Reflection;
using System.Linq;

namespace ShaderGUITree {

    [Elements.CustomTreeElement()]
    [XmlRoot("SXML")]
    public class SxmlRoot : ShaderGUITreeElement{
    }


    public class SxmlSerializer {

        public SxmlRoot root { get; set; }


        static XmlAttributeOverrides s_overrides;

        static SxmlSerializer() {
            s_overrides     = new XmlAttributeOverrides();
            var attributes  = new XmlAttributes();

            var info            = typeof(ShaderGUITreeElement).GetMember("children")[0];
            var originalAttrs   = Attribute.GetCustomAttributes(info, typeof(XmlElementAttribute));
            foreach(var it in originalAttrs)
            {
                attributes.XmlElements.Add(it as XmlElementAttribute);
            }

            var assembly = Assembly.GetAssembly(typeof(Elements.CustomTreeElementAttribute));
            var customAttributes = assembly.GetTypes()
                .Where( (it)=>(it.GetCustomAttributes(typeof(Elements.CustomTreeElementAttribute),false).Count()!=0 ) )
                .ToDictionary(it=>it, it=>it.GetCustomAttributes(typeof(Elements.CustomTreeElementAttribute),false)[0] as Elements.CustomTreeElementAttribute);

            var classes = Assembly.GetAssembly(typeof(ShaderGUITreeElement)).GetTypes()
                .Where( (it)=>it.IsSubclassOf(typeof(ShaderGUITreeElement)) )
            ;


            foreach(var it in customAttributes)
            {
                it.Value.SetNameFromTypeIfNotDefined(it.Key);
                attributes.XmlElements.Add(new XmlElementAttribute(it.Key));
            }
            foreach(var it in classes)
            {
                s_overrides.Add(it, "children", attributes);
            }
        }

        public SxmlSerializer(string path)
        {
            using(var fs = new StreamReader(path))
            {
                var serializer  = new XmlSerializer(typeof(SxmlRoot), s_overrides);
                root            = (SxmlRoot)serializer.Deserialize(fs);
            }
        }
    }
}
