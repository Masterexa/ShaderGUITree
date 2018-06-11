using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NowhereUnityEditor {

    public static class NowhereEditorGUILayout {

        public class HorizontalSplitterScope : GUI.Scope {

            int     m_prevIndent;
            public bool foldout { get; set; }

            public HorizontalSplitterScope(bool foldout, GUIContent content) {
                m_prevIndent = EditorGUI.indentLevel;
                this.foldout = HorizontalSplitter(foldout, content);
            }

            protected override void CloseScope() {
                EditorGUI.indentLevel = m_prevIndent;
            }
        }


        #region Class
            #region Styles
                static GUIStyle s_horizontalSplitterStyle = null;
                static GUIStyle horizontalSplitterStyle {
                    get {
                        if( s_horizontalSplitterStyle==null )
                        {
                            var style                   = new GUIStyle("ProjectBrowserHeaderBgMiddle");
                            style.font                  = EditorStyles.boldFont;
                            style.alignment             = TextAnchor.MiddleLeft;
                            style.border                = new RectOffset(15,7,4,4);
                            style.fixedHeight           = 20f;
                            style.margin                = new RectOffset(-2,-2,-3,-3);
                            s_horizontalSplitterStyle   = style;
                        }
                        return s_horizontalSplitterStyle;
                    }
                }
            #endregion

            #region Methods
                public static bool HorizontalSplitter(bool foldout, GUIContent content)
                {
                    var     style       = horizontalSplitterStyle;
                    var     rc          = GUILayoutUtility.GetRect(16f, 22f, style);
                    float   indentW     = 32f;
                    style.contentOffset = new Vector2(indentW, -2f);
                    rc.xMin             = 0f;
                    rc.xMax             += 4f;
                    // Draw
                    GUI.Box(rc, content, style);


                    var ev          = Event.current;
                    var toggleRc    = new Rect(indentW-13f-4f, rc.y+2f, 13f, 13f);
                    var isMouseOnRc = rc.Contains(ev.mousePosition);
                    // Draw triangle
                    if( ev.type==EventType.Repaint )
                    {
                        EditorStyles.foldout.Draw(toggleRc, false, false, foldout, false);
                    }

                    if( ev.type==EventType.MouseUp && isMouseOnRc )
                    {
                        foldout = !foldout;
                        ev.Use();
                    }

                    return foldout;
                }
            #endregion
        #endregion
    }
}