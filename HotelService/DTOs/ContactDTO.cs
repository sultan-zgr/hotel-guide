﻿namespace HotelService.DTOs
{
    public class ContactDTO
    {
        public Guid Id { get; set; }
        public string Type { get; set; } // Phone, Email, etc.
        public string Value { get; set; }
    }
}
