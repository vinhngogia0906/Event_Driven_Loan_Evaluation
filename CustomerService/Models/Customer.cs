﻿namespace CustomerService.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
