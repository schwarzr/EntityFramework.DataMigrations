using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Extensions.Test.Model
{
    public class Country
    {
        public Country()
        {
            this.Cities = new HashSet<City>();
        }

        public ICollection<City> Cities { get; set; }

        public Currency Currency { get; set; }

        public Guid CurrencyId { get; set; }

        public int Id { get; set; }

        [StringLength(2)]
        public string Iso2Code { get; set; }

        [StringLength(3)]
        public string Iso3Code { get; set; }

        public string Name { get; set; }
    }
}