using UnityEngine.UIElements;

namespace RuniOS.APIMarshal.UnityEngine.UIElements
{
    public abstract class TextInputBaseFieldMarshal<TValueType> : TextInputBaseField<TValueType> 
    {
        protected TextInputBaseFieldMarshal(int maxLength, char maskChar, TextInputBase textInputBase) : base(maxLength, maskChar, textInputBase) { }
        protected TextInputBaseFieldMarshal(string? label, int maxLength, char maskChar, TextInputBase textInputBase) : base(label, maxLength, maskChar, textInputBase) { }

        public abstract class TextInputBaseMarshal : TextInputBase
        {
            public new TextElement textElement => base.textElement;
            public override bool AcceptCharacter(char character) => base.AcceptCharacter(character);
        }
    }
}