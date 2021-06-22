using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace MaterializeSignalR.Models
{
    public class IndexModel : PageModel
    {
        public string Name { get; set; }
        public string Color { get; set; } = "#4B0082";
        public string TextColor { get; set; } = "#FFFFFF";
        public string Avatar { get; set; } = "images/Avatar.png";
        public string Email { get; set; }
        public string Password { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                Email = HttpContext.Session.GetString("Email");
                Avatar = HttpContext.Session.GetString("Avatar");
                Color = HttpContext.Session.GetString("Color");
                TextColor = HttpContext.Session.GetString("TextColor");
                Name = HttpContext.Session.GetString("Name");

                if (!Database.ExistInDatabase(Email))
                {
                    return Redirect("~/Login");
                }
            }
            else
            {
                AuthenticateResult result = await HttpContext.AuthenticateAsync();

                if (!result.Succeeded)
                {
                    return Redirect("~/Login");
                }
                else
                {
                    Email = result.Principal.Claims.Where(p => p.Type == ClaimTypes.Email)?.FirstOrDefault()?.Value;
                    HttpContext.Session.SetString("Email", Email);
                    var user = Database.GetFromDatabase(Email);
                    
                    if (user != null)
                    {
                        Avatar = user.Avatar;
                        Color = user.Color;
                        TextColor = user.TextColor;
                        Name = user.UserName;

                        HttpContext.Session.SetString("Avatar", Avatar);
                        HttpContext.Session.SetString("Color", Color);
                        HttpContext.Session.SetString("TextColor", TextColor);
                        HttpContext.Session.SetString("TextColor", TextColor);
                        HttpContext.Session.SetString("Name", Name);
                    }
                    else
                    {
                        return Redirect("~/Login");
                    }
                }
            }

            return Page();
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(string page, string email, string password, string name, string color, string textColor)
        {
            if (page == "Login")
            {
                UserData user = Database.GetFromDatabase(email);

                if (user != null && user.Password == password)
                {
                    Name = user.UserName;
                    Avatar = user.Avatar;
                    Color = user.Color;
                    TextColor = user.TextColor;
                    Email = user.Email;

                    HttpContext.Session.SetString("Email", Email);
                    HttpContext.Session.SetString("Avatar", Avatar);
                    HttpContext.Session.SetString("Color", Color);
                    HttpContext.Session.SetString("TextColor", TextColor);
                    HttpContext.Session.SetString("Name", Name);
                }
                else
                {
                    if (user == null)
                    {
                        TempData.Add("LoginError", "There is no user registered with that e-mail address.");
                    }
                    else
                    {
                        TempData.Add("LoginError", "Password is not valid for the user with that e-mail address.");
                    }
                    return Redirect("~/Login");
                }
            }
            else if (page == "Register")
            {
                if (Database.ExistInDatabase(email))
                {
                    TempData.Add("RegisterError", "The user with used e-mail address already exist in the database.");
                    return Redirect("~/Register");
                }

                string fileName = null;
                var file = Request.Form.Files.FirstOrDefault();

                if (file != null)
                {
                    do
                    {
                        fileName = Path.GetRandomFileName();
                        fileName = Path.ChangeExtension(fileName, Path.GetExtension(file.FileName));
                        fileName = Path.Combine("wwwroot", "images", "avatars", fileName);
                    }
                    while (System.IO.File.Exists(fileName));

                    using (FileStream fs = new FileStream(fileName, FileMode.CreateNew))
                    {
                        await Request.Form.Files.First().CopyToAsync(fs);
                    }
                }

                if (fileName != null)
                {
                    fileName = "images/avatars/" + Path.GetFileName(fileName);
                }

                if (string.IsNullOrWhiteSpace(color) || string.IsNullOrWhiteSpace(textColor))
                {
                    TempData.Add("RegisterError", "Color can't be empty.");
                    return Redirect("~/Register");
                }

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    TempData.Add("RegisterError", "Email, password and name can't be empty.");
                    return Redirect("~/Register");
                }

                UserData user = new UserData
                {
                    Avatar = fileName ?? "images/Avatar.png",
                    Color = color,
                    TextColor = textColor,
                    Email = email,
                    Password = password,
                    UserName = name
                };

                Database.SaveToDatabase(user);

                Name = user.UserName;
                Avatar = user.Avatar;
                Color = user.Color;
                TextColor = user.TextColor;
                Email = user.Email;

                HttpContext.Session.SetString("Email", Email);
                HttpContext.Session.SetString("Avatar", Avatar);
                HttpContext.Session.SetString("Color", Color);
                HttpContext.Session.SetString("TextColor", TextColor);
                HttpContext.Session.SetString("Name", Name);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                RedirectUri = "~/Login"
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            
            return Redirect("~/");
        }
    }
}
