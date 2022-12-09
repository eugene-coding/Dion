using Microsoft.AspNetCore.Components;

namespace Identity.Pages.Shared;

public sealed partial class Prompt
{
    [Parameter]
    [EditorRequired]
    public string Text { get; init; } = string.Empty;

    [Parameter]
    [EditorRequired]
    public string ButtonText { get; init; } = string.Empty;

    [Parameter]
    [EditorRequired]
    public Uri Url { get; init; } = default!;
}
