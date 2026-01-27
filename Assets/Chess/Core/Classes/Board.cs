using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    public Turn currentTurn;
    public Position currentPosition;

    public GameObject[] piecePrefabs;
    public GameObject[] Pieces;

    public Transform piecesParent;

    private void Start()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
            
        Instance = this;
        CreateBoard();
    }

    public void CreateBoard()
    {
        currentPosition.CustomBoard("rnbqkbnr/ppp1pppp/8/8/3pP3/6PP/PPPP1P2/RNBQKBNR b KQkq e3 0 3");
        //currentPosition.NewBoard();
        Pieces = new GameObject[32];
        PlacePieces();

        print("?");
    }
    public void PlacePieces()
    {
        SpawnFromBitboard(currentPosition.w_Pawns,   piecePrefabs[0]);
        SpawnFromBitboard(currentPosition.w_Knights, piecePrefabs[1]);
        SpawnFromBitboard(currentPosition.w_Bishops, piecePrefabs[2]);
        SpawnFromBitboard(currentPosition.w_Rooks,   piecePrefabs[3]);
        SpawnFromBitboard(currentPosition.w_Queens,  piecePrefabs[4]);
        SpawnFromBitboard(currentPosition.w_King,    piecePrefabs[5]);

        SpawnFromBitboard(currentPosition.b_Pawns,   piecePrefabs[6]);
        SpawnFromBitboard(currentPosition.b_Knights, piecePrefabs[7]);
        SpawnFromBitboard(currentPosition.b_Bishops, piecePrefabs[8]);
        SpawnFromBitboard(currentPosition.b_Rooks,   piecePrefabs[9]);
        SpawnFromBitboard(currentPosition.b_Queens,  piecePrefabs[10]);
        SpawnFromBitboard(currentPosition.b_King,    piecePrefabs[11]);
    }

    public void SpawnFromBitboard(ulong bitboard,GameObject prefab)
    {
        int index = 0;
        for (int sqr = 0; sqr < 64; sqr++)
        {
            if ((bitboard & (1UL << sqr)) != 0)
            {
                bitboard &= bitboard - 1; // clear lowest bit

                
                var p = Instantiate(prefab,piecesParent);
                Pieces[index] = p;
                PlacePiece(p,sqr);
                index++;
            }
        }
    }
    public void PlacePiece(GameObject Piece,int Sqr)
    {
        Piece.GetComponent<ChessPiece>().MoveToSqr(Sqr);
    }

    public void RemovePiece(GameObject Piece)
    {
        
    }

    public void ClearBoard()
    {
        for (int i = 0; i < Pieces.Length; i++)
        {
            if(Pieces[i] != null)
                RemovePiece(Pieces[i]);
        }
        
        Pieces = new GameObject[32];
    }
}
