using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Websocket
{
    interface IWebsocket
    {
        void Send(String message);
        void Close();

    }
}
