using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExclusiveCard.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.Website.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [DisplayName("*Username:")]
        [MaxLength(30)]
        public string Username { get; set; }

        [DisplayName("*Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [DisplayName("*Confirm Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [MaxLength(100)]
        [Compare("Email", ErrorMessage = "Email and confirm email do not match.")]
        public string ConfirmEmail { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("*Password:")]
        public string Password { get; set; }

        [DisplayName("*Confirm Password:")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }
        [DisplayName("*Role:")]
        public string RoleId { get; set; }
        public List<SelectListItem> Roles { get; set; }

        public UserViewModel()
        {
            List<SelectListItem> roles = (from object eVal in Enum.GetValues(typeof(Roles))
                select new SelectListItem { Text = Enum.GetName(typeof(Roles), eVal), Value = eVal.ToString() }).ToList();

            var index = roles.FindIndex(x => x.Text == Enums.Roles.RootUser.ToString());
            if (index > -1)
            {
                roles.RemoveAt(index);
            }
            index = roles.FindIndex(x => x.Text == Enums.Roles.User.ToString());
            if (index > -1)
            {
                roles.RemoveAt(index);
            }

            Roles = roles.Select(role => new SelectListItem
            {
                Text = role.Text == Enums.Roles.AdminUser.ToString()? Keys.Keys.AdminUser : Keys.Keys.BackOfficeUser,
                Value = role.Value
            })
            .ToList();
        }
    }
}
