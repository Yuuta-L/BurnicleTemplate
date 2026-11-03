using BurnicleFramework.Runtime;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Template
{
    public class LaunchForm : MonoBehaviour
    {
        public Image background;
        public TextMeshProUGUI versionText;
        public Image title;
        public Slider progress;
        public TextMeshProUGUI tipText;
        public Button closeButton;
        public GameObject launchCamera;
        public GameObject orna;
        public GameObject startGameButton;

        private bool _finished;
        private Tweener _progressTweener;
        public event System.Action OnClosed;

        public static LaunchForm I;
        private void Awake()
        {
            I = this;
            _finished = false;
            versionText.text = $"v{RuntimeCrossPlatformSettings.Load().Version}";
            AdaptBackground();
            // 初始隐藏进度条
            orna.SetActive(false);
            progress.gameObject.SetActive(false);
        }

        private void AdaptBackground()
        {
            // cover 模式：取宽高比最大值，确保背景在任何屏幕比例下都能完全覆盖不留空白
            var designRatio = 1920f / 1080f;
            var screenRatio = ScreenUtility.ScreenHeight() / ScreenUtility.ScreenWidth();
            var scale = Mathf.Max(1f, screenRatio / designRatio);
            background.transform.localScale = Vector3.one * scale;
        }
        
        private void Start()
        {
            // closeButton 是可选的 UI 元素：模板默认不给启动界面返回按钮
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            UpdateText();
        }

        private void UpdateText()
        {
            startGameButton.gameObject.SetActive(_finished);
        }

        /// <summary>
        /// 设置进度和提示文本。当 value >= 0 时显示进度条。
        /// </summary>
        public void SetProgress(float value, string tip)
        {
            if (_finished) return;

            value = Mathf.Clamp01(value);
            if (progress != null)
            {
                orna.SetActive(true);
                progress.gameObject.SetActive(true);
                _progressTweener?.Kill();
                if (value <= progress.value)
                {
                    progress.value = value;
                }
                else
                {
                    _progressTweener = DOTween.To(
                        () => progress.value,
                        x => progress.value = x,
                        value,
                        0.15f
                    ).SetEase(Ease.OutCubic);
                }
            }
            if (tipText != null)
            {
                tipText.text = tip;
            }
        }

        /// <summary>
        /// 设置提示文本（不影响进度条）。
        /// </summary>
        public void SetTipText(string text)
        {
            if (_finished) return;

            if (tipText != null)
            {
                tipText.text = text;
            }
        }
        
        private void OnCloseButtonClick()
        {
            if (!_finished) return;
            
            OnClosed?.Invoke();
            OnClosed = null;
            if (I == this)
                I = null;
            Destroy(transform.parent.gameObject);
        }

        public void SetMainReady()
        {
            _finished = true;
            orna.SetActive(false);
            progress.gameObject.SetActive(false);
            UpdateText();
            if (launchCamera != null)
            {
                Destroy(launchCamera);
            }
        }

        public void Close()
        {
            if (I == this)
                I = null;
            Destroy(transform.parent.gameObject);
        }
    }
}
