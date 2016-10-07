using System;

namespace MemoryLeakExample
{
    public static class MemoryLeak
    {
        public static WeakReference Reference { get; private set; }

        public static void CheckObject(object ObjToCheck)
        {
            Reference = new WeakReference(ObjToCheck);
        }

        public static bool IsItDead()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return Reference.IsAlive;
        }
    }
}
