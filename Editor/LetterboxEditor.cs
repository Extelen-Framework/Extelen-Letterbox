using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Extelen.UI;

namespace Extelen.Editor.UI {

	[CustomEditor(typeof(Letterbox))]
	public class LetterboxEditor : UnityEditor.Editor {
		
		//Set Params
		private SerializedProperty m_letterboxRect = null;
		private SerializedProperty m_topBarRect = null;
		private SerializedProperty m_bottomBarRect = null;

		private SerializedProperty m_defaultActive = null;
		private SerializedProperty m_useStandard = null;
		private SerializedProperty m_barViewportSize = null;
					
		private SerializedProperty m_unscaledTime = null;
		private SerializedProperty m_activationSmoothness = null;
		private SerializedProperty m_activationTime = null;

		//Methods
		private void OnEnable() {

			m_letterboxRect = serializedObject.FindProperty("m_letterboxRect");
			m_topBarRect = serializedObject.FindProperty("m_topBarRect");
			m_bottomBarRect = serializedObject.FindProperty("m_bottomBarRect");

			m_defaultActive = serializedObject.FindProperty("m_defaultActive");
			m_useStandard = serializedObject.FindProperty("m_useStandard");
			m_barViewportSize = serializedObject.FindProperty("m_barViewportSize");

			m_unscaledTime = serializedObject.FindProperty("m_unscaledTime");
			m_activationSmoothness = serializedObject.FindProperty("m_activationSmoothness");
			m_activationTime = serializedObject.FindProperty("m_activationTime");
			}

		public override void OnInspectorGUI() {
			
			serializedObject.Update();
			
			EditorGUILayout.PropertyField(m_letterboxRect);

			bool m_canvasIsAssigned = m_letterboxRect.objectReferenceValue;

			if (m_canvasIsAssigned) {

				EditorGUILayout.PropertyField(m_topBarRect);
				EditorGUILayout.PropertyField(m_bottomBarRect);

				bool m_topBarIsAssigned = m_topBarRect.objectReferenceValue;
				bool m_bottomBarIsAssigned = m_bottomBarRect.objectReferenceValue;

				if (m_topBarIsAssigned || m_bottomBarIsAssigned) {

					if ((m_topBarIsAssigned && !m_bottomBarIsAssigned) || (!m_topBarIsAssigned && m_bottomBarIsAssigned)) {

						EditorGUILayout.HelpBox("It is recommended that you have the two bars assigned.", MessageType.Warning);
						}

					EditorGUILayout.PropertyField(m_defaultActive);
					EditorGUILayout.PropertyField(m_useStandard);
						
					if (!m_useStandard.boolValue) {

						EditorGUILayout.PropertyField(m_barViewportSize);
						}
					else {

						Letterbox m_letterbox = (Letterbox) target;
						float m_aspectRatio = m_letterbox.AspectRatio;
						string m_aspectRatioString = m_letterbox.AspectRatio.ToString("N2");

						if (m_aspectRatioString == "1.78") m_aspectRatioString = m_aspectRatioString + " (16:9)";
						if (m_aspectRatioString == "1.32") m_aspectRatioString = m_aspectRatioString + " (4:3)";

						float m_viewPortSize = m_letterbox.StandardViewportBarSize;
						string m_viewPortSizeString = m_viewPortSize.ToString("N3");

						string m_clampMessage = ".";

						if (m_viewPortSize == 0.075f) m_clampMessage = "(The viewport size is clamped to 0.075).";
						else if (m_viewPortSize == 0.25f) m_clampMessage = "(The viewport size is clamped to 0.25).";

						EditorGUILayout.HelpBox("The cinematic standard aspect ratio is 2.35 (21:9) and the current aspect ratio is " + m_aspectRatioString + ", this will make the size of each letterbox be "+ m_viewPortSizeString +" viewport units " + m_clampMessage, MessageType.Info);
						}	

					EditorGUILayout.PropertyField(m_unscaledTime);
					EditorGUILayout.PropertyField(m_activationSmoothness);
					EditorGUILayout.PropertyField(m_activationTime);
					}

				else {

					EditorGUILayout.HelpBox("You do not have any bar assigned, to use the letterbox you need to have at least one.", MessageType.Error);
					}
				}
			
			else {
				
				EditorGUILayout.HelpBox("Canvas property is not assigned.", MessageType.Error);
				}

			serializedObject.ApplyModifiedProperties();
			}	
		}
	}