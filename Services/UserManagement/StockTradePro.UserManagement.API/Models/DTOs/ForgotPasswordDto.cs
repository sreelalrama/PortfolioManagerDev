﻿using System.ComponentModel.DataAnnotations;

namespace StockTradePro.UserManagement.API.Models.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
