using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace KristofferStrube.Blazor.Popover;

public class Popover : ComponentBase
{
    private ElementReference? popoverElementReference;
    private Lazy<Task<IJSObjectReference>> popoverJSObjectReference = default!;

    protected override async Task OnInitializedAsync()
    {
        popoverJSObjectReference = new(async () =>
        {
            IJSObjectReference helper = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/KristofferStrube.Blazor.Popover/KristofferStrube.Blazor.Popover.js");
            return await helper.InvokeAsync<IJSObjectReference>("self", popoverElementReference);
        });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, TagType);

        builder.AddMultipleAttributes(1, Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<string, object>>>(AdditionalAttributes));

        builder.AddAttribute(2, "id", Id);
        builder.AddAttribute(3, "popover", Type is PopoverType.Auto ? "auto" : "manual");
        builder.AddContent(4, ChildContent);
        builder.AddElementReferenceCapture(5, value =>
        {
            popoverElementReference = value;
        });

        builder.CloseElement();

    }

    [Inject]
    private IJSRuntime jsRuntime { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// The content that will be rendered inside the popover
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// The behaviour of the popover. Defaults to <see cref="PopoverType.Auto"/>.
    /// </summary>
    [Parameter]
    public PopoverType Type { get; set; } = PopoverType.Auto;

    /// <summary>
    /// The id of the popover element.
    /// </summary>
    [Parameter, EditorRequired]
    public required string Id { get; set; }

    /// <summary>
    /// The type of element used for the popover element. This defaults to using a <c>div</c> tag.
    /// </summary>
    [Parameter]
    public string TagType { get; set; } = "div";

    public async Task HidePopoverAsync()
    {
        IJSObjectReference popover = await popoverJSObjectReference.Value;
        await popover.InvokeVoidAsync("hidePopover");
    }
}