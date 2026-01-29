using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    public interface IGame
    {
        void Start();
        void PlayRound();
        void GameOver();
    }
}
