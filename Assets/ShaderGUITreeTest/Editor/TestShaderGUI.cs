using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ShaderGUITree.Elements;

namespace ShaderGUITree {

    public enum RenderingMode {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public enum DisplacementMode {
        None,
        ParallaxMapping,
        Tessellation
    }


    public class TestShaderTreeView : ShaderGUITreeView {

        #region Events
            protected override void OnAwake() {
                Reload();
            }

            protected override ShaderGUITreeElement OnBuildRoot() {
                
                var root = new ShaderGUITreeElement();
                

                // Lit Area
                var lit = new HorizontalSplitter { labelContent=new GUIContent("Lit") };
                root.AddChild(lit);
                {
                    // Rendering Mode
                    lit.AddChild( new ShaderProperty {property="_Mode"} );
                    lit.AddChild( new SpaceLayout() );

                    // Base Map Label
                    lit.AddChild( new Label {labelContent=new GUIContent("Main Maps"), style=EditorStyles.boldLabel} );

                    // Albedo
                    lit.AddChild( new TextureSingleLine
                    {
                        labelContent           = new GUIContent("Albedo"),
                        textureProperty         = "_MainTex",
                        extraProperty1  = "_Color"
                    });

                    // Metallic & Smoothness
                    lit.AddChild(new TextureTwoLines
                    {
                        labelContent           = new GUIContent("Metallic"),
                        textureProperty         = "_GlossMap",
                        extraProperty1  = "_Metallic",
                        labelContant2          = new GUIContent("Smoothness"),
                        extraProperty2  = "_Glossiness"
                    });

                    // Occlusion
                    lit.AddChild(new TextureSingleLine
                    {
                        labelContent           = new GUIContent("Occlusion"),
                        textureProperty         = "_OcclusionMap",
                        extraProperty1  = "_OcclusionStrength"
                    });


                    // Emission
                    var emissionArea = new EmissionEnabledProperty();
                    lit.AddChild(emissionArea);
                    {
                        emissionArea.AddChild( new TextureWithHDRColor
                        {
                            labelContent           = new GUIContent("Color"),
                            colorProperty   = "_EmissionColor",
                            textureProperty = "_EmissionMap"
                        });
                        
                        emissionArea.AddChild( new LightmapEmissionFlagsProperty
                        {
                            labelIndent = MaterialEditor.kMiniTextureFieldLabelIndentLevel
                        });
                    }

                    // Misc
                    lit.AddChild(new TextureScaleOffset { textureProperty="_MainTex"});
                    lit.AddChild(new SpaceLayout());

                    // Forward Rendering options
                }


                // Displacement Area
                var displacement = new HorizontalSplitter { labelContent = new GUIContent("Displacement")};
                root.AddChild(displacement);
                {
                    displacement.AddChild(new ShaderProperty {property="_DisplacementMode"});
                    displacement.AddChild(new SpaceLayout());

                    // Basis
                    displacement.AddChild(new Label { labelContent=new GUIContent("Basis"), style=EditorStyles.boldLabel });
                    displacement.AddChild(new TextureSingleLine
                    {
                        labelContent            = new GUIContent("Normal Map"),
                        textureProperty          = "_BumpMap",
                        extraProperty1   = "_BumpScale"
                    });
                    displacement.AddChild(new TextureSingleLine
                    {
                        labelContent           = new GUIContent("Height Map"),
                        textureProperty         = "_ParallaxMap",
                        extraProperty1  = "_Parallax"
                    });
                    displacement.AddChild( new SpaceLayout() );

                    // Tessellation
                    displacement.AddChild(new Label
                    {
                        labelContent   = new GUIContent("Tessellation"),
                        style   = EditorStyles.boldLabel
                    });
                    displacement.AddChild(new ShaderProperty {property="_Tessellation"});
                    displacement.AddChild(new ShaderProperty {property="_TessellationEdge"});
                    displacement.AddChild(new ShaderProperty {property="_TessellationPhong"});

                    displacement.AddChild( new SpaceLayout() );
                }


                root.AddChild(new DefaultOtherPropertyTab());
                root.AddChild(new DefaultShaderOptions());
                root.AddChild(new TreeDebugTab());

                return root;
            }

            protected override void OnFirstApply() {
                
                RegisterProperty("_Mode");
                RegisterProperty("_MainTex");
                RegisterProperty("_Color");
                RegisterProperty("_Glossiness");
                RegisterProperty("_Metallic");
                RegisterProperty("_GlossMap");
                RegisterProperty("_OcclusionStrength");
                RegisterProperty("_OcclusionMap");
                RegisterProperty("_EmissionMap");
                RegisterProperty("_EmissionColor");

                // Displacement
                RegisterProperty("_DisplacementMode");
                RegisterProperty("_BumpScale");
                RegisterProperty("_BumpMap");
                RegisterProperty("_Parallax");
                RegisterProperty("_ParallaxMap");
                RegisterProperty("_Tessellation");
                RegisterProperty("_TessellationEdge");
                RegisterProperty("_TessellationPhong");
            }
        #endregion
    }
    

    public class TestShaderGUI : ShaderGUI {

        static TestShaderTreeView s_view;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {

            if( !s_view )
            {
                s_view = ScriptableObject.CreateInstance<TestShaderTreeView>();
            }
            s_view.DrawGUI(this, materialEditor, properties);
        }
    }
}