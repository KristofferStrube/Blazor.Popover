using KristofferStrube.Blazor.DOM;
using KristofferStrube.Blazor.DOM.Extensions;
using KristofferStrube.Blazor.WebIDL;
using Microsoft.JSInterop;

namespace KristofferStrube.Blazor.Popover;

[IJSWrapperConverter]
public class ToggleEvent : Event, IJSCreatable<ToggleEvent>
{
    /// <summary>
    /// A lazily loaded task that evaluates to a helper module instance from the Blazor.Popover library.
    /// </summary>
    protected readonly Lazy<Task<IJSObjectReference>> popoverHelperTask;

    /// <inheritdoc/>
    public static new async Task<ToggleEvent> CreateAsync(IJSRuntime jSRuntime, IJSObjectReference jSReference)
    {
        return await CreateAsync(jSRuntime, jSReference, new());
    }

    /// <inheritdoc/>
    public static new Task<ToggleEvent> CreateAsync(IJSRuntime jSRuntime, IJSObjectReference jSReference, CreationOptions options)
    {
        return Task.FromResult(new ToggleEvent(jSRuntime, jSReference, new()));
    }

    protected ToggleEvent(IJSRuntime jSRuntime, IJSObjectReference jSReference, CreationOptions options) : base(jSRuntime, jSReference, options)
    {
        popoverHelperTask = new(jSRuntime.GetHelperAsync);
    }

    /// <summary>
    /// Set to <c>"closed"</c> when transitioning from closed to open, or set to <c>"open"</c> when transitioning from open to closed.
    /// </summary>
    public async Task<string> GetOldState()
    {
        IJSObjectReference helper = await popoverHelperTask.Value;
        return await helper.InvokeAsync<string>("getAttribute", JSReference, "oldState");
    }

    /// <summary>
    /// Set to <c>"open"</c> when transitioning from closed to open, or set to <c>"closed"</c> when transitioning from open to closed.
    /// </summary>
    public async Task<string> GetNewState()
    {
        IJSObjectReference helper = await popoverHelperTask.Value;
        return await helper.InvokeAsync<string>("getAttribute", JSReference, "newState");
    }
}
