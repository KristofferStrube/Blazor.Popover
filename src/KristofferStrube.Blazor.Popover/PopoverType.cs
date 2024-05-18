namespace KristofferStrube.Blazor.Popover;

/// <summary>
/// Controls be haviour of a popover tag.
/// </summary>
public enum PopoverType
{
    /// <summary>
    /// Closes other popovers when opened; has light dismiss and responds to close requests.
    /// </summary>
    Auto,

    /// <summary>
    /// Does not close other popovers; does not light dismiss or respond to close requests.
    /// </summary>
    Manual,
}
