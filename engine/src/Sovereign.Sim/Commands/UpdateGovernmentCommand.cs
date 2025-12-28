using Sovereign.Sim;

namespace Sovereign.Sim.Commands
{
    public class UpdateGovernmentCommand : ICommand
    {
        public long? UniversalBasicIncomeCents { get; }
        public double? CorporateTaxRate { get; }
        public double? IncomeTaxRate { get; }

        public UpdateGovernmentCommand(long? ubi = null, double? corpTax = null, double? incomeTax = null)
        {
            UniversalBasicIncomeCents = ubi;
            CorporateTaxRate = corpTax;
            IncomeTaxRate = incomeTax;
        }

        public void Execute(Universe universe)
        {
            if (universe.ActiveGovernment == null) return;

            if (UniversalBasicIncomeCents.HasValue)
                universe.ActiveGovernment.UniversalBasicIncomeCents = UniversalBasicIncomeCents.Value;
            
            if (CorporateTaxRate.HasValue)
                universe.ActiveGovernment.CorporateTaxRate = CorporateTaxRate.Value;

            if (IncomeTaxRate.HasValue)
                universe.ActiveGovernment.IncomeTaxRate = IncomeTaxRate.Value;
        }
    }
}
