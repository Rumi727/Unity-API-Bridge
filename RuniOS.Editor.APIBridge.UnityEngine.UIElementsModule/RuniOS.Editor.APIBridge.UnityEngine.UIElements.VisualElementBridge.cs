using UnityEngine.UIElements;

namespace RuniOS.Editor.APIBridge.UnityEngine.UIElements
{
    public partial class VisualElementBridge
    {
#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        public PseudoStatesBridge pseudoStates
        {
            get => (PseudoStatesBridge)__instance.pseudoStates;
            set => __instance.pseudoStates = (PseudoStates)value;
        }
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    }
}