using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootEmUp.Entities
{
    public interface IDamageable
    {
        void Damage(double damage);
        void Heal(double heal);
    }


}
