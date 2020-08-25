using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    class ClientContainer
    {
        private int idNumber;
        private String password;
        private NetworkStream stream;
        private StreamWriter myWriter;

        public ClientContainer(int idNumber,String password, NetworkStream stream, StreamWriter myWriter)
        {
            this.idNumber = idNumber;
            this.password = password;
            this.stream = stream;
            this.myWriter = myWriter;
        }
        public StreamWriter getWriter()
        {
            return myWriter;
        }
        public int getIdNumber()
        {
            return idNumber;
        }
        public String getPassword()
        {
            return password;
        }
        public NetworkStream getStream()
        {
            return stream;
        }
    }
}
