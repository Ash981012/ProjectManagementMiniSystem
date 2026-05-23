using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagement.Application.Users.Commands;
using ProjectManagement.Web.Pages.Account.Models;

namespace ProjectManagement.Web.Pages.Account;

[AllowAnonymous]
public sealed class RegisterModel(RegisterEmployeeCommandHandler registerEmployee) : PageModel
{
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await registerEmployee.Handle(
            new RegisterEmployeeCommand(Input.FullName, Input.Email, Input.Password),
            cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result.Errors);
            return Page();
        }

        TempData["SuccessMessage"] = "Registration successful. Please login.";
        return RedirectToPage("/Account/Login");
    }

    private void AddErrors(IEnumerable<string> errors)
    {
        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);
    }
}
