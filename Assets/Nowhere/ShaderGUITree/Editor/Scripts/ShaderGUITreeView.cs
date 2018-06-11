using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace ShaderGUITree{

    [XmlRoot]
    public class ShaderGUITreeElement {
        #region Fields
            [XmlAttribute()]
            public string                       name            { get; set; }
            
            [XmlAttribute()]
            public string label {
                get {
                    return labelContent.text;
                }
                set {
                    labelContent.text = value;
                }
            }
            public GUIContent                   labelContent    { get; set; }
            
            [XmlElement(Type =typeof(ShaderGUITreeElement))]
            public List<ShaderGUITreeElement>   children        { get; set; }
            public bool                         hasChildren     {get { return children!=null && children.Count!=0; } }
            public ShaderGUITreeElement         parent          { get; set; }

            public bool isVisible { get; set; }
            public bool isExpanded { get; set; }
            public bool isDisabled { get; set; }
        #endregion

        #region Events
            public ShaderGUITreeElement() {
                this.isVisible  = true;
                this.isExpanded = true;
                this.labelContent      = GUIContent.none;
                this.children   = new List<ShaderGUITree.ShaderGUITreeElement>();
            }

            public virtual void OnGUIRow(ShaderGUITreeView treeView) {}

            public virtual void OnGUIFamily(ShaderGUITreeView treeView){

                if( !this.isVisible)
                {
                    return;
                }

                using(var dis = new EditorGUI.DisabledGroupScope(this.isDisabled))
                {
                    this.OnGUIRow(treeView);

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
        #endregion

        #region Child
            public void AddChild(ShaderGUITreeElement child)
            {
                this.children.Add(child);
                child.parent = this;
            }
        #endregion
    }

   

    ///<summary>ShaderGUITreeView</summary>
    ///<remarks>
    ///A scriptable object class.
    ///</remarks>
    public abstract class ShaderGUITreeView : ScriptableObject{

		#region Instance
			#region Statements
                public ShaderGUI                            shaderGUI               { get; private set; }
                public MaterialEditor                       materialEditor          { get; private set; }
                public MaterialProperty[]                   materialProperties      { get; private set; }
                public HashSet<MaterialProperty>            miscProperties          { get; private set; }
                public Dictionary<string,MaterialProperty>  registeredProperties    { get; set; }

                public ShaderGUITreeElement rootItem { get; private set; }
			#endregion

			#region Events
				protected abstract ShaderGUITreeElement OnBuildRoot();
                protected virtual void OnFirstApply() {}
                protected virtual void OnAwake() { }
                protected virtual void OnPreGUI() { }
                protected virtual void OnPostGUI(bool changed) {}
            #endregion

            #region Events
                private void Awake() {
                    registeredProperties    = new Dictionary<string, MaterialProperty>();
                    miscProperties          = new HashSet<MaterialProperty>();
                    OnAwake();
                }


                public void DrawGUI(ShaderGUI shaderGUI, MaterialEditor materialEditor, MaterialProperty[] properties) {

                    if( shaderGUI != this.shaderGUI )
                    {
                        this.shaderGUI       = shaderGUI;
                        this.materialEditor  = materialEditor;
                        this.materialProperties     = properties;

                        this.miscProperties.Clear();
                        foreach(var it in properties)
                        {
                            this.miscProperties.Add(it);
                        }
                        this.OnFirstApply();
                    }

                    bool changed = false;

                    this.OnPreGUI();
                    using(var chk = new EditorGUI.ChangeCheckScope())
                    {
                        if( this.rootItem!=null )
                        {
                            this.rootItem.OnGUIFamily(this);
                        }
                        changed = chk.changed;
                    }
                    this.OnPostGUI(changed);
                }
                
            #endregion

            #region Property Methods
                public void Reload() {
                    this.rootItem = OnBuildRoot();
                }

                public MaterialProperty RegisterProperty(string name, bool propertyIsMandatory=true){
                    
                    MaterialProperty prop = null;
                    
                    if( !registeredProperties.ContainsKey(name) )
                    {
                        registeredProperties.Add(name,null);
                    }


                    prop = materialProperties.First( (it)=>(it.name == name) );
                    if( prop==null && propertyIsMandatory )
                    {
                        throw new System.ArgumentException( string.Format("property \"{0}\" is not found", name) );
                    }

                    registeredProperties[name] = prop;
                    miscProperties.Remove(prop);

                    return prop;
                }

                public MaterialProperty GetRegisteredProperty(string name)
                {
                    if( string.IsNullOrEmpty(name) )
                    {
                        return null;
                    }
                    MaterialProperty prop;
                    registeredProperties.TryGetValue(name, out prop);
                    return prop;
                }
            #endregion
        #endregion
    }
}