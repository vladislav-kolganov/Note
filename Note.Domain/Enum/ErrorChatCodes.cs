using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Note.Domain.Enum
{
    /// <summary>
    /// Ошибки чата
    /// </summary>
    public enum ErrorChatCodes
    {
        ChatNotFound = 0,

        MessageNotFound = 1,
        MessageIsEmpty = 2,
    }
}
