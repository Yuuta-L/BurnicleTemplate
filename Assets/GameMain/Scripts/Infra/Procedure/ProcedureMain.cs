using BurnicleFramework.Generated;
using BurnicleFramework.Runtime;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Obfuz;

namespace Template
{
    /// <summary>
    /// 模板主流程：关闭启动界面并打开主界面，之后由项目自己的业务推进。
    /// </summary>
    [ObfuzIgnore]
    public class ProcedureMain : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            if (LaunchForm.I != null)
            {
                LaunchForm.I.Close();
            }

            Burnicle.UI.Open(EnumUIForm.MainForm);
        }
    }
}
