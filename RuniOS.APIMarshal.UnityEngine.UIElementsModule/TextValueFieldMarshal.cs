#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
using UnityEngine.UIElements;
namespace RuniOS.APIMarshal.UnityEngine.UIElements
{
    public abstract class TextValueFieldMarshal<TValueType> : TextValueField<TValueType> 
    {
        protected TextValueFieldMarshal(int maxLength, TextValueInputMarshal textValueInput) : base(maxLength, textValueInput) { }
        protected TextValueFieldMarshal(string? label, int maxLength, TextValueInputMarshal textValueInput) : base(label, maxLength, textValueInput) { }

        public new TextValueInputMarshal textInputBase => (TextValueInputMarshal)base.textInputBase;

        public abstract override bool CanTryParse(string textString);

        public abstract class TextValueInputMarshal : TextValueInput
        {
            public new TextElement textElement => base.textElement;
            public new string originalText => base.originalText;
            
            public override bool AcceptCharacter(char character) => base.AcceptCharacter(character);
        }
    }
}