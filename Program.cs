using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Project3310_Inventory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<char> inventory = new List<char>(' ');
            int iterator = 0;
            Task.Run(async () =>
            {
                while (true)
                {

                    Console.Clear();
                    foreach (char c in inventory)
                    {
                        Console.Write(c);
                    }
                    inventory = await Receive(inventory);
                }
            });



            while (true)
            {
                if (inventory.Count > 0)
                {
                    Console.SetCursorPosition(iterator, 0);
                    ConsoleKeyInfo pressedKey = new ConsoleKeyInfo();
                    pressedKey = Console.ReadKey();

                    switch (pressedKey.Key)
                    {
                        case ConsoleKey.RightArrow:
                            if (iterator + 1 < inventory.Count)
                            {
                                iterator++;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            if (iterator > 0)
                            {
                                iterator--;
                            }
                            break;
                        case ConsoleKey.Q:
                            Console.SetCursorPosition(iterator, 0);
                            Console.Write(inventory[iterator]);
                            inventory.RemoveAt(iterator);
                            Console.Clear();
                            //TODO вызов метода Send для возвращения измененного массива инвентаря
                            break;

                    }
                    Console.SetCursorPosition(0, 0);
                    foreach (char c in inventory)
                    {
                        Console.Write(c);
                    }
                }

            }
            static async void Send(List<char> inventory)
            {
                using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                char[] message = inventory.ToArray();
                byte[] data = Encoding.UTF8.GetBytes(message);
                EndPoint remotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
                int bytes = await udpSocket.SendToAsync(data, SocketFlags.None, remotePoint);
                Console.WriteLine($"Отправлено {bytes} байт");
            }

            static async Task<List<char>> Receive(List<char> inventory)
            {

                using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
                // начинаем прослушивание входящих сообщений
                udpSocket.Bind(localIP);
                byte[] data = new byte[256]; // буфер для получаемых данных
                                             //адрес, с которого пришли данные
                EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
                // получаем данные в массив data

                var result = await udpSocket.ReceiveFromAsync(data, SocketFlags.None, remoteIp);
                char[] message = Encoding.UTF8.GetChars(data, 0, result.ReceivedBytes);

                inventory = new List<char>(message);
                return inventory;

            }
        }

    }
}