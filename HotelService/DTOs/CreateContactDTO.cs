﻿namespace HotelService.DTOs
{
    public class CreateContactDTO
    {
        public string Type { get; set; } // Phone, Email, etc.
        public string Value { get; set; }
    }
}
