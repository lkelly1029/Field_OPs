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

        public const int MVP_DISTANCE_KM = 100;
        public const double LOSS_PER_100KM = 0.01; // 1%
        public const long FEE_CENTS_PER_10KM_UNIT = 1;

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

            // 2. Fallback to AI (Hardcoded for MVP, should be in config)
            long aiPriceValue = type switch
            {
                ResourceType.Power => 2,
                ResourceType.Water => 5,
                ResourceType.Food => 10,
                ResourceType.Steel => 50,
                ResourceType.Iron => 15,
                _ => 100 // Default expensive
            };
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