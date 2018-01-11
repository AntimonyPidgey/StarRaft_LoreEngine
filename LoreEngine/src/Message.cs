//----------------------------------------------------------------------
//  Message.cs
//  by Zephyrus (1/9/2016)
//  Part of LoreEngine 0.1
//  
//  Container class to hold values associated with a specific Message,
//  once it has been set to send.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoreEngine
{
    class Message
    {
        public Ship sendingShip { get; set; }
        public Message sourceMessage { get; set; }
        public string ID { get; set; }
        public string Text { get; set; }
        public double ReceiveTime { get; set; }
        public double baseQuality { get; set; }
        public List<string[]> Replies { get; set; }
        public List<string[]> Events { get; set; }
    }
}
