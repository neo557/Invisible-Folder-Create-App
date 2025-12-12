using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenValult
{
    internal class AppSettings
    {
        public string HiddenFolderPath { get; set; }
        public DateTime Birthday { get; set; }
        public string ForbiddenParhMessage { get; set; } = "このパスは使用できません";
    }
}
