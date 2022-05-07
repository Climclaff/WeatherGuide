using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IotEmul
{
    public class Sensor
    {
        private static readonly HttpClient client = new HttpClient();

        public Sensor(int countryId, int stateId)
        {
            CountryId = countryId;
            StateId = stateId;
            IsWorking = false;
        }

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public bool IsWorking { get; set; }

        public async void SendData()
        {
            Random rnd = new Random();           
            IotMeasurement measurement = new IotMeasurement();
            measurement.Temperature = rnd.Next(-20, 40); 
            measurement.WindSpeed = rnd.Next(1, 26); 
            measurement.Humidity = rnd.Next(0, 100); 
            measurement.DateTime = DateTime.UtcNow;
            measurement.CountryId = CountryId;
            measurement.StateId = StateId;
            var content = JsonConvert.SerializeObject(measurement);
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = client.PostAsync("https://localhost:44302/api/Iot/SendMeasurement", byteContent).Result;

            var responseString = await result.Content.ReadAsStringAsync();
        }
    }
    public class IotMeasurement
    {
        public int Temperature { get; set; }

        public int Humidity { get; set; }

        public int WindSpeed { get; set; }

        public DateTime DateTime { get; set; }

        public int CountryId { get; set; }

        public int StateId { get; set; }
    }
    class Program
    {
        static void InitializeSensors(List<Sensor> sensors)
        {
            sensors.Add(new Sensor(1, 1));
            sensors.Add(new Sensor(1, 2));
            sensors.Add(new Sensor(2, 3));
            sensors.Add(new Sensor(2, 4));
            sensors.Add(new Sensor(2, 5));
            sensors.Add(new Sensor(4, 6));
            sensors.Add(new Sensor(4, 7));
            sensors.Add(new Sensor(4, 8));
        }
        static void PrintMenu()
        {
            Console.WriteLine("Консоль управління IoT пристроями, оберіть операцію:");
            Console.WriteLine("1: Вивести на екран усі доступні пристрої");
            Console.WriteLine("2: Увімкнути/Вимкнути пристрій");
            Console.WriteLine("3: Перемкнути усі пристрої");
        }


        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;
            List<Sensor> sensors = new List<Sensor>();          
            InitializeSensors(sensors);
            Task task = new Task(() =>
            {
                while (true)
                {
                    for (int i = 0; i < sensors.Count; ++i)
                    {
                        if (sensors[i].IsWorking == true)
                        {
                            sensors[i].SendData();
                            Console.WriteLine("Знімаються покази з сенсору[" + i + "] " + "Країна: " + sensors[i].CountryId + " Регіон: " + sensors[i].StateId);
                            Console.WriteLine();
                        }
                    }
                    Thread.Sleep(5000);
                }
            });
            task.Start(); 
            while (true)
            {             
                PrintMenu();
                waitForInput:
                int resultIndex = -1;
                string input = Console.ReadLine();
                if (input == "1")
                {
                    for (int i = 0; i < sensors.Count; ++i)
                    {
                        Console.WriteLine("Сенсор[" + i + "] " + "Країна: " + sensors[i].CountryId + " Регіон: " + sensors[i].StateId + " Активний: " + sensors[i].IsWorking);
                    }
                    Console.WriteLine("Продовжити роботу?");
                    Console.ReadLine();
                    continue;
                }
                if (input == "2")
                {
                    Console.WriteLine("Введіть порядковий номер(індекс) пристрою");
                    string index = Console.ReadLine();
                    try
                    {
                        resultIndex = int.Parse(index);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"Невірний формант '{index}'");
                        continue;
                    }
                    if (resultIndex >= 0 && resultIndex < sensors.Count)
                    {                       
                        if (sensors[resultIndex].IsWorking == true)
                        {
                            Console.WriteLine("Пристрій Сенсор[" + resultIndex + "] був вимкнутий");
                            sensors[resultIndex].IsWorking = false;
                        }
                        else
                        {
                            Console.WriteLine("Пристрій Сенсор[" + resultIndex + "] був увімкнений");
                            sensors[resultIndex].IsWorking = true;
                        }
                        Console.WriteLine("Продовжити роботу?");
                        Console.ReadLine();
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Невірний індекс");
                        Console.WriteLine("Продовжити роботу?");
                        Console.ReadLine();
                        continue;
                    }

                }
                if (input == "3")
                {
                    for (int i =0; i<sensors.Count; ++i)
                    {
                        if (sensors[i].IsWorking == true)
                        {
                            sensors[i].IsWorking = false;
                        }
                        else
                        {
                            sensors[i].IsWorking = true;
                        }
                    }
                    Console.WriteLine("Усі пристрої було перемкнуто");
                    Console.WriteLine("Продовжити роботу?");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    goto waitForInput;
                }
            }
        }
    }
}

