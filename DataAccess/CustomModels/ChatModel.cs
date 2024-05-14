using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class ChatViewModel
    {
        public int ChatId { get; set; }

        public int AdminId { get; set; }

        public int ProviderId { get; set; }

        public int RequestId { get; set; }

        public int RoleId { get; set; }

        public string? Message { get; set; }

        public string? ChatDate { get; set; }

        public int SentBy { get; set; }

        public string? ChatBoxClass { get; set; }

        public string? RecieverName { get; set; }

        public string? flag { get; set; }

        public List<ChatViewModel> Chats { get; set; }
    }
}
