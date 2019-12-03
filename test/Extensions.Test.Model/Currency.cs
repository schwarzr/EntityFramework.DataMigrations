using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Extensions.Test.Model
{
    public class Currency
    {
        public Currency()
        {
            this.Countries = new HashSet<Country>();
        }

        public ICollection<Country> Countries { get; set; }

        public Guid Id { get; set; }

        [StringLength(3)]
        public string IsoCode { get; set; }

        [StringLength(255)]
        public string Title { get; set; }
    }
}