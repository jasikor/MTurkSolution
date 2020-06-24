using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTurk.UIModels
{
    public class SettingUI
    {
        [Required]
        public string Key;
        public string Value;
    }
}
