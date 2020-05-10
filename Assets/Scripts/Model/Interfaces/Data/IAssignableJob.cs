namespace Assets.Scripts.Model.Interfaces.Data
{
    public interface IAssignableJob : IJob
    {
        void Assign();
        void Unassign();
    }
}
