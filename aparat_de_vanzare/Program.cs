using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace aparat_de_vanzare
{
    public class Coins 
    {
        public static readonly decimal[] coins = new decimal[3] { 0.05m, 0.10m, 0.25m }; // pentru a adauga noi monede acceptate
        public static readonly int[] coinsavailable = new int[3] { 0, 0, 0 }; // pentru referinta, sau daca vrem sa folosim un anumit nr de monede disponibile in aparat in intreaga functionare a acestuia.
        public static readonly decimal cost = 0.20m; // pentru a schimba costul produsului
        public static readonly decimal balance = 3m; // pentru a schimba banii de buzunar
    }

    public class Program
    {
        public static readonly decimal nickel = Coins.coins[0];
        public static readonly decimal dime = Coins.coins[1];
        public static readonly decimal quarter = Coins.coins[2];
        public static readonly decimal cost = Coins.cost;
        public static decimal balance = Coins.balance;
        public static int[] coinsavailable = Coins.coinsavailable;
        static void Main(string[] args)
        {           
            int merch = 0;
            decimal machinebalance = 0;
            Interface(ref machinebalance, ref merch, ref balance);
            while (balance > 0)
            {
                //coinsavailable = { 0 , 0 , 0 };
                Console.WriteLine();
                Console.WriteLine("Introduceti o moneda! ( Q: 0.25 | D: 0.10 | N: 0.05 ).");
                string Button = Console.ReadLine();
                Console.Clear();
                switch (Button)
                {
                    case "Q":
                        State(quarter,ref machinebalance,ref merch, ref balance);
                        Interface(ref machinebalance, ref merch, ref balance);
                        break;
                    case "D":
                        State(dime, ref machinebalance, ref merch, ref balance);
                        Interface(ref machinebalance, ref merch, ref balance);
                        break;
                    case "N":
                        State(nickel, ref machinebalance, ref merch, ref balance);
                        Interface(ref machinebalance, ref merch, ref balance);
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("Ai ramas fara bani , dar ai primit " + merch + " produse");
        }
        private static void State(decimal input, ref decimal machinebalance, ref int merch, ref decimal balance)
        {
            // maxrest : 1 * dime + 1 * nickel;
            Console.WriteLine($"Ati introdus: {input}");
            machinebalance += input;
            if (balance - input < 0)
            {
                Console.WriteLine($"N-ai destui bani sa introduci!");
                return;
            }
            balance -= input;           
            if (machinebalance >= cost) // returneaza produs
            {
                merch++;
                Console.WriteLine("Ati primit un produs.");
                machinebalance -= cost;
                RestCalc(ref balance, ref machinebalance);
                return;
            }
            if (machinebalance < cost) // nu returneaza produs si asteapta bani
            {
                Console.WriteLine($"Mai introduceti monede pentru a primi produsul!{Environment.NewLine}");
                return;
            }
        }
        public static void RestCalc(ref decimal balance, ref decimal machinebalance) // calculeaza cat rest trebuie sa dea / cat poate da
        {
            decimal r;
            r = machinebalance;
            int[] coinsgiven = new int[3] { 0, 0, 0 }; // monede date ca rest
            int[] coinsavailable = new int[3] { 1, 1 ,0 }; //  { nickels, dimes , quarters } 
            // monede disponibile de dat ca rest intr-un ciclu.     ( se comenteaza linia de sus si se inlocuiesc valorile din clasa coins pentru a da monede disponibile totale in intreaga functiune a aparatului).       
            for (int i = 0; i < Coins.coins.Length; i++)
            {
                while (r > 0 && coinsavailable[i] > 0)
                {
                    coinsgiven[i]++;
                    coinsavailable[i]--;
                    r -= Coins.coins[i];
                }
            }
            Console.WriteLine($"Aparatul a dat rest: {coinsgiven[0]} x 0.05 (nickel) si {coinsgiven[1]} x 0.10 (dime).{Environment.NewLine}");
            
            // cand exista mai multe tipuri de monede:
            /*
            Console.WriteLine();
            Console.Write($"Aparatul a dat rest:");           
            for (int j = 0; j < Coins.coins.Length; j++)
            {
                if(coinsgiven[j] > 0)
                {
                Console.Write($" {coinsgiven[j]} x {Coins.coins[j]} ,");
                }
            }
            Console.WriteLine();
            
            for(int i = 0; i < Coins.coins.Length; i++)
            {
                balance += coinsgiven[i] * Coins.coins[i];
            }
            */
                
            balance += coinsgiven[0] * nickel + coinsgiven[1] * dime; // se aduna restul la balanta
            machinebalance = r; // se actualizeaza banii ramasi in aparat.
            return;
        }
        public static void Interface (ref decimal machinebalance, ref int merch, ref decimal balance)
        {
            Console.WriteLine($"Bani in buzunar: {balance}");
            Console.WriteLine($"Produse primite: {merch}");
            Console.WriteLine($"Bani ramasi in aparat: {machinebalance}");
        }
    }
}
