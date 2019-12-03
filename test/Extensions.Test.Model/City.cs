using System;

namespace Extensions.Test.Model
{
    public class City
    {
        public City()
        {
        }

        public Country Country { get; set; }

        public Guid CountryId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}