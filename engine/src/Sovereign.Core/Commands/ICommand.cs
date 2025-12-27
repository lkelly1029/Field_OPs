namespace Sovereign.Core.Commands
{
    public interface ICommand
    {
        void Execute(Sim.Universe universe);
    }
}
