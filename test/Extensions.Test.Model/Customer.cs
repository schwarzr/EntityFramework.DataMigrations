using System;
using System.ComponentModel.DataAnnotations;

namespace Extensions.Test.Model
{
    public class Customer
    {
        public Country Country { get; set; }

        public int CountryId { get; set; }

        [StringLength(255)]
        public string FirstName { get; set; }

        public Guid Id { get; set; }

        [StringLength(255)]
        public string LastName { get; set; }
    }
}