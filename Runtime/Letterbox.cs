using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extelen.UI {

	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	public class Letterbox : MonoBehaviour {
			
		//Params
					
			//Non Static
			[Header("Letterbox References")]
			[SerializeField] private RectTransform m_letterboxRect = null;
			[SerializeField] private RectTransform m_topBarRect = null;
			[SerializeField] private RectTransform m_bottomBarRect = null;

			[Header("Letterbox Values")]
			[SerializeField] private bool m_defaultActive = false;
			[SerializeField] private bool m_useStandard = true;
			[SerializeField] [Range(0.075f, 0.25f)] private float m_barViewportSize = 0.1f;

			[Header("Letterbox Animation")]
			[SerializeField] private bool m_unscaledTime = true;
			[SerializeField] private AnimationCurve m_activationSmoothness = 
				new AnimationCurve(new Keyframe[2] {

				new Keyframe(0, 0),
				new Keyframe(1, 1),
				});
			
			[SerializeField] private float m_activationTime = 0.25f;

			public float AspectRatio {

				get => m_letterboxRect.rect.width / m_letterboxRect.rect.height;
				}
			public float StandardViewportBarSize {
				
				get {
					
					float m_standard = 21f/9f;
					return Mathf.Clamp((m_letterboxRect.rect.height / m_standard) / (m_letterboxRect.rect.width * 2), 0.075f, 0.25f); 
					}
				}

			private Coroutine m_letterboxRoutine = null;
			private bool m_letterboxActive = false;
			private float m_letterboxValue = 0;

		//Methods
		
			//Non Static

				//MonoBehaviour Methods
				private void OnValidate() {

					if (m_letterboxRect == null) TryGetComponent<RectTransform>(out m_letterboxRect);
					m_barViewportSize = Mathf.Clamp(m_barViewportSize, 0.05f, 0.25f);

					UpdateBarScale(m_defaultActive ? 1 : 0);
					}
				private void Start() {

					m_letterboxActive = m_defaultActive;
					m_letterboxValue = m_defaultActive ? 1 : 0;

					UpdateBarAnchors();
					UpdateBarScale(m_defaultActive ? 1 : 0);
					}
				private void Update() {
						
					if (!Application.IsPlaying(gameObject)) {

						if (m_bottomBarRect && (m_topBarRect || m_letterboxRect)) {
							
							UpdateBarAnchors();
							}
						}
					}
				
				//Change Anchors
				private void UpdateBarAnchors() => UpdateBarAnchors(m_useStandard ? StandardViewportBarSize : m_barViewportSize);
				private void UpdateBarAnchors(float barViewportSize) {

					if (m_topBarRect != null) {
						
						m_topBarRect.anchorMin = new Vector2(0, 1 - barViewportSize);
						m_topBarRect.anchorMax = Vector2.one;
						}
					if (m_bottomBarRect != null) {
						
						m_bottomBarRect.anchorMin = Vector2.zero;
						m_bottomBarRect.anchorMax = new Vector2(1, barViewportSize);
						}
					}
					
				public void ChangeBarSize(bool cinematographicStandard) {
					
					m_useStandard = cinematographicStandard;

					if (cinematographicStandard) {
						
						UpdateBarAnchors(StandardViewportBarSize);
						}

					else {

						UpdateBarAnchors(m_barViewportSize);
						}
					}
				public void ChangeBarSize(float barViewportSize) {

					m_useStandard = false;
					UpdateBarAnchors(barViewportSize);
					}

				//Bars Scale
				public void ActiveLetterbox() => SetLetterboxActive(true);
				public void DeactiveLetterbox() => SetLetterboxActive(false);
				public void ToggleLetterbox() => SetLetterboxActive(!m_letterboxActive);
				public void ToggleLetterbox(bool overwrite) => SetLetterboxActive(!m_letterboxActive, overwrite);
					
				public void SetLetterboxActive(bool active) => SetLetterboxActive(active, false);
				public void SetLetterboxActive(bool active, bool overwrite) {

					if (m_letterboxActive == active && !overwrite) return;
					m_letterboxActive = active;

					if (active) RunRoutine(true);
					else RunRoutine(false);
					}

				private void UpdateBarScale(float value) {
					
					value = Mathf.Clamp01(value);
					m_letterboxValue = value;

					Vector3 m_scale = new Vector3(1, value, 1);
					if (m_topBarRect != null) m_topBarRect.localScale = m_scale;
					if (m_bottomBarRect != null) m_bottomBarRect.localScale = m_scale;
					}

				private void RunRoutine(bool active) {
					
					if (m_letterboxRoutine != null) StopCoroutine(m_letterboxRoutine);
					m_letterboxRoutine = StartCoroutine(LetterboxRoutine(active));
					}	

		//Coroutines
		private IEnumerator LetterboxRoutine(bool active) {
			
			float m_startValue = m_letterboxValue;
			float m_finalValue = active ? 1 : 0;

			for(float i = 0; i < m_activationTime; i += m_unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) {
				
				UpdateBarScale(Mathf.Lerp(m_startValue, m_finalValue, m_activationSmoothness.Evaluate(i / m_activationTime)));
				yield return null;
				}

			UpdateBarScale(m_finalValue);
			}
		}
	}