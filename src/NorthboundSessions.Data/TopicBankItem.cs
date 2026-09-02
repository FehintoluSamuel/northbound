using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NorthboundSessions.Data;

namespace NorthboundSessions.Data
{
    public class TopicBankItem
    {
         public int Id { get; set; } 
         public required string Title { get; set; } 
         public required string OutlineContent { get; set; } 
         public byte[]? ImageBytes { get; set; }
         public string? MarketSymbol { get; set; } 
         public bool IsUsed { get; set; } = false; 
         public ICollection<BankQuestion> Questions { get; set; } = new List<BankQuestion>(); 
    }
}
