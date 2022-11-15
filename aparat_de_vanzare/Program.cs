using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace aparat_de_vanzare
{
    abstract public class Coins 
    {
        public static int[] coins; // pentru a adauga noi monede acceptate
        public static int[] coinsavailable;
        public static int[] coinsstock;
        public static int cost; // pentru a schimba costul produsului // pentru a schimba banii de buzunar
        public static int balance;
        public static bool refreshstock = false;
    }
    public class CoinsConfig : Coins
    {
        public class Type
        {
            public static void BalanceSet(int amount)
            {
                balance = amount;
            }
            public static void CoinsOverride()
            {
                Console.WriteLine("~~Configurare monede acceptate~~");
                Console.WriteLine("Introduceti valorile tuturor monedelor acceptate cu ',' intre ele!");
                string line = Console.ReadLine();
                line.Replace(" ", "");
                char[] sep = { ',' };
                string[] tokens = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                SortedSet<int> tokens2 = new SortedSet<int>();
                foreach(string z in tokens)
                {
                    tokens2.Add(int.Parse(z));
                }
                coins = tokens2.ToArray();
            }
            public static void DefaultCoins()
            {
                coins = new int[3];
                Coins.coins[0] = 5;
                Coins.coins[1] = 10;
                Coins.coins[2] = 25;
            }
            public static void DefaultCost()
            {
                cost = 20;
            }
            public static void CostOverride()
            {
                Console.Write("~~Introduceti costul produsului: ");
                cost = int.Parse(Console.ReadLine());
                Console.WriteLine();
            }
            public static void DispensableCoinsDefault()
            {
                coinsavailable = new int[] { 1, 1, 0 };
                coinsstock = new int[coinsavailable.Length];
                coinsstock = coinsavailable;
                refreshstock = true;
            }
            public static void DispensableCoinsOverride()
            {
                Console.WriteLine("~~Configurare stoc de monede in aparat:~~");
                coinsavailable = new int[Coins.coins.Length];
                for (int i = 0; i < coinsavailable.Length; i++)
                {
                    Console.Write($"{Coins.coins[i]}c :");
                    coinsavailable[i] = int.Parse(Console.ReadLine());
                }
                Console.WriteLine("Doriti ca stocul de monede sa se reactualizeze dupa fiecare ciclu de produse dispensat?");
                bool check = true;
                while (check)
                {
                    string response = Console.ReadLine();
                    response.ToLower();
                    switch (response)
                    {
                        case "da":
                            refreshstock = true;
                            check = false;
                            return;
                        case "nu":
                            return;
                        default: Console.WriteLine("'da' sau 'nu'"); break;
                    }
                }
                if (refreshstock)
                {
                    coinsstock = new int[coinsavailable.Length];
                    coinsstock = coinsavailable;
                }
            }
            public static void Defaults()
            {
                BalanceSet(300);
                DefaultCoins();
                DispensableCoinsDefault();               
                DefaultCost();
            }
            public static void Overrides()
            {
                Console.Write("~~ Bani initiali = ");
                BalanceSet(int.Parse(Console.ReadLine()));
                CoinsOverride();
                DispensableCoinsOverride();               
                CostOverride();                
            }
        }
        public class Setup
        {
            public static void I_Setup()
            {
                Console.WriteLine("~~ Doriti sa configurati manual aparatul? ( daca nu aparatul ruleaza cu setari implicite ) ~~");
                bool check = true;
                while (check)
                {
                    string input = Console.ReadLine();
                    input.ToLower();
                    switch (input)
                    {
                        case "da": Type.Overrides();check = false; break;
                        case "nu": Type.Defaults();check = false; break;
                        default: Console.WriteLine("da sau nu");break;
                    }
                }
            }
            public static void I_Setup(int i)
            {
                Type.Defaults();
            }
        }
    }
    public class Aparat : Coins
    {
        public static bool working = true;
        public static void Work()
        {
            int merch = 0;
            int machinebalance = 0;
            Interface(ref machinebalance, ref merch);
            while (working)
            {
                Console.WriteLine();
                Console.Write("Introduceti o moneda! Monedele suportate de masina sunt: ");
                GetCoins();
                int Button = UserInput();
                Console.Clear();
                Handle(Button, ref machinebalance, ref merch);
                
            }
            Console.WriteLine("Ai ramas fara bani , dar ai primit " + merch + " produse");
        }
        public static int UserInput()
        {
            int Button = int.Parse(Console.ReadLine());
            if (!coins.Contains(Button))
            {
                Console.WriteLine("Moneda dvs nu este suportata de aparat! Incercati alta moneda.");
                Button = UserInput();
            }
            return Button;
        }
        public static void Handle(int input, ref int machinebalance, ref int merch)
        {
            State(input, ref machinebalance, ref merch);
            Interface(ref machinebalance, ref merch);
        }
        public static void State(int input, ref int machinebalance, ref int merch)
        {
            Console.WriteLine($"Ati introdus: {input}");
            if (balance == 0)
            {
                working = false;
            }
            if (balance - input < 0)
            {
                Console.WriteLine($"N-ai destui bani sa introduci!");
                return;
            }
            machinebalance += input;
            balance -= input;
            if (machinebalance >= cost) // returneaza produs
            {
                merch++;
                Console.WriteLine("Ati primit un produs.");
                machinebalance -= cost;
                RestCalc(ref machinebalance, ref coinsstock);
                return;
            }
            if (machinebalance < cost) // nu returneaza produs si asteapta bani
            {
                Console.WriteLine($"Mai introduceti monede pentru a primi produsul!{Environment.NewLine}");
                return;
            }
        }
        public static void RestCalc(ref int machinebalance, ref int[] coinsstock) // calculeaza cat rest trebuie sa dea / cat poate da
        {
            int r;
            r = machinebalance;
            int CL = GetCoinsLength();
            int[] coinsgiven = new int[CL]; // tablout pt monede date ca rest in acest ciclu
            if (refreshstock)
            {
                coinsavailable = coinsstock;
                for (int i = 0; i < coinsavailable.Length; i++)
                {
                    Console.WriteLine(coinsstock[i] + "s");
                }
            }
            for (int i = CL - 1; i >= 0; i--)
            {
                while (r > 0 && coinsavailable[i] > 0)
                {
                    if ((r - Coins.coins[i]) >= 0)
                    {
                        coinsgiven[i]++;
                        coinsavailable[i]--;
                        r -= Coins.coins[i];
                    }
                    else break;
                }               
            }
            Console.WriteLine();
            Console.Write($"Restul este:");           
            for (int j = 0; j < CL; j++)
            {
                if(coinsgiven[j] > 0)
                {
                Console.Write($" {coinsgiven[j]} x {Coins.coins[j]}c ,");
                machinebalance -= coinsgiven[j] * Coins.coins[j];
                balance += coinsgiven[j] * Coins.coins[j];
                }
            }
            Console.WriteLine();
            return;
        }

        private static void GetCoins()
        {
            foreach (int coin in coins)
            {
                Console.Write(coin + "c ");
            }
            Console.Write($"; costul unui produs este {cost}");
            Console.WriteLine();
        }
        public static int GetCoinsLength()
        {
            return Coins.coins.Length;
        }

        public static void Interface(ref int machinebalance, ref int merch)
        {
            Console.WriteLine();
            Console.WriteLine($"Centi in buzunar: {balance}");
            Console.WriteLine($"Produse primite: {merch}");
            Console.WriteLine($"Bani ramasi in aparat: {machinebalance}");
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            CoinsConfig.Setup.I_Setup();           
            Aparat.Work();
        }
    }
}
