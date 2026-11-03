using BurnicleFramework.Runtime;
using UnityEngine;

namespace Template
{
    public partial class Burnicle : MonoBehaviour
    {
        void Start()
        {
            RegisterBuiltinComponents();
            RegisterCustomComponents();
            BurnicleEntry.InitComponents();
        }
    }
}
