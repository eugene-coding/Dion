using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace UI.Pages;

public partial class Index
{
    [Inject] public IStringLocalizer<Index> Text { get; set; } = default!;
}
