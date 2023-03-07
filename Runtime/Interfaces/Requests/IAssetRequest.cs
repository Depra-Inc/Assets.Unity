namespace Depra.Assets.Runtime.Interfaces.Requests
{
    public interface IAssetRequest
    {
        bool Done { get; }

        bool Running { get; }

        void Cancel();

        void Destroy();
    }
}