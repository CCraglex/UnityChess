
using System;
using System.Collections.Generic;
using UnityEngine;

public static class MoveGenerator
{
    #region  Files/Rows
    const ulong FileA = 0x0101010101010101UL;
    const ulong FileH = 0x8080808080808080UL;
    const ulong Rank2 = 0x000000000000FF00UL;
    const ulong Rank7 = 0x00FF000000000000UL;
    const ulong Rank8 = 0xFF00000000000000UL;
    const ulong Rank1 = 0x00000000000000FFUL;
    const ulong Rank3 = 0x0000000000FF0000UL;
    const ulong Rank6 = 0x0000FF0000000000UL;
    #endregion

    public static ulong GetPseudoMovesOfSqr(Position pos,int sqr)
    {
        if(((pos.w_Pawns | pos.b_Pawns) & (1UL << sqr)) != 0)
            return GetPseudoMovePawnSqr(pos,sqr);
        
        if(((pos.w_Knights | pos.b_Knights) & (1UL << sqr)) != 0)
            return GetPseudoMoveKnightSqr(pos,sqr);

        if(((pos.w_Bishops | pos.b_Bishops) & (1UL << sqr)) != 0)
            return GetPseudoMoveBishopSqr(pos,sqr);
        
        if(((pos.w_Rooks | pos.b_Rooks) & (1UL << sqr)) != 0)
            return GetPseudoMoveRookSqr(pos,sqr);
        
        if(((pos.w_Queens | pos.b_Queens) & (1UL << sqr)) != 0)
            return GetPseudoMoveQueenSqr(pos,sqr);
        
        if(((pos.w_King | pos.b_King) & (1UL << sqr)) != 0)
            return GenerateKingPseudoMoves(pos,sqr);
        
        return 0;
    }

    public static bool IsPlayingColor(Position pos,int sqr)
    {
        ulong mask = pos.playingSide == Turn.White ? pos.WhitePieces : pos.BlackPieces;
        return (mask & (1UL << sqr)) != 0;
    }

    public static ulong GeneratePawnAttacks(Position pos, bool isWhite)
    {

        ulong pawns = isWhite ? pos.w_Pawns : pos.b_Pawns;
        ulong empty = ~pos.AllPieces;
        ulong enemy = isWhite ? pos.BlackPieces : pos.WhitePieces;
        ulong epBB  = pos.enPassantSqr; // bitboard with 1 at en passant target or 0

        ulong attacks = 0;

        if (isWhite)
        {
            // ----- single pushes -----
            ulong one = (pawns << 8) & empty;
            attacks = one & ~Rank8;
            // ----- double pushes -----
            attacks = ((one & Rank3) << 8) & empty;

            // ----- captures -----
            ulong capLeft  = (pawns << 7) & ~FileH;
            ulong capRight = (pawns << 9) & ~FileA;

            attacks = (capLeft & enemy & Rank8) | (capRight & enemy & Rank8);
            attacks = (capLeft & enemy & ~Rank8) | (capRight & enemy & ~Rank8);

            // ----- en passant -----
            attacks = (capLeft | capRight) & epBB;
        }
        else
        {
            // ----- single pushes -----
            ulong one = (pawns >> 8) & empty;

            // ----- double pushes -----
            attacks = ((one & Rank6) >> 8) & empty;

            // ----- captures -----
            ulong capLeft  = (pawns >> 9) & ~FileH;
            ulong capRight = (pawns >> 7) & ~FileA;

            attacks = (capLeft & enemy & Rank1) | (capRight & enemy & Rank1);
            attacks  = (capLeft & enemy & ~Rank1) | (capRight & enemy & ~Rank1);

            // ----- en passant -----
            attacks = (capLeft | capRight) & epBB;
        }

        return attacks;
    }
    public static ulong GetPseudoMovePawnSqr(Position pos,ulong pawnBB)
    {
        bool isWhite = pos.playingSide == Turn.White;

        ulong moves = 0;
        ulong empty = ~pos.AllPieces;
        ulong enemy = isWhite ? pos.BlackPieces : pos.WhitePieces;
        ulong epBB = pos.enPassantSqr;

        if (isWhite)
        {
            // single push
            ulong onePush = (pawnBB << 8) & empty & ~Rank8;
            moves |= onePush;

            // double push
            ulong doublePush = ((pawnBB & Rank2) << 8) & empty;
            doublePush = (doublePush << 8) & empty;
            moves |= doublePush;

            // captures
            ulong capLeft = (pawnBB << 7) & ~FileH;
            ulong capRight = (pawnBB << 9) & ~FileA;
            moves |= (capLeft & enemy) | (capRight & enemy);

            // en passant
            moves |= (capLeft | capRight) & epBB;
        }
        else
        {
            // single push
            ulong onePush = (pawnBB >> 8) & empty & ~Rank1;
            moves |= onePush;

            // double push
            ulong doublePush = ((pawnBB & Rank7) >> 8) & empty;
            doublePush = (doublePush >> 8) & empty;
            moves |= doublePush;

            // captures
            ulong capLeft = (pawnBB >> 9) & ~FileH;
            ulong capRight = (pawnBB >> 7) & ~FileA;
            moves |= (capLeft & enemy) | (capRight & enemy);

            // en passant
            moves |= (capLeft | capRight) & epBB;
        }

        return moves;

    }
    public static ulong GetPseudoMovePawnSqr(Position pos,int PawnSqr)
        => GetPseudoMovePawnSqr(pos,BitOperations.BitFromSqr(PawnSqr));
    //--
    public static ulong GenerateKnightMoves(Position pos, bool isWhite)
    {   
        ulong knights = isWhite ? pos.w_Knights : pos.b_Knights;
        ulong friendlies = isWhite ? pos.WhitePieces : pos.BlackPieces;
        ulong allAttacks = 0UL;

        while (knights != 0)
        {
            allAttacks |= GetPseudoMoveKnightSqr(pos,knights & (~knights + 1));
            knights &= knights - 1;
        }

        return allAttacks;
    }
    public static ulong GetPseudoMoveKnightSqr(Position pos,ulong KnightBB_Pos)
        => GetPseudoMoveKnightSqr(pos,BitOperations.SquareFromBit(KnightBB_Pos));
    public static ulong GetPseudoMoveKnightSqr(Position pos,int KnightSqr)
    {
        var friendlies = pos.playingSide == Turn.White ? pos.WhitePieces : pos.BlackPieces;
        return PieceAttacks.KnightAttacks[KnightSqr] & ~friendlies;
    }
    //--
    static ulong GetPseudoMoveBishopSqr(Position pos,int square)
    {
        ulong attacks = 0UL;
        ulong current;
        ulong bishop = BitOperations.BitFromSqr(square);

        ulong notH = ~FileH;
        ulong notA = ~FileA; 
        ulong allPieces = pos.AllPieces;
        ulong friendlies = pos.playingSide == Turn.White ? pos.WhitePieces : pos.BlackPieces;

        // NE
        current = bishop;
        while ((current & notH) != 0)
        {
            current <<= 9;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // NW
        current = bishop;
        while ((current & notA) != 0)
        {
            current <<= 7;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // SE
        current = bishop;
        while ((current & notH) != 0)
        {
            current >>= 7;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // SW
        current = bishop;
        while ((current & notA) != 0)
        {
            current >>= 9;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        return attacks;
    }    
    //--
    static ulong GenerateAllRookAttacks(Position pos, bool isWhite)
    {

        ulong allPieces = pos.WhitePieces | pos.BlackPieces;
        ulong rooks = isWhite ? pos.w_Rooks : pos.b_Rooks;
        ulong allAttacks = 0UL;

        while (rooks != 0)
        {
            ulong singleRook = rooks & (~rooks + 1); // isolate LSB
            allAttacks |= GetPseudoMoveRookSqr(pos, BitOperations.SquareFromBit(singleRook));
            rooks &= rooks - 1; // remove this rook
        }

        return allAttacks;
    }
    static ulong GetPseudoMoveRookSqr(Position pos, int RookSqr)
    {
        ulong attacks = 0UL;
        ulong current;
        ulong rook = BitOperations.BitFromSqr(RookSqr);

        ulong notA = ~FileA;
        ulong notH = ~FileH;
        ulong allPieces = pos.AllPieces;
        ulong friendlies = pos.playingSide == Turn.White ? pos.WhitePieces : pos.BlackPieces;

        // North
        current = rook;
        while ((current << 8) != 0)
        {
            current <<= 8;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // South
        current = rook;
        while ((current >> 8) != 0)
        {
            current >>= 8;
            if((current & friendlies) == 0)
                attacks |= current;

            if ((current & allPieces) != 0) break;
        }

        // East (right)
        current = rook;
        while ((current & notH) != 0)
        {
            current <<= 1;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // West (left)
        current = rook;
        while ((current & notA) != 0)
        {
            current >>= 1;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        return attacks;
    }
    //--
    static ulong GenerateQueenAttacks(Position pos, bool isWhite)
    {
        ulong allPieces = pos.WhitePieces | pos.BlackPieces;
        ulong queens = isWhite ? pos.w_Queens : pos.b_Queens;
        ulong attacks = 0UL;

        while (queens != 0)
        {
            ulong singleQueen = queens & (~queens + 1);
            attacks |= GetPseudoMoveQueenSqr(pos, BitOperations.SquareFromBit(singleQueen));
            queens &= queens - 1;
        }

        return attacks;
    }
    static ulong GetPseudoMoveQueenSqr(Position pos, int QueenSqr)
    {
        ulong attacks = 0UL;
        ulong current;
        ulong queen = BitOperations.BitFromSqr(QueenSqr);

        ulong notA = ~FileA;
        ulong notH = ~FileH;
        ulong allPieces = pos.AllPieces;
        ulong friendlies = pos.playingSide == Turn.White ? pos.WhitePieces : pos.BlackPieces;

        // NE
        current = queen;
        while ((current & notH) != 0)
        {
            current <<= 9;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // NW
        current = queen;
        while ((current & notA) != 0)
        {
            current <<= 7;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // SE
        current = queen;
        while ((current & notH) != 0)
        {
            current >>= 7;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // SW
        current = queen;
        while ((current & notA) != 0)
        {
            current >>= 9;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // North
        current = queen;
        while ((current << 8) != 0)
        {
            current <<= 8;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // South
        current = queen;
        while ((current >> 8) != 0)
        {
            current >>= 8;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // East
        current = queen;
        while ((current & notH) != 0)
        {
            current <<= 1;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        // West
        current = queen;
        while ((current & notA) != 0)
        {
            current >>= 1;
            if((current & friendlies) == 0)
                attacks |= current;
            if ((current & allPieces) != 0) break;
        }

        return attacks;
    }
    //--
    static ulong GenerateKingPseudoMoves(Position pos, int KingSqr)
    {
        bool isWhite = pos.playingSide == Turn.White;
        
        ulong king = isWhite ? pos.w_King : pos.b_King;
        ulong allPieces = pos.AllPieces;
        ulong ownPieces = isWhite ? pos.WhitePieces : pos.BlackPieces;

        ulong attacks = PieceAttacks.KingAttacks[BitOperations.SquareFromBit(king)];
        attacks &= ~ownPieces;

        byte castleRights = pos.castleRights;

        if (isWhite)
        {
            // White king-side castling
            if ((castleRights & 0b1000) != 0 && (allPieces & ((1UL << 5) | (1UL << 6))) == 0)
                attacks |= 1UL << 6; // G1

            // White queen-side castling
            if ((castleRights & 0b0100) != 0 && (allPieces & ((1UL << 1) | (1UL << 2) | (1UL << 3))) == 0)
                attacks |= 1UL << 2; // C1
        }
        else
        {
            // Black king-side castling
            if ((castleRights & 0b0010) != 0 && (allPieces & ((1UL << 61) | (1UL << 62))) == 0)
                attacks |= 1UL << 62; // G8

            // Black queen-side castling
            if ((castleRights & 0b0001) != 0 && (allPieces & ((1UL << 57) | (1UL << 58) | (1UL << 59))) == 0)
                attacks |= 1UL << 58; // C8
        }

        return attacks;
    }

    public static Span<Move> GetLegitMoves()
        => null;
}