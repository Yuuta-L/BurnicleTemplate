using BurnicleFramework.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Template
{
    /// <summary>
    /// 项目层子页面基类：在 <see cref="UISubForm"/> 之上补齐多语言处理，与 UIFormBase / UIItemBase 保持一致。
    /// <para>
    /// 懒加载的子页面不在界面初始化时存在，界面自己的 InitLocalization 扫不到它，
    /// 所以子页面需要在自己的 Awake 里扫一遍静态文本（`UIStringKey`）。
    /// </para>
    /// </summary>
    public class UISubFormBase : UISubForm
    {
        protected virtual void Awake()
        {
            InitLocalization();
        }

        /// <summary>
        /// 更新子页面中静态文本的多语言文字。
        /// </summary>
        public virtual void InitLocalization()
        {
            UIStringKey[] texts = GetComponentsInChildren<UIStringKey>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                UIStringKey key = texts[i];
                if (key.TryGetComponent<TMP_Text>(out var tmpText))
                {
                    tmpText.text = Burnicle.Localization.GetString(key.Key);
                }
                else if (key.TryGetComponent<Text>(out var uiText))
                {
                    uiText.text = Burnicle.Localization.GetString(key.Key);
                }
            }
        }
    }
}
