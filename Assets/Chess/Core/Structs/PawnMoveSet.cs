public struct PawnMoveSet
{
    public ulong SinglePush;   // one square pushes
    public ulong DoublePush;   // two square pushes
    public ulong Captures;     // normal captures (no promos)
    public ulong PromoPushes;  // single pushes into promotion rank
    public ulong PromoCaps;    // captures into promotion rank
    public ulong EnPassant;    // en passant targets
}
