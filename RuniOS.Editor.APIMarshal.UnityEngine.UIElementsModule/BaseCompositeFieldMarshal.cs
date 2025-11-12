#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
using RuniOS.Editor.APIMarshal.UnityEngine.UIElements;
using RuniOS.Editor.APIMarshalAbstract.UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace RuniOS.Editor.APIMarshalAbstract.UnityEngine.UIElements
{
    // 이렇게 한번 일반 클래스로 안씌워주면 IDE 상에서는 정상인데, 분명 override로 추상 메소드 구현 해줬는데도 유니티 컴파일러에서는 추상 메소드 구현 안했다고 컴파일 안함
    /// <inheritdoc />
    [Obsolete("This class should not be used directly as it is an inner class to prevent compilation errors that occur when access modifiers are forcibly ignored.")]
    public class BaseCompositeFieldMarshalAbstract<TValueType, TField, TFieldValue>(string? label, int fieldsByLine) : BaseCompositeField<TValueType, TField, TFieldValue>(label, fieldsByLine) where TField : TextValueField<TFieldValue>, new()
    {
        public sealed override FieldDescription[] DescribeFields() => DescribeFieldsMarshalAbstract().Select(static x => new FieldDescription(x.name, x.ussName, x.read, (ref TValueType val, TFieldValue value) => x.write(ref val, value))).ToArray();
        internal virtual IEnumerable<BaseCompositeFieldMarshal<TValueType, TField, TFieldValue>.FieldDescriptionMarshal> DescribeFieldsMarshalAbstract() => throw new NotImplementedException();
    }
}

namespace RuniOS.Editor.APIMarshal.UnityEngine.UIElements
{
    /// <inheritdoc />
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
    public abstract class BaseCompositeFieldMarshal<TValueType, TField, TFieldValue>(string? label, int fieldsByLine) : BaseCompositeFieldMarshalAbstract<TValueType, TField, TFieldValue>(label, fieldsByLine) where TField : TextValueField<TFieldValue>, new()
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
    {
        internal override IEnumerable<FieldDescriptionMarshal> DescribeFieldsMarshalAbstract() => DescribeFieldsMarshal();
        public abstract IEnumerable<FieldDescriptionMarshal> DescribeFieldsMarshal();
        
        public readonly struct FieldDescriptionMarshal(string name, string ussName, Func<TValueType, TFieldValue> read, FieldDescriptionMarshal.WriteDelegate write)
        {
            public readonly string name = name;
            public readonly string ussName = ussName;
            public readonly Func<TValueType, TFieldValue> read = read;
            public readonly WriteDelegate write = write;

            public delegate void WriteDelegate(ref TValueType val, TFieldValue fieldValue);
        }
    }
}
