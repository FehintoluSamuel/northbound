using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthboundSessions.Data
{
    public class BankOption
    {
        public int Id { get; set; }
        public int BankQuestionId { get; set; } 
        public required string OptionText { get; set; } 
        public bool IsCorrect { get; set; } 
        public int DisplayOrder { get; set; } 
        public BankQuestion? BankQuestion { get; set; } 
    }
}
