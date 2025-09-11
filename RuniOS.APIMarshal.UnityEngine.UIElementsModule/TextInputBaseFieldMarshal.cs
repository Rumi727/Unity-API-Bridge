#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
using UnityEngine.UIElements;

namespace RuniOS.APIMarshal.UnityEngine.UIElements
{
    public abstract class TextInputBaseFieldMarshal<TValueType> : TextInputBaseField<TValueType> 
    {
        protected TextInputBaseFieldMarshal(int maxLength, char maskChar, TextInputBase textInputBase) : base(maxLength, maskChar, textInputBase) { }
        protected TextInputBaseFieldMarshal(string? label, int maxLength, char maskChar, TextInputBase textInputBase) : base(label, maxLength, maskChar, textInputBase) { }
        
        public new TextInputBaseMarshal textInputBase => (TextInputBaseMarshal)base.textInputBase;

        public abstract class TextInputBaseMarshal : TextInputBase
        {
            public new TextElement textElement => base.textElement;
            public new string originalText => base.originalText;
            
            public override bool AcceptCharacter(char character) => base.AcceptCharacter(character);
        }
    }
}