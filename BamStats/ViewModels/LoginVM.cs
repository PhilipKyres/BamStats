using System;
using System.ComponentModel.DataAnnotations;

namespace BamStats.ViewModels
{
    public partial class LoginVM
    {
		[Required]
        public String Password { get; set; }
	}
}