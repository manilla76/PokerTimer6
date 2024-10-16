using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace PokerTimer8.Client.Data
{
    public class InputText : InputBase<string>
    {
        [DisallowNull]
        public ElementReference? Element { get; protected set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValue));
            builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder<string>(
                this, _value => CurrentValueAsString = _value, CurrentValueAsString));
            builder.AddElementReferenceCapture(4, _inputReference =>  Element = _inputReference);
            builder.CloseElement();
        }

        protected override bool TryParseValueFromString(string? value, out string? result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            result = value;
            validationErrorMessage = null;
            return true;
        }
    }
}
