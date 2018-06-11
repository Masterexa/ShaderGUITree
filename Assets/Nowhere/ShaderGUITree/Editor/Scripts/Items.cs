using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using NowhereUnityEditor;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using TrasnformSpace = UnityEngine.Space;

namespace ShaderGUITree.Elements{

    [System.AttributeUsage(AttributeTargets.Class)]
    public class CustomTreeElementAttribute : System.Attribute{
        
        public string elementName       { get; set; }
        public string elementNamespace  { get; set; }


        public void SetNameFromTypeIfNotDefined(Type type)
        {
            if(string.IsNullOrEmpty(elementName) && string.IsNullOrEmpty(elementNamespace))
            {
                elementName         = type.Name;
                elementNamespace    = type.Namespace;
            }
        }
    }



    #region Utilities

        [XmlRoot()]
        public class DefaultOtherPropertyTab : ShaderGUITreeElement {
        
            private bool foldout { get; set; }

            public DefaultOtherPropertyTab() : base() {
                this.isExpanded = false;
                labelContent = new GUIContent("Other Properties");
            }


            public override void OnGUIRow(ShaderGUITreeView treeView) {
                if(this.foldout = NowhereEditorGUILayout.HorizontalSplitter(this.foldout, this.labelContent))
                {
                    foreach(var it in treeView.miscProperties)
                    {
                        treeView.materialEditor.ShaderProperty(it, it.displayName);
                    }
                    EditorGUILayout.Space();
                }
            }
        }

        [XmlRoot()]
        public class DefaultShaderOptions : ShaderGUITreeElement {

            public DefaultShaderOptions() : base() {
                this.isExpanded = false;
                this.labelContent = new GUIContent("Shader Options");
            }

            public override void OnGUIRow(ShaderGUITreeView treeView) {

                if(this.isExpanded = NowhereEditorGUILayout.HorizontalSplitter(this.isExpanded, this.labelContent))
                {
                    var editor = treeView.materialEditor;

                    editor.EnableInstancingField();
                    editor.RenderQueueField();
                    editor.DoubleSidedGIField();
                }
            }
        }

        [XmlRoot()]
        public class LightmapEmissionFlagsProperty : ShaderGUITreeElement {

            public int  labelIndent         { get; set; }
            public bool emissionEnabled     { get; set; }

            public LightmapEmissionFlagsProperty() : base()
            {
                this.labelIndent        = -1;
                this.emissionEnabled    = true;
            }

            public override void OnGUIRow(ShaderGUITreeView treeView) {

                var ind = (labelIndent==-1) ? EditorGUI.indentLevel : labelIndent;

                treeView.materialEditor.LightmapEmissionFlagsProperty(ind, emissionEnabled);
            }
        }

        [XmlRoot()]
        public class EmissionEnabledProperty : ShaderGUITreeElement {

            public override void OnGUIFamily(ShaderGUITreeView treeView) {
                

                if( !this.isVisible)
                {
                    return;
                }

                using(var dis = new EditorGUI.DisabledGroupScope(this.isDisabled))
                {
                    this.isExpanded = treeView.materialEditor.EmissionEnabledProperty();

                    if( !this.isExpanded || !this.hasChildren )
                    {
                        return;
                    }
                    foreach(var it in this.children)
                    {
                        it.OnGUIFamily(treeView);
                    }
                }
            }
        }

        [XmlRoot()]
        public class TreeDebugTab : ShaderGUITreeElement {

            public TreeDebugTab() : base()
            {
                this.isExpanded = false;
            }

            void DrawChildren(ShaderGUITreeElement parent)
            {
                var name = (parent.labelContent!=null) ? parent.labelContent.text : "";
                EditorGUILayout.LabelField("+" + name);

                if( parent.hasChildren )
                {
                    EditorGUI.indentLevel++;
                    foreach(var it in parent.children)
                    {
                        DrawChildren(it);
                    }
                    EditorGUI.indentLevel--;
                }
            }

            public override void OnGUIFamily(ShaderGUITreeView treeView) {
            
                if( !this.isVisible )
                {
                    return;
                }

                if( this.isExpanded = NowhereEditorGUILayout.HorizontalSplitter( this.isExpanded,new GUIContent("Tree Debug") ) )
                {
                    using(var vt = new GUILayout.VerticalScope(GUI.skin.box))
                    {
                        DrawChildren(treeView.rootItem);
                    }
                }
            }
        }
    #endregion

    #region Texture Items
        
        [XmlRoot()]
        public class TextureSingleLine : ShaderGUITreeElement {

            [XmlAttribute()]
            public string textureProperty { get; set; }
            [XmlAttribute()]
            public string extraProperty1 { get; set; }
            [XmlAttribute()]
            public string extraProperty2 { get; set; }

            public override void OnGUIRow(ShaderGUITreeView treeView) {

                var editor  = treeView.materialEditor;
                var tex     = treeView.GetRegisteredProperty(textureProperty);
                var ex1     = treeView.GetRegisteredProperty(extraProperty1);
                var ex2     = treeView.GetRegisteredProperty(extraProperty2);

                editor.TexturePropertySingleLine(this.labelContent, tex, ex1, ex2);
            }
        }

        [XmlRoot()]
        public class TextureTwoLines : ShaderGUITreeElement {

            [XmlAttribute()]
            public string       textureProperty { get; set; }
            [XmlAttribute()]
            public string       extraProperty1 { get; set; }
            [XmlAttribute()]
            public string label2 {
                get {
                    return labelContant2.text;
                }
                set {
                    labelContant2.text = value;
                }
            }
            public GUIContent   labelContant2 { get; set; }
            [XmlAttribute()]
            public string       extraProperty2 { get; set; }



            public TextureTwoLines() : base() {
                this.labelContant2 = new GUIContent();
            }

            public override void OnGUIRow(ShaderGUITreeView treeView) {

                var editor      = treeView.materialEditor;
                var texProp     = treeView.GetRegisteredProperty(this.textureProperty);
                var ex1         = treeView.GetRegisteredProperty(this.extraProperty1);
                var ex2         = treeView.GetRegisteredProperty(this.extraProperty2);

                editor.TexturePropertyTwoLines(this.labelContent, texProp, ex1, this.labelContant2, ex2);
            }
        }

        [XmlRoot()]
        public class TextureWithHDRColor : ShaderGUITreeElement {

            [XmlAttribute()]
            public string   textureProperty { get; set; }
            [XmlAttribute()]
            public string   colorProperty { get; set; }
            [XmlAttribute()]
            public bool     showAlpha { get; set; }

            public TextureWithHDRColor() : base() {
                showAlpha = true;
            }

            public override void OnGUIRow(ShaderGUITreeView treeView) {
                
                var tex = treeView.GetRegisteredProperty(textureProperty);
                var col = treeView.GetRegisteredProperty(colorProperty);

                treeView.materialEditor.TexturePropertyWithHDRColor(this.labelContent, tex, col, null, this.showAlpha);
            }
        }

        [XmlRoot]
        public class TextureScaleOffset : ShaderGUITreeElement {

            [XmlAttribute()]
            public string textureProperty { get; set; }

            public override void OnGUIRow(ShaderGUITreeView treeView) {

                var editor  = treeView.materialEditor;
                var texProp = treeView.GetRegisteredProperty(textureProperty);

                editor.TextureScaleOffsetProperty(texProp);
            }
        }

        [XmlRoot()]
        public class ShaderProperty : ShaderGUITreeElement {

            [XmlAttribute()]
            public string property { get; set; }

            public ShaderProperty() : base() {
                this.labelContent = null;
            }

            public override void OnGUIRow(ShaderGUITreeView treeView) {

                var prop    = treeView.GetRegisteredProperty(property);
                var label   = this.labelContent==null ? prop.displayName : labelContent.text;

                treeView.materialEditor.ShaderProperty(prop, label);
            }
        }
    #endregion


    [XmlRoot()]
    public class Label : ShaderGUITreeElement {

        public GUIStyle style { get; set; }

        public Label() : base()
        {
            style = EditorStyles.label;
        }

        public override void OnGUIRow(ShaderGUITreeView treeView)
        {
            GUILayout.Label(this.labelContent, style);
        }
    }

    [XmlRoot()]
    public class Foldout : ShaderGUITreeElement {

        public override void OnGUIRow(ShaderGUITreeView treeView)
        {
            this.isExpanded = EditorGUILayout.Foldout(this.isExpanded, this.labelContent);
        }

        public override void OnGUIFamily(ShaderGUITreeView treeView) {
            
            if( !this.isVisible )
            {
                return;
            }

            using(var dis = new EditorGUI.DisabledGroupScope(this.isDisabled))
            {
                OnGUIRow(treeView);
                if( this.isExpanded && hasChildren )
                {
                    foreach(var it in this.children)
                    {
                        it.OnGUIFamily(treeView);
                    }
                }
            }
        }
    }

    [CustomTreeElement()]
    [XmlRoot("HorizontalSplitter")]
    public class HorizontalSplitter : ShaderGUITreeElement {

        public HorizontalSplitter() : base()
        {
            this.isExpanded = false;
        }

        public override void OnGUIRow(ShaderGUITreeView treeView)
        {
            this.isExpanded = NowhereEditorGUILayout.HorizontalSplitter(this.isExpanded, this.labelContent);
        }

        public override void OnGUIFamily(ShaderGUITreeView treeView) {
            
            if( !this.isVisible )
            {
                return;
            }

            using(var dis = new EditorGUI.DisabledGroupScope(this.isDisabled))
            {
                OnGUIRow(treeView);
                if( this.isExpanded && hasChildren )
                {
                    foreach(var it in this.children)
                    {
                        it.OnGUIFamily(treeView);
                    }
                }
            }
        }
    }

    [XmlRoot()]
    public class ChangeScope : ShaderGUITreeElement {

        public delegate void Callback(EventData data);

        public class EventData : IDisposable{
            public ChangeScope  item    { get; private set; }
            public bool             changed { get; private set; }

            public EventData(ChangeScope item, bool changed)
            {
                this.item       = item;
                this.changed    = changed;
            }

            public void Dispose() {
            }
        }


        #region Fields
            public Callback afterCallback { get; set; }
        #endregion

        #region Events
            public override void OnGUIFamily(ShaderGUITreeView treeView) {
               if( !this.isVisible)
                {
                    return;
                }

                using(var dis = new EditorGUI.DisabledGroupScope(this.isDisabled))
                {
                    if( !this.hasChildren )
                    {
                        return;
                    }
                    using(var chk = new EditorGUI.ChangeCheckScope())
                    {
                        foreach(var it in this.children)
                        {
                            it.OnGUIFamily(treeView);
                        }

                        var data = new EventData(this, chk.changed);
                            afterCallback.Invoke(data);
                        data.Dispose();
                    }
                }
            }
        #endregion
    }

    [XmlRoot()]
    public class SpaceLayout : ShaderGUITreeElement {

        [XmlAttribute()]
        public bool flexibleSpace { get; set; }

        public override void OnGUIRow(ShaderGUITreeView treeView)
        {
            if( flexibleSpace )
            {
                GUILayout.FlexibleSpace();
            }
            else{
                EditorGUILayout.Space();
            }
        }
    }
}
