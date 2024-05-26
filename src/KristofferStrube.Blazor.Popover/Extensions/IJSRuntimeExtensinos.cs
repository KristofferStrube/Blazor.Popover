using Microsoft.JSInterop;

namespace KristofferStrube.Blazor.Popover.Extensions;

internal static class IJSRuntimeExtensions
{
    internal static async Task<IJSObjectReference> GetHelperAsync(this IJSRuntime jSRuntime)
    {
        return await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/KristofferStrube.Blazor.Popover/KristofferStrube.Blazor.Popover.js");
    }
}