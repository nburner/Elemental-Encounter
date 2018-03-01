using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    using bitboard = UInt64;
    public class AIJob
    {
        #region DLL Imports
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void BasicRandom(bitboard white, bitboard black, Turn t, out int from, out int to);
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void BasicDefense(bitboard white, bitboard black, Turn t, out int from, out int to);
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void BasicOffense(bitboard white, bitboard black, Turn t, out int from, out int to);
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void DarylsPet(bitboard white, bitboard black, Turn t, out int from, out int to);
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void MemPet(bitboard white, bitboard black, Turn t, out int from, out int to, int id = 0);
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void RandomPet(bitboard white, bitboard black, Turn t, out int from, out int to);
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void Test(bitboard white, bitboard black, Turn t, out int from, out int to);
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void DarylsPrune(bitboard white, bitboard black, Turn t, out int from, out int to);
        [DllImport("Assets/Plugins/AILibrary.dll")]
        public static extern void Seeker(bitboard white, bitboard black, Turn t, out int from, out int to);
        #endregion

        public bitboard white;
        public bitboard black;
        public Turn Color;
        public AIType Type;

        public int fromX;
        public int fromY;
        public int toX;
        public int toY;

        private bool m_IsDone = false;
        private object m_Handle = new object();
        private System.Threading.Thread m_Thread = null;
        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (m_Handle)
                {
                    tmp = m_IsDone;
                }
                return tmp;
            }
            set
            {
                lock (m_Handle)
                {
                    m_IsDone = value;
                }
            }
        }

        public virtual void Start()
        {
            m_Thread = new System.Threading.Thread(Run);
            m_Thread.Start();
        }

        void ThreadFunction()
        {
            int from = 0, to = 0;
            switch (Type)
            {
                case AIType.B_OFFENSE: BasicOffense(white, black, Color, out from, out to); break;
                case AIType.B_DEFENSE: BasicDefense(white, black, Color, out from, out to); break;
                case AIType.B_RANDOM: BasicRandom(white, black, Color, out from, out to); break;
                case AIType.RANDOM_PET: RandomPet(white, black, Color, out from, out to); break;
                case AIType.DARYLS_PET: DarylsPet(white, black, Color, out from, out to); break;
                case AIType.MEM_PET: MemPet(white, black, Color, out from, out to); break;
                case AIType.TEST: Test(white, black, Color, out from, out to); break;
                case AIType.DARYLS_PRUNE: DarylsPrune(white, black, Color, out from, out to); break;
                case AIType.SEEKER: Seeker(white, black, Color, out from, out to); break;
                default: break;
            }

            toX = to % 8;
            toY = to / 8;
            fromX = from % 8;
            fromY = from / 8;
        }
        protected virtual void OnFinished() { }
        public virtual bool Update()
        {
            if (IsDone)
            {
                OnFinished();
                return true;
            }
            return false;
        }

        private void Run()
        {
            ThreadFunction();
            IsDone = true;
        }
    }
}
