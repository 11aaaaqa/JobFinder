﻿namespace EmployerMicroservice.Api.DTOs.Permissions_dtos
{
    public class AddPermissionsDto
    {
        public Guid EmployerId { get; set; }
        public List<string> Permissions { get; set; }
    }
}
