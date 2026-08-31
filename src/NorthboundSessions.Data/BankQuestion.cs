using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthboundSessions.Data
{
    public class BankQuestion
    {
        public int Id { get; set; }
        public int TopicBankItemId { get; set; }
        public required string QuestionText { get; set; }
        public int DisplayOrder { get; set; }
        public TopicBankItem? TopicBankItem { get; set; } 
        public ICollection<BankOption> Options { get; set; } = new List<BankOption>(); 
        
    }
}
