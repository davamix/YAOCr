using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAOCr.Plugins;

public interface IPlugin {
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
}
