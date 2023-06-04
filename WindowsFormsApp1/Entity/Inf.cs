using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Рыжик
{
    public class Inf
    {
        public enum ActiveStatus
        {
            Run,
            Boost,
            Jump,
            Idle
        }

        public static int CatRunFrames = 8;
        public static int CatShiftFrames = 4;
        public static int CatIdleFrames = 8;
        public static int CatIdleSetAnimation = 1;
        public static int CatRunSetAnimation = 5;
        public static int CatShiftSetAnimation = 42;


        public static int DogRunFrames = 4;
        public static int DogRunSetAnimation = 2;

        public static bool isMute;
    }
}
