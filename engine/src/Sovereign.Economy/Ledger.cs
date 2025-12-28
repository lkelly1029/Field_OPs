using System;
using System.Collections.Generic;
using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Economy
{
    public class Ledger
    {
        private readonly Dictionary<Guid, MoneyCents> _balances = new();
        private readonly Dictionary<ResourceType, Dictionary<Guid, long>> _resourceBalances = new();

        public IReadOnlyDictionary<Guid, MoneyCents> MonetaryBalances => _balances;
        public IReadOnlyDictionary<ResourceType, Dictionary<Guid, long>> ResourceBalances => _resourceBalances;

        public MoneyCents GetBalance(Guid accountId)
        {
            return _balances.ContainsKey(accountId) ? _balances[accountId] : new MoneyCents(0);
        }

        public void Credit(Guid accountId, MoneyCents amount)
        {
            if (!_balances.ContainsKey(accountId))
            {
                _balances[accountId] = new MoneyCents(0);
            }
            _balances[accountId] = new MoneyCents(_balances[accountId].Value + amount.Value);
        }

        public bool TryDebit(Guid accountId, MoneyCents amount)
        {
            var balance = GetBalance(accountId);
            if (balance.Value >= amount.Value)
            {
                _balances[accountId] = new MoneyCents(balance.Value - amount.Value);
                return true;
            }
            return false;
        }

        public void ForceDebit(Guid accountId, MoneyCents amount)
        {
            var balance = GetBalance(accountId);
            _balances[accountId] = new MoneyCents(balance.Value - amount.Value);
        }

        public long GetResourceBalance(Guid accountId, ResourceType type)
        {
            if (_resourceBalances.TryGetValue(type, out var balances) && balances.TryGetValue(accountId, out var balance))
            {
                return balance;
            }
            return 0;
        }

        public void CreditResource(Guid accountId, ResourceType type, long amount)
        {
            if (!_resourceBalances.ContainsKey(type))
            {
                _resourceBalances[type] = new Dictionary<Guid, long>();
            }
            if (!_resourceBalances[type].ContainsKey(accountId))
            {
                _resourceBalances[type][accountId] = 0;
            }
            _resourceBalances[type][accountId] += amount;
        }

        public bool TryDebitResource(Guid accountId, ResourceType type, long amount)
        {
            long balance = GetResourceBalance(accountId, type);
            if (balance >= amount)
            {
                _resourceBalances[type][accountId] = balance - amount;
                return true;
            }
            return false;
        }

        public void LoadState(IReadOnlyDictionary<Guid, MoneyCents> monetaryBalances, IReadOnlyDictionary<ResourceType, Dictionary<Guid, long>> resourceBalances)
        {
            _balances.Clear();
            foreach (var kvp in monetaryBalances) _balances[kvp.Key] = kvp.Value;
            _resourceBalances.Clear();
            if (resourceBalances != null) foreach (var kvp in resourceBalances) _resourceBalances[kvp.Key] = new Dictionary<Guid, long>(kvp.Value);
        }
    }
}
