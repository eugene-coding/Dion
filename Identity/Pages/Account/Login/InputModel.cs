// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Login;

public class InputModel
{
    [Required]
    [Display(Name = nameof(Username))]
    [PageRemote(ErrorMessage = "Username doesn`t exist", HttpMethod = "post", PageHandler = "ValidateUsername")]
    public string Username { get; set; }

    public string ReturnUrl { get; set; }
}
