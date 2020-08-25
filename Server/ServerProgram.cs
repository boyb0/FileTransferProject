using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class ServerProgram
    {
        private static LinkedList<ClientContainer> listOfClients = new LinkedList<ClientContainer>();
        private static IPAddress myAddress = IPAddress.Parse("127.0.0.1");

        public static bool IdExists(int idNumber)
        {
            foreach(ClientContainer cc in listOfClients)
            {
                if(cc.getIdNumber() == idNumber)
                {
                    return true;
                }
            }
            return false;
        }
        public static ClientContainer PasswordAuthentication(int idNumber, String password)
        {
            foreach (ClientContainer cc in listOfClients)
            {
                if (cc.getIdNumber() == idNumber && cc.getPassword() == password)
                {
                    return cc;
                }
            }
            return null;
        }
        static void ServerWorker(Object obj)
        {
            TcpClient cli = (TcpClient) obj;
            try
            {
                StreamReader reader = new StreamReader(cli.GetStream());
                StreamWriter writer = new StreamWriter(cli.GetStream());
                String clientInfo = reader.ReadLine(); // citanje info za najaveniot klient
                Console.WriteLine(clientInfo);
                int clid = Convert.ToInt32(clientInfo.Split(',')[0]);
                String pw = clientInfo.Split(',')[1];
                int port = ((IPEndPoint)cli.Client.RemoteEndPoint).Port;
                ClientContainer cc = new ClientContainer(clid,pw,cli.GetStream(),writer);
                if(!listOfClients.Contains(cc))
                {
                    writer.WriteLine("Најавата е успешна.");
                    writer.Flush();
                    listOfClients.AddLast(cc);
                    Console.WriteLine("Успешно додаден клиент во листата.");
                    while(true)
                    {
                        String poraka;
                        while ((poraka = reader.ReadLine()) == null);
                        int toId = Convert.ToInt32(poraka);
                        if(IdExists(toId))
                        {
                            Console.WriteLine("Проверка ID");
                            writer.WriteLine("ПОСТОИ");
                            writer.Flush();
                            String toPwd = reader.ReadLine();
                            ClientContainer ccon;
                            if((ccon=PasswordAuthentication(toId,toPwd))!=null)
                            {
                                Console.WriteLine("Проверка Password");
                                writer.WriteLine("ТОЧЕН");
                                writer.Flush();
                                String row;
                                row = reader.ReadLine();
                                String [] buffer = new String[100];
                                buffer[0] = row;
                                int i = 1;
                                Console.WriteLine(buffer[0]);
                                while (!(row = reader.ReadLine()).Equals("END"))
                                {
                                    buffer[i] = row;
                                    //Console.WriteLine(buffer[i]);
                                    i++;
                                    //Console.WriteLine(i);
                                }
                                Console.WriteLine("#Пренесувам датотека "+buffer[0]);
                                StreamWriter writerTo = ccon.getWriter();
                                writerTo.WriteLine(buffer[0]);
                                writerTo.Flush();
                                i = 1;
                                while (i<100)
                                {
                                    writerTo.WriteLine(buffer[i]);
                                    i++;                           
                                }
                                writerTo.WriteLine("END");
                                writerTo.Flush();
                                Console.WriteLine("Завршено пренесување");
                            }
                            else
                            {
                                writer.WriteLine("ГРЕШЕН");
                                writer.Flush();
                            }

                        }
                        else
                        {
                            writer.WriteLine("НЕПОСТОИ");
                            writer.Flush();
                        }

                    }


                }
                else
                {
                    Console.WriteLine("Додавањето во листа е неуспешно.");
                    writer.WriteLine("Најавата е неуспешна.");
                    writer.Flush();
                }
            }
            catch
            {

            }
            finally
            {
                if (cli != null)
                    cli.Close();
            }

        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            TcpListener serverListener = null;
            try
            {
                serverListener = new TcpListener(myAddress, 8585);
                serverListener.Start();
                Console.WriteLine("FileTransfer серверот е покренат...");
                while (true)
                {
                    Console.WriteLine("Чекам конекција од клиент...");
                    TcpClient client = serverListener.AcceptTcpClient();
                    Console.WriteLine("Поврзан е нов клиент...");
                    Thread t = new Thread(ServerWorker);
                    t.Start(client);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (serverListener != null)
                    serverListener.Stop();
            }
        }
    }
}
