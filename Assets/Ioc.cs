using Assets.Scripts.Model.Interfaces;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.Model.Interfaces.Services.WorldGenerator;
using Assets.Scripts.Model.Services;
using Assets.Scripts.Model.Services.DataServices;
using Assets.Scripts.Model.Services.WorldGenerators;
using LightInject;

namespace Assets
{
    public static class Ioc
    {
        public static void RegisterServices(this ServiceContainer container)
        {
            //Main service
            container.Register<IGameService, GameService>();

            //Services
            container.Register<IEventService, EventService>();
            container.Register<ITileService, TileService>();

            //Singleton Services
            container.RegisterSingleton<IWorldService, WorldService>();
            container.RegisterSingleton<IBuildService, BuildService>();

            //Data services
            container.RegisterSingleton<IMapDataService, MapDataService>();

            //World Generators
            container.Register<IWorldGeneratorService, WorldGeneratorBaseService>("WorldGeneratorBaseService");

        }
    }
}
