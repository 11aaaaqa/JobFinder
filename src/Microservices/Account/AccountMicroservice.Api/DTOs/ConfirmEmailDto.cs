﻿namespace AccountMicroservice.Api.DTOs
{
    public class ConfirmEmailDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}