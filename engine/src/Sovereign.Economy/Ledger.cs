using System;
using System.Collections.Generic;
using Sovereign.Core.Primitives;

namespace Sovereign.Economy
{
    public class Ledger
    {
        private readonly Dictionary<Guid, MoneyCents> _monetaryBalances = new();
        private readonly Dictionary<ResourceType, Dictionary<Guid, long>> _resourceBalances = new();

        public MoneyCents GetBalance(Guid accountId)
        {
            return _monetaryBalances.ContainsKey(accountId) ? _monetaryBalances[accountId] : new MoneyCents(0);
        }

        public void Credit(Guid accountId, MoneyCents amount)
        {
            if (!_monetaryBalances.ContainsKey(accountId))
            {
                _monetaryBalances[accountId] = new MoneyCents(0);
            }
            _monetaryBalances[accountId] = new MoneyCents(_monetaryBalances[accountId].Value + amount.Value);
        }

        public bool TryDebit(Guid accountId, MoneyCents amount)
        {
            var balance = GetBalance(accountId);
            if (balance.Value >= amount.Value)
            {
                _monetaryBalances[accountId] = new MoneyCents(balance.Value - amount.Value);
                return true;
            }
            return false;
        }

        // --- Resource Methods ---

        public long GetResourceBalance(Guid accountId, ResourceType type)
        {
            if (_resourceBalances.TryGetValue(type, out var balances))
            {
                return balances.TryGetValue(accountId, out var balance) ? balance : 0;
            }
            return 0;
        }

        public void CreditResource(Guid accountId, ResourceQuantity quantity)
        {
            if (!_resourceBalances.TryGetValue(quantity.Type, out var balances))
            {
                balances = new Dictionary<Guid, long>();
                _resourceBalances[quantity.Type] = balances;
            }

            if (!balances.ContainsKey(accountId))
            {
                balances[accountId] = 0;
            }
            balances[accountId] += quantity.Value;
        }

        public bool TryDebitResource(Guid accountId, ResourceQuantity quantity)
        {
            var balance = GetResourceBalance(accountId, quantity.Type);
            if (balance >= quantity.Value)
            {
                _resourceBalances[quantity.Type][accountId] = balance - quantity.Value;
                return true;
            }
            return false;
        }
    }
}
