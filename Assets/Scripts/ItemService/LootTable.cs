using System;

namespace ItemService
{
    public class LootTable
    {
        private readonly int _minMoney;
        private readonly int _maxMoney;

        public LootTable(int minMoney = 20, int maxMoney = 100)
        {
            _minMoney = minMoney;
            _maxMoney = maxMoney;
        }

        public int GenerateMoney(int seed)
        {
            var random = new Random(seed);
            return random.Next(_minMoney, _maxMoney + 1);
        }
    }
}
