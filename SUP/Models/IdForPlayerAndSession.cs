using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUP.Models
{
    public class IdForPlayerAndSession
    {
        public int PlayerId { get; set; }
        public int SessionId { get; set; }


        public IdForPlayerAndSession(int playerId, int sessionId)
        {
            PlayerId = playerId;
            SessionId = sessionId;
        }

    }
}
