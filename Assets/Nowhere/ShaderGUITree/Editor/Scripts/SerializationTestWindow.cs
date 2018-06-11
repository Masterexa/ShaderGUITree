using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace ShaderGUITree{


    /* --- WARNING ---

    Create this only in under the 'Editor' directory.

    */

	///<summary>SerializationTestWindow</summary>
	///<remarks>
	///A Editor Window class.
	///</remarks>
	public class SerializationTestWindow : EditorWindow{

        [MenuItem("Window/SerializationTestWindow")]
        static void Init(){
            var wnd = EditorWindow.GetWindow(typeof(SerializationTestWindow));
            wnd.Show();
        }


		#region Instance
			#region Fields
                SxmlRoot    m_root;
			#endregion

			#region Properties
			#endregion

			#region Events
				///<summary>
				///Use this for initialization.
				///</summary>
				void Awake() {
					
				}

                ///<summary>
                ///Use this for draw window.
                ///</summary>
                void OnGUI(){
                    if( GUILayout.Button("Serialize") )
                    {
                        var path = EditorUtility.OpenFilePanel("Open", EditorApplication.applicationPath, "xml");
                        if( !string.IsNullOrEmpty(path) )
                        {
                            var ser = new SxmlSerializer(path);
                            m_root = ser.root;
                        }
                    }
                }

                void DisplayChildRes(ShaderGUITreeElement elem) {

                    
                }
			#endregion

			#region Pipeline
			#endregion

			#region Methods
			#endregion
		#endregion
	}
}