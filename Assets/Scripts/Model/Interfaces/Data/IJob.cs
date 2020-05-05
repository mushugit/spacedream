namespace Assets.Scripts.Model.Interfaces.Data
{
    public interface IJob
    {
        bool Assigned { get; }

        void Execute();
        bool Doable();
    }
}
