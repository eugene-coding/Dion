// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace Identity.Pages.Device;

public class DeviceOptions
{
    public const string OfflineAccessDisplayName = "Offline Access";
    public const string OfflineAccessDescription = "Access to your applications and resources, even when you are offline";
    public const string InvalidUserCode = "Invalid user code";
    public const string MustChooseOneErrorMessage = "You must pick at least one permission";
    public const string InvalidSelectionErrorMessage = "Invalid selection";

    public static bool EnableOfflineAccess => true;
}
