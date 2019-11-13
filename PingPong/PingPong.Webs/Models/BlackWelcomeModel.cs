using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingPong.Webs.Models
{
    public class BlackWelcomeModel : IWelcomeModel
    {
        public string Color => "Black";
        public string Url => "/Assets/Paddle-Black.png";
    }
}
