using Assets.Scripts.Model.Data.Jobs.Parameters;

namespace Assets.Scripts.Model.Interfaces.Data
{
    public interface IJob
    {
        JobParameter Parameter { get; }

        bool Doable();
    }
}
