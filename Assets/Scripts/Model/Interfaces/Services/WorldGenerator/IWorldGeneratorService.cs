using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.Interfaces.Services.WorldGenerator
{
    public interface IWorldGeneratorService
    {
        int Order { get; }
        void Generate(IWorldService worldservice, bool debugGeneratorTime);
    }
}
