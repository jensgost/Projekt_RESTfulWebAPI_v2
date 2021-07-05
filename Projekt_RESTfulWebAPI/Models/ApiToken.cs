using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Projekt_RESTfulWebAPI.Models
{
    public class ApiToken
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public Guid Key { get; set; }
    }
}
