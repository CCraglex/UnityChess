public static class BitOperations
{
    static readonly int[] DeBruijnTable = new int[64]
    {
        0,  1, 48,  2, 57, 49, 28,  3,
        61, 58, 50, 42, 38, 29, 17,  4,
        62, 55, 59, 36, 53, 51, 43, 22,
        45, 39, 33, 30, 24, 18, 12,  5,
        63, 47, 56, 27, 60, 41, 37, 16,
        54, 35, 52, 21, 44, 32, 23, 11,
        46, 26, 40, 15, 34, 20, 31, 10,
        25, 14, 19,  9, 13,  8,  7,  6
    };

    public static int SquareFromBit(ulong squareBit)
    {
        const ulong DeBruijn64 = 0x03F79D71B4CB0A89UL;
        return DeBruijnTable[((squareBit & (~squareBit + 1)) * DeBruijn64) >> 58];
    }

    public static int[] SqrArrFromBit(ulong bitboard)
    {
        // count bits first to allocate array
        int count = PopCount(bitboard); // number of squares
        int[] squares = new int[count];
        int i = 0;

        while (bitboard != 0)
        {
            ulong lsb = bitboard & (~bitboard + 1);
            squares[i++] = SquareFromBit(lsb);
            bitboard &= bitboard - 1;
        }

        return squares;
    }
    
    public static ulong BitFromSqr(int sqr)
        => 1UL << sqr;
    

    public static int PopCount(ulong x)
    {
        int count = 0;
        while (x != 0)
        {
            x &= x - 1; // clear LSB
            count++;
        }
        return count;
    }
}