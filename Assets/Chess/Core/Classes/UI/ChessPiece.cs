
using UnityEngine;
using UnityEngine.EventSystems;

public class ChessPiece : DraggableObject, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    [SerializeField] private int pieceSquare;

    //+60 allows for placing on edges correectly with slight extra
    private const float mapBorder = 350 + 60;
    private Vector2 lastLocalPosition;

    public override void Init()
        => MoveToSqr(pieceSquare);

    public void MoveToSqr(int sqr)
    {
        int File = sqr % 8;
        int Row = sqr / 8;

        rect.localPosition = SnapPosition(new(File * 100 - 350,Row * 100 - 350));  
        lastLocalPosition = rect.localPosition;
        pieceSquare = sqr;
    }

    private void SetPieceSqr()
    {
        int file = Mathf.RoundToInt((rect.localPosition.x + 350f) / 100f);
        int row  = Mathf.RoundToInt((rect.localPosition.y + 350f) / 100f);

        pieceSquare = row * 8 + file;
        print("?");
    }

    private bool IsAllowedToMove()
    {
        return true;
    }

    private Vector2 SnapPosition(Vector2 newPos)
    {
            float snappedX = Mathf.Round((newPos.x - 50f) / 100f) * 100f + 50f;
            snappedX = Mathf.Clamp(snappedX, -350f, 350f);
            
            float snappedY = Mathf.Round((newPos.y - 50f) / 100f) * 100f + 50f;
            snappedY = Mathf.Clamp(snappedY, -350f, 350f);

        return new(snappedX,snappedY);
    }

    public override void OnEndDragExtras()
    {
        if(mapBorder < Mathf.Abs(rect.localPosition.x) || mapBorder < Mathf.Abs(rect.localPosition.y) || !IsAllowedToMove())
            return;
    
        var pos = SnapPosition(rect.localPosition);
        lastLocalPosition  = pos;
        rect.localPosition = pos;
        rect.localPosition = lastLocalPosition;
        SetPieceSqr();
    }

    public override void OnSelectExtras()
    {
        var board = Board.Instance;
        ulong sqr = 1UL << pieceSquare;
        print(sqr);

        ulong possibleMoves = MoveGenerator.ulongGetPseudoMovePawnSqr(board.currentPosition,false,sqr);
        print(possibleMoves);
    }
}