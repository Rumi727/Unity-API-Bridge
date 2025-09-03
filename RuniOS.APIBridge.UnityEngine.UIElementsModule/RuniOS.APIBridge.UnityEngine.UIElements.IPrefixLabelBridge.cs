using System.Reflection;
using UnityEngine.UIElements;

namespace RuniOS.APIBridge.UnityEngine.UIElements
{
    public partial interface IPrefixLabelBridge
    {
        /// <summary>
        /// 라벨 설정
        /// </summary>
        public void SetLabel(string? value)
        {
            PropertyInfo? propertyInfo = __instance.GetType().GetProperty(nameof(label));
            if (propertyInfo != null && propertyInfo.SetMethod != null)
                propertyInfo.SetValue(__instance, value);
            else
                ((INotifyValueChanged<string>)labelElement).SetValueWithoutNotify(value ?? string.Empty);
        }
    }
}