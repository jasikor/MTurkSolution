using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.UIModels
{
    public class Worker
    {
        private const string errorMessage = "Worker ID is too short, should be between 13 and 14 characters";

        [Required]
        [StringLength(14, ErrorMessage = errorMessage)]
        [MinLength(13, ErrorMessage = errorMessage)]
        public string WorkerId { get; set; } = "";
    }
}
