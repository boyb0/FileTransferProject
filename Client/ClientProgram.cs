using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

/*
 * Текстуалните датотеки кои се препраќаат меѓу клиентите мора да завршуваат со линија END
 * 
 * @Author: Бојан Богдановиќ 155002 , ФИНКИ
 */
namespace Client
{
    class ClientProgram
    {
        private static String[] commands = { "", "Приказ на мои датотеки", "Испрати датотека до друг корисник","Прими датотека", "Излези" };
        private int idNumber;
        private String password;
        private String workingDirectory;

        public ClientProgram(String workingDirectory)
        {
            Random random = new Random();
            idNumber = random.Next(10000, 99999);
            password = RandomPassword();
            this.workingDirectory = workingDirectory;
        }
        public int getIdNumber()
        {
            return idNumber;
        }
        public String getPassword()
        {
            return password;
        }
        public string RandomPassword()
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder();
            builder.Append((char)random.Next((int)'A', (int)'Z'));
            builder.Append((char)random.Next((int)'A', (int)'Z'));
            builder.Append((char)random.Next((int)'0', (int)'9'));
            builder.Append((char)random.Next((int)'0', (int)'9'));
            return builder.ToString();
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            String wd;
            while (true)
            {
                Console.WriteLine("Внеси апсолутна патека на својот работен директориум:");
                wd = @"" + Console.ReadLine();
                if (Directory.Exists(wd))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Директориумот не постои.");
                }
            }
            
            ClientProgram c = new ClientProgram(wd);
            Console.WriteLine("Твоето ID:" + c.idNumber);
            Console.WriteLine("Твојот Password:" + c.password);
            TcpClient tc = new TcpClient("127.0.0.1",8585);
            StreamReader reader = new StreamReader(tc.GetStream());
            StreamWriter writer = new StreamWriter(tc.GetStream());
            Console.WriteLine("Поврзување со серверот...");
            String infoCli = c.idNumber+","+c.password;
            writer.WriteLine(infoCli);
            writer.Flush();
            String s;
            while ((s = reader.ReadLine()) == null)
            {
            }
            Console.WriteLine(s);
            if(s.Equals("Најавата е успешна."))
            {
                while(true)
                {
                    Console.WriteLine();
                    Console.WriteLine("[МЕНИ]\nВнеси го редниот број на командата која сакаш да ја извршиш");
                    for(int i=1;i<commands.Length;i++)
                    {
                        Console.WriteLine("[" + i + "] " + commands[i]);
                    }
                    Console.WriteLine();
                    int choice = Convert.ToInt32(Console.ReadLine());
                    if(choice == 1)
                    {
                        DirectoryInfo dir = new DirectoryInfo(c.workingDirectory);
                        FileInfo[] files = dir.GetFiles();
                        Console.WriteLine("- МОИ ДАТОТЕКИ -");
                        int j;
                        for(int i=0;i<files.Length;i++)
                        {
                            j = i + 1;
                            Console.WriteLine("[" + j + "] " + files[i].Name);
                        }
                    }
                    else if (choice == 2)
                    {
                        Console.WriteLine("Внеси ID на другиот корисник");
                        int friendId = Convert.ToInt32(Console.ReadLine());
                        writer.WriteLine(friendId);
                        writer.Flush();
                        String msg = reader.ReadLine();
                        if(msg.Equals("ПОСТОИ"))
                        {
                            Console.WriteLine("Внесете Password на другиот корисник");
                            String friendPwd = Console.ReadLine();
                            writer.WriteLine(friendPwd);
                            writer.Flush();
                            String msg2 = reader.ReadLine();
                            if(msg2.Equals("ТОЧЕН"))
                            {
                                Console.WriteLine("Успешна автентикација на другиот корисник");
                                Console.WriteLine("Внесете реден број на датотеката што сакате да ја испратите");
                                int noFile = Convert.ToInt32(Console.ReadLine());
                                DirectoryInfo dir = new DirectoryInfo(c.workingDirectory);
                                FileInfo[] files = dir.GetFiles();
                                StreamReader fileReader = new StreamReader(files[noFile - 1].FullName);
                                String row;
                                Console.WriteLine("Испраќањето е започнато...");
                                row = files[noFile - 1].Name;
                                Console.WriteLine(row);
                                writer.WriteLine(row);
                                writer.Flush();
                                while((row = fileReader.ReadLine()) != null)
                                {
                                    writer.WriteLine(row);
                                    //Console.WriteLine(row);
                                    writer.Flush();
                                }
                                fileReader.Close();
                                Console.WriteLine("Испраќањето е завршено.");

                            }
                            else
                            {
                                Console.WriteLine(">Внесовте погрешна лозинка!");
                            }
                        
                        }
                        else
                        {
                            Console.WriteLine(">Корисник со внесеното ID не е логиран на серверот!");
                        }
                    }
                    else if (choice == 3)
                    {
                        Console.WriteLine("Примањето на датотека е започнато...");
                        String dirName;
                        while ((dirName = reader.ReadLine()) == null)
                        {
                        }
                        //String dirName = reader.ReadLine();
                        Console.WriteLine(dirName);
                        FileInfo file = new FileInfo(Path.Combine(c.workingDirectory,dirName));
                        Console.WriteLine("Pocnuvam so primanje");
                        using (Stream stream = file.OpenWrite())
                        using (StreamWriter writerFile = new StreamWriter(stream))
                        {
                            String row;
                            while (!(row = reader.ReadLine()).Equals("END"))
                            {
                                writerFile.WriteLine(row);

                            }
                            writerFile.Flush();

                        }
                        Console.WriteLine("Примањето на датотека е завршено");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Крај.");
            }
           
        }
    }
}
