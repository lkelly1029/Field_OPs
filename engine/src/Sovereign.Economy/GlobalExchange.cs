using System;
using System.Collections.Generic;
using Sovereign.Core;
using Sovereign.Core.Primitives;

namespace Sovereign.Economy
{
    public struct ResourceOffer
    {
        public Guid SellerUniverseId { get; set; }
        public ResourceQuantity Quantity { get; set; }
        public MoneyCents PricePerUnit { get; set; }
    }

    public class GlobalExchange
    {
        private readonly Dictionary<ResourceType, ResourceOffer> _bestOffers = new();
        private readonly Dictionary<ResourceType, long> _aiPrices = new();
        private readonly Random _random = new Random();

        public const int MVP_DISTANCE_KM = 100;
        public const double LOSS_PER_100KM = 0.01; // 1%
        public const long FEE_CENTS_PER_10KM_UNIT = 1;

        public GlobalExchange()
        {
            // Initialize Base Prices
            _aiPrices[ResourceType.Power] = 2;
            _aiPrices[ResourceType.Water] = 5;
            _aiPrices[ResourceType.Food] = 10;
            _aiPrices[ResourceType.Steel] = 50;
            _aiPrices[ResourceType.Iron] = 15;
        }

        public void Tick()
        {
            // Simulate Volatility (Random Walk)
            // Create a list of keys to avoid modification errors
            var keys = new List<ResourceType>(_aiPrices.Keys);
            foreach (var type in keys)
            {
                long current = _aiPrices[type];
                // Fluctuate by -1, 0, or +1
                int delta = _random.Next(-1, 2); 
                
                // 10% Chance of larger shock (-5 to +5)
                if (_random.NextDouble() < 0.1) delta = _random.Next(-5, 6);

                long next = Math.Max(1, current + delta);
                _aiPrices[type] = next;
            }
        }

        public long GetAiPrice(ResourceType type)
        {
            return _aiPrices.TryGetValue(type, out long price) ? price : 100;
        }

        public void ListOffer(ResourceOffer offer)
        {
            _bestOffers[offer.Quantity.Type] = offer;
        }

        public bool TryBuy(ResourceQuantity quantity, MoneyCents maxPrice, out ResourceOffer offer)
        {
            ResourceType type = quantity.Type;

            // 1. Check Exchange
            if (_bestOffers.TryGetValue(type, out var best) &&
                best.Quantity.Value >= quantity.Value &&
                best.PricePerUnit.Value <= maxPrice.Value)
            {
                offer = best;
                return true;
            }

            // 2. Fallback to AI (Dynamic Prices)
            long aiPriceValue = GetAiPrice(type);
            var aiPrice = new MoneyCents(aiPriceValue);

            if (aiPrice.Value <= maxPrice.Value)
            {
                offer = new ResourceOffer
                {
                    SellerUniverseId = Guid.Empty, // AI Market
                    Quantity = quantity,
                    PricePerUnit = aiPrice
                };
                return true;
            }

            offer = default;
            return false;
        }

        public static void CalculateTransport(long quantity, int distanceKm, out MoneyCents fee, out long loss)
        {
            // Fee: 1 cent per 10km per unit
            long feeValue = (distanceKm / 10) * FEE_CENTS_PER_10KM_UNIT * quantity;
            fee = new MoneyCents(feeValue);

            // Loss: 1% per 100km
            double lossRatio = (distanceKm / 100.0) * LOSS_PER_100KM;
            loss = (long)(quantity * lossRatio);
        }

        public MarketSnapshot GetSnapshot(long tick)
        {
            var snapshot = new MarketSnapshot { Tick = tick };

            foreach (var kvp in _bestOffers)
            {
                var offer = kvp.Value;
                var metrics = new ResourceMarketMetrics
                {
                    BestPrice = offer.PricePerUnit,
                    TotalVolume = offer.Quantity.Value,
                    OfferCount = 1,
                    AiSharePct = 0,
                    PlayerSharePct = 100
                };
                metrics.TopOffers.Add(offer);
                snapshot.Metrics[kvp.Key] = metrics;
            }

            return snapshot;
        }
    }
}