using KristofferStrube.Blazor.DOM;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace KristofferStrube.Blazor.Popover;

public class Popover : ComponentBase, IAsyncDisposable
{
    private ElementReference? popoverElementReference;
    private Lazy<Task<EventTarget>> lazyPopoverEventTarget = default!;

    protected override void OnInitialized()
    {
        lazyPopoverEventTarget = new(async () =>
        {
            return await EventTarget.CreateAsync(jsRuntime, popoverElementReference!.Value);
        });
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, TagType);

        builder.AddMultipleAttributes(1, AdditionalAttributes);

        if (Id is not null)
        {
            builder.AddAttribute(2, "id", Id);
        }
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
    public required Dictionary<string, object> AdditionalAttributes { get; set; }

    /// <summary>
    /// The content that will be rendered inside the popover
    /// </summary>
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    /// <summary>
    /// The behaviour of the popover. Defaults to <see cref="PopoverType.Auto"/>.
    /// </summary>
    [Parameter]
    public PopoverType Type { get; set; } = PopoverType.Auto;

    /// <summary>
    /// The id of the popover element.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// The type of element used for the popover element. This defaults to using a <c>div</c> tag.
    /// </summary>
    [Parameter]
    public string TagType { get; set; } = "div";

    /// <summary>
    /// Shows the popover element by adding it to the top layer.
    /// If element's popover attribute is in the <see cref="PopoverType.Auto"/> state, then this will also close all other auto popovers unless they are an ancestor of element according to the topmost popover ancestor algorithm.
    /// </summary>
    public async Task ShowPopoverAsync()
    {
        EventTarget popover = await lazyPopoverEventTarget.Value;
        await popover.JSReference.InvokeVoidAsync("showPopover");
    }

    /// <summary>
    /// Hides the popover element by removing it from the top layer and applying <c>display: none</c> to it.
    /// </summary>
    public async Task HidePopoverAsync()
    {
        EventTarget popover = await lazyPopoverEventTarget.Value;
        await popover.JSReference.InvokeVoidAsync("hidePopover");
    }

    /// <summary>
    /// If the popover element is not showing, then this method shows it. Otherwise, this method hides it.
    /// </summary>
    /// <returns>
    /// This method returns <see langword="true"/> if the popover is open after calling it, otherwise  <see langword="false"/>.
    /// </returns>
    public async Task<bool> TogglePopoverAsync()
    {
        EventTarget popover = await lazyPopoverEventTarget.Value;
        return await popover.JSReference.InvokeAsync<bool>("togglePopover");
    }

    /// <summary>
    /// Adds an <see cref="EventListener{ToggleEvent}"/> for when the popover element transitions between shown and hidden.
    /// </summary>
    /// <param name="callback">Callback that will be invoked when the event is dispatched.</param>
    /// <param name="options"><inheritdoc cref="EventTarget.AddEventListenerAsync{TEvent}(string, EventListener{TEvent}?, AddEventListenerOptions?)" path="/param[@name='options']"/></param>
    public async Task AddOnBeforeToggleEventListener(EventListener<ToggleEvent> callback, AddEventListenerOptions? options = null)
    {
        EventTarget popover = await lazyPopoverEventTarget.Value;
        await popover.AddEventListenerAsync("beforetoggle", callback, options);
    }

    /// <summary>
    /// Removes the event listener from the event listener list if it has been parsed to <see cref="AddOnBeforeToggleEventListener"/> previously.
    /// </summary>
    /// <param name="callback">The callback <see cref="EventListener{TEvent}"/> that you want to stop listening to events.</param>
    /// <param name="options"><inheritdoc cref="EventTarget.RemoveEventListenerAsync{TEvent}(string, EventListener{TEvent}?, EventListenerOptions?)" path="/param[@name='options']"/></param>
    public async Task RemoveOnBeforeToggleEventListener(EventListener<ToggleEvent> callback, EventListenerOptions? options = null)
    {
        EventTarget popover = await lazyPopoverEventTarget.Value;
        await popover.RemoveEventListenerAsync("beforetoggle", callback, options);
    }

    public async ValueTask DisposeAsync()
    {
        if (lazyPopoverEventTarget.IsValueCreated)
        {
            EventTarget popover = await lazyPopoverEventTarget.Value;
            await popover.DisposeAsync();
        }
    }
}