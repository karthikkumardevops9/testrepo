namespace MSRecordsEngine.Imaging
{
    internal partial class WordMath
    {
        internal static int HiWord(int lValue)
        {
            if ((lValue & int.MinValue + 0x00000000) == int.MinValue + 0x00000000)
                return (int)((lValue & 0x7FFF0000) / 0x10000 | 0x8000L);
            return (lValue & int.MinValue + 0x7FFF0000) / 0x10000;
        }

        internal static uint HiUWord(uint lValue)
        {
            if ((lValue & int.MinValue + 0x00000000) == int.MinValue + 0x00000000)
                return (uint)((lValue & 0x7FFF0000) / 0x10000L | 0x8000L);
            return (uint)((lValue & int.MinValue + 0x7FFF0000) / 0x10000L);
        }

        internal static int LoWord(int lValue)
        {
            if ((lValue & 0x8000) == 0x8000)
                return (int)(lValue & 0x7FFFL) | 0x8000;
            return lValue & 0xFFFF;
        }

        internal static uint LoUWord(uint lValue)
        {
            if ((lValue & 0x8000L) == 0x8000L)
                return (uint)(lValue & 0x7FFFL) | 0x8000;
            return (uint)(lValue & 0xFFFFL);
        }

        internal static int MakeWord(ushort LoWord, short HiWord)
        {
            return (int)(HiWord * 0x10000 + (long)LoWord);
        }

        internal static uint MakeUWord(ushort LoWord, ushort HiWord)
        {
            return (uint)(HiWord * 0x10000L + LoWord);
        }
    }
}
