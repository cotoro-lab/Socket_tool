using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace socket_client
{
    class client
    {
        static void Main()
        {
            IConfigurationBuilder client_info = new ConfigurationBuilder();
            client_info.SetBasePath(Directory.GetCurrentDirectory());
            client_info.AddJsonFile("client_setting.json", true, true);
            IConfiguration configuration = client_info.Build();

            IConfigurationSection section = configuration.GetSection("ClientSettingInfo");
            string ip = section["ip"];
            string port = section["port"];


            IPAddress host1 = IPAddress.Parse(ip);
            int port1 = Int32.Parse(port);
            IPEndPoint ipe1 = new IPEndPoint(host1, port1);
            string line = "";
            byte[] buf1, buf2 = new byte[1024];
            Regex reg = new Regex("\0");
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect(ipe1);
                    using (var stream = client.GetStream())
                    {
                        while(line != "bye")
                        {
                            // 標準入力からデータ取得
                            Console.WriteLine("--------------------------");
                            Console.WriteLine("偶数の数値を入力してください。");
                            Console.WriteLine("--------------------------");

                            // サーバに送信
                            line = Console.ReadLine();
                            buf1 = Encoding.UTF8.GetBytes(line);
                            stream.Write(buf1, 0, buf1.Length);

                            // サーバから受信
                            stream.Read(buf2, 0, buf2.Length);
                            Console.WriteLine(reg.Replace(Encoding.UTF8.GetString(buf2), ""));
                            Array.Clear(buf2, 0, buf2.Length);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("クライアント側終了です。");

            }
        }
    }
}
