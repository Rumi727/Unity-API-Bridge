using RuniOS.Editor.APIMarshalAbstract.UnityEditor;
using System;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;

namespace RuniOS.Editor.APIMarshalAbstract.UnityEditor
{
    // 이렇게 한번 일반 클래스로 안씌워주면 IDE 상에서는 정상인데, 분명 override로 추상 메소드 구현 해줬는데도 유니티 컴파일러에서는 추상 메소드 구현 안했다고 컴파일 안함
    /// <inheritdoc />
    [Obsolete("This class should not be used directly as it is an inner class to prevent compilation errors that occur when access modifiers are forcibly ignored.")]
    public class MainToolbarElementMarshalAbstract : MainToolbarElement
    {
        /// <inheritdoc />
        public sealed override VisualElement CreateElement() => CreateElementMarshalAbstract();
        internal virtual VisualElement CreateElementMarshalAbstract() => throw new NotImplementedException();
    }
}

namespace RuniOS.Editor.APIMarshal.UnityEditor
{
    /// <inheritdoc />
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
    public abstract class MainToolbarElementMarshal : MainToolbarElementMarshalAbstract
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
    {
#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
        internal override VisualElement CreateElementMarshalAbstract() => CreateElementMarshal();
        public abstract VisualElement CreateElementMarshal();
#pragma warning restore CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
    }
}
