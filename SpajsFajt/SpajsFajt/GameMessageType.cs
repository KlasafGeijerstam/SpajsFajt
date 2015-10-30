using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    enum GameMessageType
    {
        ClientID = 0,
        ClientReady = 1,
        ClientUpdate = 2,
        ClientPosition = 3,
        ProjectileRequest = 4,
        ObjectUpdate = 5,
        ObjectDeleted = 6,
        HPUpdate = 7,
        PlayerDead = 8,
        PlayerRespawn = 9,
        PowerUpdate = 10
    };
}
