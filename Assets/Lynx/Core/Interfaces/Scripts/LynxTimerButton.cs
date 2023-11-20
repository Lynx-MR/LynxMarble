//   ==============================================================================
//   | Lynx Interfaces (2023)                                                     |
//   |======================================                                      |
//   | LynxTimerButton Script                                                     |
//   | Script to set a UI element as Timer Button.                                |
//   ==============================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lynx.UI
{
    public class LynxTimerButton : Button
    {
        #region INSPECTOR VARIABLES

        // Button Parameters
        [SerializeField] private UnityEvent OnPress;
        [SerializeField] private UnityEvent OnUnpress;
        [SerializeField] public UnityEvent OnTimerPress;
        [SerializeField] private bool m_disableSelectState = true;
        [SerializeField] private Graphic[] m_secondaryTargetGraphic;

        // Timer Button Parameters
        [SerializeField] private Image m_timerImage = null;
        [SerializeField] private float m_deltaTime = 2.0f;
        [SerializeField] private ButtonAnimation m_animation = new ButtonAnimation();

        //theme button 
        [SerializeField] private bool useTheme = false;
        #endregion

        #region PRIVATE VARIABLES

        private bool m_isRunning = false; // Avoid multiple press or unpress making the object in unstable state.
        private bool m_isCurrentlyPressed = false; // Status of the current object.
        private bool m_timerIsRunning = false; // Status of the timer.
        
        private IEnumerator timerCoroutine = null; // Timer coroutine reference.

        #endregion

        #region UNITY API


        protected override void Awake()
        {
            if (useTheme && LynxThemeManager.Instance)
            {
                LynxThemeManager.Instance.ThemeUpdateEvent += this.SetThemeColors;
                SetThemeColors();
            }
        }

        // OnSelect is called when the selectable UI object is selected.
        public override void OnSelect(BaseEventData eventData)
        {
            // State Select can affect the expected behaviour of the button.
            // It is natively deactivated on our buttons.
            // But can be reactivated by unchecking disableSelectState

            if (m_disableSelectState)
            {
                base.OnDeselect(eventData);
            }
            else
            {
                base.OnSelect(eventData);
            }
        }

        // OnPointerDown is called when the mouse is clicked over this selectable UI object.
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            base.OnPointerDown(eventData);

            if(!m_isRunning && !m_isCurrentlyPressed)
            {
                m_isRunning = true;
                StartCoroutine(ButtonAnimationMethods.PressingAnimationCoroutine(m_animation, this.transform, CallbackStopRunning));
                m_isCurrentlyPressed = true;
            }

            if (!m_timerIsRunning)
            {
                m_timerIsRunning = true;
                timerCoroutine = TimerAnimationCoroutine();
                StartCoroutine(timerCoroutine);
            }

            if (LynxThemeManager.Instance.currentTheme.CallAudioOnPress(out AudioClip clip) && useTheme)
            {
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
            }
        }

        // OnPointerUp is called when the mouse click on this selectable UI object is released.
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!IsInteractable()) return;

            base.OnPointerUp(eventData);

            if (m_isCurrentlyPressed)
            {
                m_isRunning = true;
                StartCoroutine(ButtonAnimationMethods.UnpressingAnimationCoroutine(m_animation, this.transform, CallbackStopRunning));
                m_isCurrentlyPressed = false;
            }

            m_timerIsRunning = false;
            StopCoroutine(timerCoroutine);

            m_timerImage.fillAmount = 0.0f;

            if (LynxThemeManager.Instance.currentTheme.CallOnAudioUnpress(out AudioClip clip) && useTheme)
            {
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
            }
        }

        //DoStateTranistion is called on every state change to manage graphic modification
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (transition != Transition.ColorTint) return;

            for (int i = 0; i < m_secondaryTargetGraphic.Length; ++i)
            {
                Color tintColor;
                switch (state)
                {
                    case SelectionState.Normal:
                        tintColor = colors.normalColor;
                        break;
                    case SelectionState.Highlighted:
                        tintColor = colors.highlightedColor;
                        break;
                    case SelectionState.Pressed:
                        tintColor = colors.pressedColor;
                        break;
                    case SelectionState.Selected:
                        tintColor = colors.selectedColor;
                        break;
                    case SelectionState.Disabled:
                        tintColor = colors.disabledColor;
                        break;
                    default:
                        tintColor = Color.black;
                        break;
                }
                m_secondaryTargetGraphic[i].CrossFadeColor(tintColor, instant ? 0f : colors.fadeDuration, true, true);
            }
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// CallbackStopRunning is called when a button animation coroutine is complete.
        /// </summary>
        /// <param name="state">True to call OnUnpress, false to call OnPress.</param>
        private void CallbackStopRunning(bool state)
        {
            m_isRunning = false;

            if (state)
            {
                OnUnpress.Invoke();
            }
            else
            {
                OnPress.Invoke();
            }
        }

        #endregion

        #region ANIMATION COROUTINES

        /// <summary>
        /// Start this coroutine to launch the fill animation.
        /// </summary>
        private IEnumerator TimerAnimationCoroutine()
        {
            float elapsedTime = 0.0f;

            while (elapsedTime < m_deltaTime)
            {
                elapsedTime += Time.deltaTime;
                m_timerImage.fillAmount = elapsedTime / m_deltaTime;
                yield return new WaitForEndOfFrame();
            }

            m_timerImage.fillAmount = 0.0f;


            OnTimerPress.Invoke();

            m_timerIsRunning = false;
        }

        #endregion

        #region THEME MANAGING

        /// <summary>
        /// change the colorblock of a button to match the selected theme
        /// </summary>
        public void SetThemeColors()
        {
            if (LynxThemeManager.Instance == null)
                return;
            colors = LynxThemeManager.Instance.currentTheme.selectableColors;
        }

        /// <summary>
        /// Define if this element should use the theme manager
        /// </summary>
        /// <param name="enable">True to use theme manager</param>
        public void SetUseTheme(bool enable = true)
        {
            useTheme = enable;
        }

        /// <summary>
        /// Check if current element is using theme manager.
        /// </summary>
        /// <returns>True if this element use theme manager.</returns>
        public bool IsUsingTheme()
        {
            return useTheme;
        }
        #endregion
    }
}
