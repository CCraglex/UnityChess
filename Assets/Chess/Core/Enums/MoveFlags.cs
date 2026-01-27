using System;

[Flags]
public enum MoveFlags : byte
{
    Quiet        = 0,
    Capture      = 1 << 0,

    PromoteQueen = 1 << 1,
    PromoteRook  = 1 << 2,
    PromoteBishop= 1 << 3,
    PromoteKnight= 1 << 4,
    EnPassant    = 1 << 5,
    Castling     = 1 << 6,
}
