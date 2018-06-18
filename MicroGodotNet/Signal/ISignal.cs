using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet.Signal
{
    public interface ISignal<T>
    {
        void Execute(T message);
    }
}
