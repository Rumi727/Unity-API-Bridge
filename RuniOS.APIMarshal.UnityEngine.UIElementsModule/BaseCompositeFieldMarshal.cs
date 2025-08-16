using RuniOS.APIMarshalAbstract.UnityEngine.UIElements;
using RuniOS.APIMarshal.UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace RuniOS.APIMarshalAbstract.UnityEngine.UIElements
{
    // 이렇게 한번 일반 클래스로 안씌워주면 IDE 상에서는 정상인데, 분명 override로 추상 메소드 구현 해줬는데도 유니티 컴파일러에서는 추상 메소드 구현 안했다고 컴파일 안함 
    public class BaseCompositeFieldMarshalAbstract<TValueType, TField, TFieldValue>(string? label, int fieldsByLine) : BaseCompositeField<TValueType, TField, TFieldValue>(label, fieldsByLine) where TField : TextValueField<TFieldValue>, new()
    {
        public sealed override FieldDescription[] DescribeFields() => DescribeFieldsMarshalAbstract().Select(static x => new FieldDescription(x.name, x.ussName, x.read, (ref TValueType val, TFieldValue value) => x.write(ref val, value))).ToArray();
        public virtual IEnumerable<BaseCompositeFieldMarshal<TValueType, TField, TFieldValue>.FieldDescriptionMarshal> DescribeFieldsMarshalAbstract() => throw new NotImplementedException();
    }
}

namespace RuniOS.APIMarshal.UnityEngine.UIElements
{
    public abstract class BaseCompositeFieldMarshal<TValueType, TField, TFieldValue>(string? label, int fieldsByLine) : BaseCompositeFieldMarshalAbstract<TValueType, TField, TFieldValue>(label, fieldsByLine) where TField : TextValueField<TFieldValue>, new()
    {
        public override IEnumerable<FieldDescriptionMarshal> DescribeFieldsMarshalAbstract() => DescribeFieldsMarshal();
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