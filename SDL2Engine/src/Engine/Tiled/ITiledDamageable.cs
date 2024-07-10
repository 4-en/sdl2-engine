using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Tiled
{
    public interface ITiledDamageable
    {
        void Damage(int damage);
    }
}
