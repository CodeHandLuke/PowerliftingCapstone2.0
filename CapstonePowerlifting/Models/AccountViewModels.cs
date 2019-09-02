﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstonePowerlifting.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

	public class RepsPercentageViewModel
	{
		public string Exercise { get; set; }
		public int? TotalReps { get; set; }
	}

	public class WeightPercentageViewModel
	{
		public string Exercise { get; set; }
		public int? TotalWeight { get; set; }
	}

	public class OneRepMaxValueViewModel
	{
		public string Date { get; set; }
		public double Weight { get; set; }
	}

	public class ExerciseObjectViewModel
	{
		public string Name { get; set; }
		public List<OneRepMaxValueViewModel> OneRepMaxValues { get; set; }
	}

	public class OneRepMaxLeaderboardViewModel
	{
		public string UserName { get; set; }
		public int Age { get; set; }
		public double Weight { get; set; }
		[Display(Name = "Squat(kg)")]
		public double Squat { get; set; }
		[Display(Name = "Bench(kg)")]
		public double Bench { get; set; }
		[Display(Name = "Deadlift(kg)")]
		public double Deadlift { get; set; }
		[Display(Name = "Total(kg)")]
		public double Total { get; set; }
		public double Wilks { get; set; }
	}
}
