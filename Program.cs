using System;
using System.Collections.Generic;

namespace War
{
    internal class Program
    {
        static void Main(string[] args)
        {
            War war = new War();
            war.Work();
        }
    }

    class War
    {
        public void Work()
        {
            const string CommandPlay = "1";
            const string CommandExit = "exit";

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine($"Введите {CommandPlay}, чтобы начать играть.\n" +
                    $"Введите {CommandExit}, чтобы выйти.");
                string userInput = Console.ReadLine();

                Console.Clear();

                switch (userInput)
                {
                    case CommandPlay:
                        Play();
                        break;

                    case CommandExit:
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Такой команды нет.");
                        break;
                }
            }
        }

        private void Play()
        {
            int platoonsAmount = 2;

            Platoon[] platoons = new Platoon[platoonsAmount];

            for (int i = 0; i < platoonsAmount; i++)
            {
                platoons[i] = new Platoon();
                platoons[i].ShowStats();
                Console.WriteLine();
            }

            Fight(platoons);
        }

        private void Fight(Platoon[] platoons)
        {
            Platoon firstPlatoon = platoons[0];
            Platoon secondPlatoon = platoons[1];

            int numberRound = 0;

            while (firstPlatoon.Count > 0 && secondPlatoon.Count > 0)
            {
                numberRound++;

                Console.WriteLine($"Раунд {numberRound}");

                Console.ForegroundColor = ConsoleColor.Green;
                firstPlatoon.Attack(secondPlatoon);

                Console.ForegroundColor = ConsoleColor.Red;
                secondPlatoon.Attack(firstPlatoon);

                Console.ResetColor();
            }

            Console.WriteLine($"\nБой закончился в {numberRound} раунде.\n");

            DetermineOfWinner(firstPlatoon, secondPlatoon);
        }

        private void DetermineOfWinner(Platoon firstPlatoon, Platoon secondPlatoon)
        {
            if (firstPlatoon.Count > 0 && secondPlatoon.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nБой окончен, победил первый взвод:");
                firstPlatoon.ShowStats();
            }
            else if (firstPlatoon.Count == 0 && secondPlatoon.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nБой окончен, победил второй взвод:");
                secondPlatoon.ShowStats();
            }
            else
            {
                Console.WriteLine("\nБоевая ничья, оба взвода погибли.");
            }

            Console.ReadLine();
        }
    }

    class Platoon
    {
        private List<Soldier> _soldiers;

        public Platoon()
        {
            _soldiers = GenerateSoldiers();
        }

        public int Count => _soldiers.Count;

        public void Attack(Platoon platoon)
        {
            if (platoon.Count != 0)
            {
                for (int i = 0; i < _soldiers.Count; i++)
                {
                    _soldiers[i].Attack(platoon.GetCopyListSoldiers());
                }

                platoon.DeleteDeads();
            }
            else
            {
                Console.WriteLine("Взвод погиб.");
            }
        }

        public List<Soldier> GetCopyListSoldiers()
        {
            List<Soldier> soldiers = new List<Soldier>();

            foreach (Soldier soldier in _soldiers)
                soldiers.Add(soldier);

            return soldiers;
        }

        public void ShowStats()
        {
            foreach (Soldier soldier in _soldiers)
                soldier.ShowStats();
        }

        private void DeleteDeads()
        {
            for (int i = 0; i < _soldiers.Count; i++)
            {
                if (_soldiers[i].Health <= 0)
                {
                    _soldiers[i].ShowMessageDead();

                    _soldiers.Remove(_soldiers[i]);
                }
            }
        }

        private List<Soldier> GenerateSoldiers()
        {
            int platoonStrength = 15;
            int unitRatio = 6;

            int numberSniper = platoonStrength / unitRatio;
            int numberGrenadier = platoonStrength / unitRatio;
            int numberAutomaticRifleman = platoonStrength / unitRatio;
            int numberSoldier = platoonStrength - numberSniper - numberGrenadier - numberAutomaticRifleman;

            List<Soldier> soldiers = new List<Soldier>();

            for (int i = 0; i < numberSoldier; i++)
                soldiers.Add(new Soldier());

            for (int i = 0; i < numberSniper; i++)
                soldiers.Add(new Sniper());

            for (int i = 0; i < numberGrenadier; i++)
                soldiers.Add(new Grenadier());

            for (int i = 0; i < numberAutomaticRifleman; i++)
                soldiers.Add(new AutomaticRifleman());

            return soldiers;
        }
    }

    class CreatorSoldier
    {
        protected string Name;
        protected int Damage;
        protected int Armor = 1;

        public CreatorSoldier()
        {
            Name = GetName();

            int minLimitHealth = 15;
            int maxLimitHealth = 20;

            int minLimitDamage = 1;
            int maxLimitDamage = 5;

            Health = UserUtils.GenerateRandomNumber(minLimitHealth, maxLimitHealth);
            Damage = UserUtils.GenerateRandomNumber(minLimitDamage, maxLimitDamage);
        }

        public int Health { get; protected set; }

        private string GetName()
        {
            List<string> names = new List<string>() { "Крис Тэйлор", "Боб Барнс", "Элиас Гродин", "Ред О’Нил", "Кинг", "Банни", "Текс", "Менни", "Гарднер", "Кроуфорд", "Гейтор Лернер" };

            return names[UserUtils.GenerateRandomNumber(0, names.Count - 1)];
        }
    }

    class Soldier : CreatorSoldier
    {
        public virtual void Attack(List<Soldier> soldiers)
        {
            soldiers[UserUtils.GenerateRandomNumber(0, soldiers.Count)].TakeDamage(Damage);
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= (damage - Armor);

            if (Health > 0)
                ShowMessageWound();
            else
                ShowMessageDead();
        }

        public void ShowStats()
        {
            Console.WriteLine($"Боец - {Name} имеет {Health} здоровья и наносит {Damage} базового урона.");
        }

        public void ShowMessageDead()
        {
            Console.WriteLine($"{Name} - погиб.");
        }

        private void ShowMessageWound()
        {
            Console.WriteLine($"{Name} - получил ранение, у него осталось {Health}.");
        }
    }

    class Sniper : Soldier
    {
        private int _increaseDamage = 2;

        public override void Attack(List<Soldier> soldiers)
        {
            soldiers[UserUtils.GenerateRandomNumber(0, soldiers.Count)].TakeDamage(Damage * _increaseDamage);
        }
    }

    class Grenadier : Soldier
    {
        private int _numberOfAttacked = 3;

        public override void Attack(List<Soldier> soldiers)
        {
            List<int> usedIndex = new List<int>();

            if (soldiers.Count > 3)
            {
                while (usedIndex.Count != _numberOfAttacked)
                {
                    int index = UserUtils.GenerateRandomNumber(0, soldiers.Count);

                    if (usedIndex.Contains(index) == false)
                    {
                        usedIndex.Add(index);
                        soldiers[index].TakeDamage(Damage);
                    }
                }
            }
            else
            {
                foreach (Soldier soldier in soldiers)
                    soldier.TakeDamage(Damage);
            }
        }
    }

    class AutomaticRifleman : Soldier
    {
        private int _numberOfAttacked = 3;

        public override void Attack(List<Soldier> soldier)
        {
            for (int i = 0; i < _numberOfAttacked; i++)
                soldier[UserUtils.GenerateRandomNumber(0, soldier.Count)].TakeDamage(Damage);
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }
    }
}
