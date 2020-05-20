using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.UIModels
{
    public class Worker
    {
        [Required]
        [StringLength(14, ErrorMessage = "Worker ID is too long, should be 13 or 14 characters long")]
        [MinLength(13, ErrorMessage = "Worker ID is too short, should be 13 or  14 characters long")]
        public string WorkerId { get; set; } = "";
    }
}
