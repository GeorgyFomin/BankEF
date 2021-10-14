using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domain.Model
{
    public static class RandomBank
    {
        /// <summary>
        /// Хранит максимально возможную сумму вклада.
        /// </summary>
        private const double MaxSize = 1_000_000_000;

        /// <summary>
        /// Хранит максимально возможную доходность вклада в процентах.
        /// </summary>
        private const double MaxRate = 10;
        /// <summary>
        /// Хранит ссылку на генератор случайных чисел.
        /// </summary>
        static public readonly Random random = new();
        /// <summary>
        /// Возвращает случайный банк.
        /// </summary>
        /// <returns></returns>
        //static public Bank GetBank() => new Bank(GetRandomString(4, random), GetRandomDeps(random.Next(1, 5), random));
        public static ObservableCollection<Department> Deps = GetRandomDeps(random.Next(1, 5), random);
        /// <summary>
        /// Возвращает список случайных отделов.
        /// </summary>
        /// <param name="v">Число отделов.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        private static ObservableCollection<Department> GetRandomDeps(int v, Random random) =>
           new ObservableCollection<Department>(
            Enumerable.Range(0, v).
            Select(index => new Department() { Name = GetRandomString(random.Next(1, 6), random), Clients = GetRandomClients(random.Next(1, 20), random) }).
            ToList());
        /// <summary>
        /// Возвращает список случайных клиентов.
        /// </summary>
        /// <param name="v">Число клиентов.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        private static ObservableCollection<Client> GetRandomClients(int v, Random random) =>
            new ObservableCollection<Client>(Enumerable.Range(0, v).
            Select(index => new Client()
            {
                Name = GetRandomString(random.Next(3, 6), random),
                Loans = GetRandomLoans(random.Next(1, 5), random),
                Deposits = GetRandomDeposits(random.Next(1, 5), random)
            }).
            ToList());
        /// <summary>
        /// Возвращает список случайных депозитов.
        /// </summary>
        /// <param name="v">Число депозитов.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        private static ObservableCollection<Deposit> GetRandomDeposits(int v, Random random) =>
            new ObservableCollection<Deposit>(Enumerable.
            Range(0, v).
            Select(index => new Deposit()
            {
                // Случайна доходность.
                Rate = MaxRate * random.NextDouble(),
                // Случайная капитализация.
                Cap = random.NextDouble() < .5,
                // Случайный размер вклада (положительный) или кредита (отрицательный).
                Size = (decimal)(MaxSize * random.NextDouble())
            }).
            ToList());
        /// <summary>
        /// Возвращает список случайных кредитов.
        /// </summary>
        /// <param name="v">Число кредитов.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        private static ObservableCollection<Loan> GetRandomLoans(int v, Random random) =>
            new ObservableCollection<Loan>(Enumerable.
            Range(0, v).
            Select(index => new Loan()
            {
                // Случайна доходность.
                Rate = MaxRate * random.NextDouble(),
                // Случайная капитализация.
                Cap = random.NextDouble() < .5,
                // Случайный размер вклада (положительный) или кредита (отрицательный).
                Size = (decimal)(MaxSize * random.NextDouble())
            }).
            ToList());
        /// <summary>
        /// Генерирует случайную строку из латинских букв нижнего регистра..
        /// </summary>
        /// <param name="length">Длина строки.</param>
        /// <param name="random">Генератор случайных чисел.</param>
        /// <returns></returns>
        public static string GetRandomString(int length, Random random)
            => new string(Enumerable.Range(0, length).Select(x => (char)random.Next('a', 'z' + 1)).ToArray());
    }
}
