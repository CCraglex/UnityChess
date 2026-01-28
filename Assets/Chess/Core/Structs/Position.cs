
public struct Position
{
    public ulong w_Pawns,w_Knights,w_Bishops,w_Rooks,w_Queens,w_King;
    public ulong b_Pawns,b_Knights,b_Bishops,b_Rooks,b_Queens,b_King;
    public ulong enPassantSqr;

    public ulong WhitePieces => w_Pawns | w_Knights | w_Bishops | w_Rooks | w_Queens | w_King;
    public ulong BlackPieces => b_Pawns | b_Knights | b_Bishops | b_Rooks | b_Queens | b_King;
    public ulong AllPieces => WhitePieces | BlackPieces;

    public Turn playingSide;
    public byte castleRights;

    public void NewBoard()
        => CustomBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

    public void CustomBoard(string fen)
    {
        string[] parts = fen.Split(' ');
        string board = parts[0];
        string side = parts[1];
        string castling = parts[2];
        string enPassant = parts[3];

        int sqr = 56; // start at a8

        foreach (char c in board)
        {
            if (c == '/')
            {
                sqr -= 16; // move to next rank
                continue;
            }

            if (char.IsDigit(c))
            {
                sqr += c - '0';
                continue;
            }

            ulong bit = 1UL << sqr;

            switch (c)
            {
                case 'P': w_Pawns   |= bit; break;
                case 'N': w_Knights |= bit; break;
                case 'B': w_Bishops |= bit; break;
                case 'R': w_Rooks   |= bit; break;
                case 'Q': w_Queens  |= bit; break;
                case 'K': w_King    |= bit; break;

                case 'p': b_Pawns   |= bit; break;
                case 'n': b_Knights |= bit; break;
                case 'b': b_Bishops |= bit; break;
                case 'r': b_Rooks   |= bit; break;
                case 'q': b_Queens  |= bit; break;
                case 'k': b_King    |= bit; break;
            }

            sqr++;
        }

        // Side to move
        playingSide = side == "w" ? Turn.White : Turn.Black;

        // Castling rights
        castleRights = 0;
        if (castling.Contains('K')) castleRights |= 1 << 0;
        if (castling.Contains('Q')) castleRights |= 1 << 1;
        if (castling.Contains('k')) castleRights |= 1 << 2;
        if (castling.Contains('q')) castleRights |= 1 << 3;

        // En passant
        enPassantSqr = 0UL;
        if (enPassant != "-")
        {
            int file = enPassant[0] - 'a';
            int rank = enPassant[1] - '1';
            int epSqr = rank * 8 + file;
            enPassantSqr = 1UL << epSqr;
        }       
    }
}