using DG.Tweening;

namespace Template
{
    public partial class LoadingForm : UIFormBase
    {
        private float _currentProgress;
        private Tweener _progressTweener;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            varScrollAnimation.PlayAnimation();
            SetProgress(0f,true);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            varScrollAnimation.StopAnimation();
            _progressTweener?.Kill();
        }

        public void SetProgress(float value,bool immediately = false)
        {
            _progressTweener?.Kill();
            if (immediately)
            {
                _currentProgress = value;
                varProgressText.text = $"{value*100:F1}%";
            }
            else
            {
                float start = _currentProgress;
                _progressTweener = DOTween.To(() => start, (x) =>
                {
                    _currentProgress = x;
                    if (varProgressText != null)
                    {
                        varProgressText.text = $"{x*100:F1}%";
                    }
                },value,0.5f).SetEase(Ease.Linear).SetUpdate(true);
            }
        }
    }
}