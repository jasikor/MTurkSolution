using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.UIModels
{
    public class GameParameterUI
    {
        public int Id;
        [Required]
        public int Surplus;
        [Required]
        public int TurksDisValue;
        [Required]
        public int MachineDisValue;
        [Required]
        public int TimeOut;
        [Required]
        [Range(0.0, 1.0, ErrorMessage = "Stubborn must be in [0..1]")]
        public double Stubborn;
        [Required]
        public bool MachineStarts;
    }
}
