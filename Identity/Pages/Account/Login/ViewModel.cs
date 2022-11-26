// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace Identity.Pages.Login;

public class ViewModel
{
    public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
    public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));
}
