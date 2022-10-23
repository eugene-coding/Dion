// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Login;

public class InputModel
{
    [Required]
    [Display(Name = nameof(Username))]
    public string Username { get; set; }

    [Required]
    [Display(Name = nameof(Password))]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = nameof(Password))]
    public bool RememberLogin { get; set; }

    public string ReturnUrl { get; set; }

    public string Button { get; set; }
}
