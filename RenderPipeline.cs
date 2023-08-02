using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKBase
{
    public abstract class RenderPipeline
    {
        public abstract void Render(Scene scene);
    }
}
