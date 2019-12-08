using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TurnPhase
{
    idle,
    pre,
    waiting,
    post,
    gameOver
}

public class LostCities : MonoBehaviour
{
    static public LostCities S;
    static public Player CURRENT_PLAYER;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;
    public float handFanDegrees = 10f;
    public int numStartingCards = 8;
    public float drawTimeStagger = 0.1f;

    [Header("Set Dynamically")]
    public Deck deck;
    public List<CardLostCities> drawPile;
    //public List<CardLostCities> discardPile;

    public List<CardLostCities> redDiscardPile;
    public List<CardLostCities> greenDiscardPile;
    public List<CardLostCities> whiteDiscardPile;
    public List<CardLostCities> blueDiscardPile;
    public List<CardLostCities> yellowDiscardPile;

    public List<CardLostCities> redPlayer1;
    public List<CardLostCities> redPlayer2;
    public List<CardLostCities> greenPlayer1;
    public List<CardLostCities> greenPlayer2;
    public List<CardLostCities> whitePlayer1;
    public List<CardLostCities> whitePlayer2;
    public List<CardLostCities> bluePlayer1;
    public List<CardLostCities> bluePlayer2;
    public List<CardLostCities> yellowPlayer1;
    public List<CardLostCities> yellowPlayer2;

    public List<Player> players;
    public CardLostCities targetCard;
    public CardLostCities targetKeepCard;

    public TurnPhase phase = TurnPhase.idle;
    public int firstCard = 0;

    public bool popUp = false;
    public bool playerTurn = true;
    public bool played = false;

    private BartokLayout layout;
    private Transform layoutAnchor;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {


        deck = GetComponent<Deck>(); // Get the Deck
        deck.InitDeck(deckXML.text); // Pass DeckXML to it
        Deck.Shuffle(ref deck.cards); // This shuffles the deck

        layout = GetComponent<BartokLayout>(); // Get the Layout
        layout.ReadLayout(layoutXML.text); // Pass LayoutXML to it

        drawPile = UpgradeCardsList(deck.cards);
        LayoutGame();
    }

    private void Update()
    {
        if(!playerTurn)
        {
            playAI();
            players[1].AddCard(Draw());
        }
    }

    List<CardLostCities> UpgradeCardsList(List<Card> lCD)
    {
        List<CardLostCities> lCB = new List<CardLostCities>();
        foreach (Card tCD in lCD)
        {
            lCB.Add(tCD as CardLostCities);
        }
        return (lCB);
    }

    // Position all the cards in the drawPile properly
    public void ArrangeDrawPile()
    {
        CardLostCities tCB;

        for (int i = 0; i < drawPile.Count; i++)
        {
            tCB = drawPile[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.drawPile.pos;
            // Rotation should start at 0
            Vector2 dpStagger = layout.drawPile.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
                layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
                -layout.drawPile.layerID + 0.1f * i);
            tCB.faceUp = false;
            tCB.SetSortingLayerName(layout.drawPile.layerName);
            tCB.SetSortOrder(-i * 4); // Order them front-to-back
            tCB.state = CBState.drawpile;
        }
    }

    public void ArrangePlayerPile()
    {
        Vector3 pos;
        CardLostCities tCB;
        for (int i = 0; i < redPlayer1.Count; i++)
        {
            tCB = redPlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.redPlayer1.pos;
            
            Vector2 dpStagger = layout.redPlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.redPlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.redPlayer1.y + i * dpStagger.y),
                -layout.redPlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.redPlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.target;
        }
        for (int i = 0; i < greenPlayer1.Count; i++)
        {
            tCB = greenPlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.greenPlayer1.pos;

            Vector2 dpStagger = layout.greenPlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.greenPlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.greenPlayer1.y + i * dpStagger.y),
                -layout.greenPlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.greenPlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.player1;
        }
        for (int i = 0; i < whitePlayer1.Count; i++)
        {
            tCB = whitePlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.whitePlayer1.pos;

            Vector2 dpStagger = layout.whitePlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.whitePlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.whitePlayer1.y + i * dpStagger.y),
                -layout.whitePlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.whitePlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.player1;
        }
        for (int i = 0; i < bluePlayer1.Count; i++)
        {
            tCB = bluePlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.bluePlayer1.pos;

            Vector2 dpStagger = layout.bluePlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.bluePlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.bluePlayer1.y + i * dpStagger.y),
                -layout.bluePlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.bluePlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.player1;
        }
        for (int i = 0; i < yellowPlayer1.Count; i++)
        {
            tCB = yellowPlayer1[i];
            tCB.transform.SetParent(layoutAnchor);
            //tCB.transform.localPosition = layout.yellowPlayer1.pos;

            Vector2 dpStagger = layout.yellowPlayer1.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.yellowPlayer1.x + i * dpStagger.x),
                layout.multiplier.y * (layout.yellowPlayer1.y + i * dpStagger.y),
                -layout.yellowPlayer1.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.yellowPlayer1.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            tCB.state = CBState.player1;
        }
    }

    public void ArrangePlayer2Pile()
    {
        CardLostCities tCB;
        for (int i = 0; i < redPlayer2.Count; i++)
        {
            
            tCB = redPlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.redPlayer2.pos;
            
            Vector2 dpStagger = layout.redPlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.redPlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.redPlayer2.y + i * dpStagger.y),
                -layout.redPlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.redPlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if(tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.player2;
        }
        for (int i = 0; i < greenPlayer2.Count; i++)
        {
            tCB = greenPlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.greenPlayer2.pos;

            Vector2 dpStagger = layout.greenPlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.greenPlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.greenPlayer2.y + i * dpStagger.y),
                -layout.greenPlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.greenPlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.player2;
        }
        for (int i = 0; i < whitePlayer2.Count; i++)
        {
            tCB = whitePlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.whitePlayer2.pos;

            Vector2 dpStagger = layout.whitePlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.whitePlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.whitePlayer2.y + i * dpStagger.y),
                -layout.whitePlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.whitePlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.player2;
        }
        for (int i = 0; i < bluePlayer2.Count; i++)
        {
            tCB = bluePlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.bluePlayer2.pos;

            Vector2 dpStagger = layout.bluePlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.bluePlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.bluePlayer2.y + i * dpStagger.y),
                -layout.bluePlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.bluePlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.player2;
        }
        for (int i = 0; i < yellowPlayer2.Count; i++)
        {
            tCB = yellowPlayer2[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.yellowPlayer2.pos;

            Vector2 dpStagger = layout.yellowPlayer2.stagger;
            tCB.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.yellowPlayer2.x + i * dpStagger.x),
                layout.multiplier.y * (layout.yellowPlayer2.y + i * dpStagger.y),
                -layout.yellowPlayer2.layerID + 0.1f * i);
            tCB.faceUp = true;
            tCB.SetSortingLayerName(layout.yellowPlayer2.layerName);
            tCB.SetSortOrder(i * 4); // Order them front-to-back
            if (tCB.transform.rotation == Quaternion.Euler(Vector3.zero))
            {
                tCB.transform.Rotate(180, 0, 0);
            }
            tCB.state = CBState.target;
        }
    }

    //Perform the initial game layout
    void LayoutGame()
    {
        // Create an empty GameObject to serve as the tableau's anchor
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        // Position the drawPile cards
        ArrangeDrawPile();

        // Set up the players
        Player pl;
        players = new List<Player>();
        foreach (SlotDef tSD in layout.slotDefs)
        {
            pl = new Player();
            pl.handSlotDef = tSD;
            players.Add(pl);
            pl.playerNum = tSD.player;
        }
        players[0].type = PlayerType.human; // Make only the 0th player human
        Debug.Log(players[1].type);
        CardLostCities tCB;
        // Deal # cards to each player
        for (int i = 0; i < numStartingCards; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                
                tCB = Draw(); // Draw a card
                // Stagger the draw time a bit.
                tCB.timeStart = Time.time + drawTimeStagger * (i * 4 + j);
                
                players[j].AddCard(tCB);
            }
        }

        //Invoke("DrawFirstTarget", drawTimeStagger * (numStartingCards * 4 + 4));
    }

    

    // This callback is used by the last card to be dealt at the beginning
    public void CBCallback(CardLostCities cb)
    {
        // You sometimes want to have reporting of method calls like this
        Utils.tr("LostCities:CBCallback()", cb.name);
        StartGame(); // Start the Game
    }

    public void StartGame()
    {
        // Pick the player to the left of the human to go first.
        //PassTurn(1);
    }

    public void PassTurn(int num=-1)
    {
        Utils.tr("LostCities:PassTurn()", "Current Player: " + players.IndexOf(CURRENT_PLAYER));
        int lastPlayerNum = 0;
        int ndx = players.IndexOf(CURRENT_PLAYER);
        lastPlayerNum = players.IndexOf(CURRENT_PLAYER);
        if (ndx == 0)
            num = 1;
        else if (ndx == 1)
            num = 0;
        if (CURRENT_PLAYER != null)
        {
            lastPlayerNum = CURRENT_PLAYER.playerNum;
            // Check for Game Over and need to reshuffle discards
            /*
            if (CheckGameOver())
            {
                return;
            }*/
        }
        CURRENT_PLAYER = players[num];
        phase = TurnPhase.pre;

        CURRENT_PLAYER.TakeTurn();

        // Report the turn passing
        Utils.tr("LostCities:PassTurn()", "Old: " + lastPlayerNum, "New: " + CURRENT_PLAYER.playerNum);
    }

    
    public bool CheckGameOver()
    {
        // See if we need to reshuffle the discard pile into the draw pile
        if (drawPile.Count == 0)
        {
            // The player that just played has won!
            phase = TurnPhase.gameOver;
            Invoke("RestartGame", 1);
            return (true);
        }

        
        return (false);
    }
    

    public void RestartGame()
    {
        CURRENT_PLAYER = null;
        SceneManager.LoadScene("__LostCities_Scene_0");
    }
    

    // ValidPlay verifies that the card chosen can be played on the discard pile
    public bool ValidPlay(CardLostCities cb)
    {

        // It's a valid play if the suit is the same
        // Check is its a higher rank
        //if (cb.suit == targetCard.suit)
        //{
        //    if (cb.rank > targetCard.rank) return (true);
        //}
        if (playerTurn == true)
        {
            if (WhichPlayerDiscard(cb).Count == 0)
                return true;
            else if (cb.rank > WhichPlayerDiscard(cb)[WhichPlayerDiscard(cb).Count - 1].rank) return true;
            else if (cb.rank == 1 && WhichPlayerDiscard(cb)[WhichPlayerDiscard(cb).Count - 1].rank == 1) return true;
        }

        if (playerTurn == false)
        {
            if (WhichAIDiscard(cb).Count == 0)
                return true;
            else if (cb.rank > WhichAIDiscard(cb)[WhichAIDiscard(cb).Count - 1].rank) return true;
            else if (cb.rank == 1 && WhichAIDiscard(cb)[WhichAIDiscard(cb).Count - 1].rank == 1) return true;
        }

        // Otherwise, return false
        return (false);
    }

    // This makes a new card the target
    public CardLostCities MoveToTarget(CardLostCities tCB)
    {
        tCB.timeStart = 0;


        tCB.state = CBState.toTarget;
        tCB.faceUp = true;

        tCB.SetSortingLayerName("10");
        tCB.eventualSortLayer = layout.target.layerName;
        if (targetCard != null)
        {
            MoveToDiscard(targetCard);
        }

        targetCard = tCB;

        return tCB;
    }

    public void ChangeTarget(CardLostCities tCB)
    {
        targetKeepCard = tCB;
        Debug.Log(tCB.state);
    }

    public CardLostCities MoveToDiscard(CardLostCities tCB)
    {
        WhichDiscard(tCB).Add(tCB);
        Vector3 pos;
        tCB.timeStart = 0;
        if (tCB.suit.Equals("R"))
        {
            
            tCB.SetSortingLayerName(layout.redDiscardPile.layerName);
            tCB.SetSortOrder(redDiscardPile.Count * 4);
            //tCB.transform.localPosition = layout.greenDiscardPile.pos + Vector3.back / 2;
            //tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.redDiscardPile.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toRedDiscard;
        }
        if (tCB.suit.Equals("G"))
        {
            tCB.SetSortingLayerName(layout.greenDiscardPile.layerName);
            tCB.SetSortOrder(greenDiscardPile.Count * 4);

            pos = layout.greenDiscardPile.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toGreenDiscard;
        }
        if (tCB.suit.Equals("W"))
        {
            tCB.SetSortingLayerName(layout.whiteDiscardPile.layerName);
            tCB.SetSortOrder(whiteDiscardPile.Count * 4);

            pos = layout.whiteDiscardPile.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toWhiteDiscard;
        }
        if (tCB.suit.Equals("B"))
        {
            tCB.SetSortingLayerName(layout.blueDiscardPile.layerName);
            tCB.SetSortOrder(blueDiscardPile.Count * 4);

            pos = layout.blueDiscardPile.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toBlueDiscard;
        }
        if (tCB.suit.Equals("Y"))
        {
            tCB.SetSortingLayerName(layout.yellowDiscardPile.layerName);
            tCB.SetSortOrder(yellowDiscardPile.Count * 4);

            pos = layout.yellowDiscardPile.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toYellowDiscard;
        }

        players[0].RemoveCard(tCB);
        return tCB;
    }

    public CardLostCities MoveToPlayerDiscard(CardLostCities tCB)
    {
        WhichPlayerDiscard(tCB).Add(tCB);
        Vector3 pos;
        tCB.timeStart = 0;
        if (tCB.suit.Equals("R"))
        {
            tCB.SetSortingLayerName(layout.redPlayer1.layerName);
            tCB.SetSortOrder(redPlayer1.Count * 4);
            //tCB.transform.localPosition = layout.redPlayer1.pos + Vector3.back / 2;
            //tCB.transform.localRotation = Quaternion.Euler(Vector3.zero);

            pos = layout.redPlayer1.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toRedPlayer1;

            ArrangePlayerPile();
        }
        if (tCB.suit.Equals("G"))
        {
            tCB.SetSortingLayerName(layout.greenPlayer1.layerName);
            tCB.SetSortOrder(greenPlayer1.Count * 4);

            pos = layout.greenPlayer1.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toGreenPlayer1;

            ArrangePlayerPile();
        }
        if (tCB.suit.Equals("W"))
        {
            tCB.SetSortingLayerName(layout.whitePlayer1.layerName);
            tCB.SetSortOrder(whitePlayer1.Count * 4);
            pos = layout.whitePlayer1.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toWhitePlayer1;
            ArrangePlayerPile();
        }
        if (tCB.suit.Equals("B"))
        {
            tCB.SetSortingLayerName(layout.bluePlayer1.layerName);
            tCB.SetSortOrder(bluePlayer1.Count * 4);

            pos = layout.bluePlayer1.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toBluePlayer1;
            ArrangePlayerPile();
        }
        if (tCB.suit.Equals("Y"))
        {
            tCB.SetSortingLayerName(layout.yellowPlayer1.layerName);
            tCB.SetSortOrder(yellowPlayer1.Count * 4);

            pos = layout.yellowPlayer1.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toYellowPlayer1;
            ArrangePlayerPile();
        }

        

        players[0].RemoveCard(tCB);

        return tCB;
    }

    public CardLostCities MoveToAIDiscard(CardLostCities tCB)
    {
        WhichAIDiscard(tCB).Add(tCB);
        Vector3 pos;
        tCB.timeStart = 0;
        if (tCB.suit.Equals("R"))
        {
            tCB.SetSortingLayerName(layout.redPlayer2.layerName);
            tCB.SetSortOrder(redPlayer2.Count * 4);
            pos = layout.redPlayer2.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toRedPlayer2;
            ArrangePlayer2Pile();
        }
        if (tCB.suit.Equals("G"))
        {
            tCB.SetSortingLayerName(layout.greenPlayer2.layerName);
            tCB.SetSortOrder(greenPlayer2.Count * 4);
            pos = layout.greenPlayer2.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toGreenPlayer2;
            ArrangePlayer2Pile();
        }
        if (tCB.suit.Equals("W"))
        {
            tCB.SetSortingLayerName(layout.whitePlayer2.layerName);
            tCB.SetSortOrder(whitePlayer2.Count * 4);
            pos = layout.whitePlayer2.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toWhitePlayer2;
            ArrangePlayer2Pile();
        }
        if (tCB.suit.Equals("B"))
        {
            tCB.SetSortingLayerName(layout.bluePlayer2.layerName);
            tCB.SetSortOrder(bluePlayer2.Count * 4);
            pos = layout.bluePlayer2.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toBluePlayer2; ;
            ArrangePlayer2Pile();
        }
        if (tCB.suit.Equals("Y"))
        {
            tCB.SetSortingLayerName(layout.yellowPlayer2.layerName);
            tCB.SetSortOrder(yellowPlayer2.Count * 4);
            pos = layout.yellowPlayer2.pos;
            tCB.MoveTo(pos);

            tCB.state = CBState.toYellowPlayer2;
            ArrangePlayer2Pile();
        }

        

        players[1].RemoveCard(tCB);

        return tCB;
    }

    public List<CardLostCities> WhichDiscard(CardLostCities tCB)
    {
        Debug.Log(tCB.suit);
        if (tCB.suit.Equals("R"))
        {
            return redDiscardPile;
        }
        else if (tCB.suit.Equals("G"))
        {
            return greenDiscardPile;
        }
        else if (tCB.suit.Equals("W"))
        {
            return whiteDiscardPile;
        }
        else if (tCB.suit.Equals("B"))
        {
            return blueDiscardPile;
        }
        else
        {
            return yellowDiscardPile;
        }
    }

    public List<CardLostCities> WhichPlayerDiscard(CardLostCities tCB)
    {
        Debug.Log(tCB.suit);

        
        {
            if (tCB.suit.Equals("R"))
            {
                return redPlayer1;
            }
            else if (tCB.suit.Equals("G"))
            {
                return greenPlayer1;
            }
            else if (tCB.suit.Equals("W"))
            {
                return whitePlayer1;
            }
            else if (tCB.suit.Equals("B"))
            {
                return bluePlayer1;
            }
            else
            {
                return yellowPlayer1;
            }
        }
        
        
    }
    public List<CardLostCities> WhichAIDiscard(CardLostCities tCB)
    {
        if (tCB.suit.Equals("R"))
        {
            return redPlayer2;
        }
        else if (tCB.suit.Equals("G"))
        {
            return greenPlayer2;
        }
        else if (tCB.suit.Equals("W"))
        {
            return whitePlayer2;
        }
        else if (tCB.suit.Equals("B"))
        {
            return bluePlayer2;
        }
        else
        {
            return yellowPlayer2;
        }
    }
    void OnGUI()
    {

        if (popUp && !played && playerTurn)
        {
            if (GUI.Button(new Rect(10, 200, 100, 28), "Discard"))
            {

                if (playerTurn && !played && targetKeepCard.state == CBState.hand)
                {
                    MoveToDiscard(targetKeepCard);
                    played = true;
                    popUp = false;
                    //passturn();


                }
            }

            if (GUI.Button(new Rect(10, 300, 100, 28), "Play"))
            {

                if (ValidPlay(targetKeepCard) && playerTurn && !played && targetKeepCard.state == CBState.hand)
                {
                    //MoveToPlayerDiscard(targetKeepCard);
                    MoveToPlayerDiscard(targetKeepCard);
                    popUp = false;
                    played = true;
                    
                    
                }
                
                //phase = TurnPhase.waiting;
            }

        }

    }

    public void playAI()
    {
        Utils.tr("AI Turn");
        players[1].TakeTurn();
        playerTurn = true;
        //if (ValidPlay(players[1].TakeTurn(cd))
        //{
            //MoveToPlayerDiscard(targetKeepCard);
        //    MoveToAIDiscard(targetKeepCard);
        //}
    }

    // The Draw function will pull a single card from the drawPile and return it
    public CardLostCities Draw()
    {

        CardLostCities cd = drawPile[0]; // Pull the 0th CardLostCities

        //if (drawPile.Count == 0)
        //{
        // If the drawPile is now empty
        // We need to shuffle the discards into the drawPile
        //int ndx;
        //while (discardPile.Count > 0)
        //{
        //    // Pull a random card from the discard pile
        //    ndx = Random.Range(0, discardPile.Count);
        //    drawPile.Add(discardPile[ndx]);
        //    discardPile.RemoveAt(ndx);
        //}
        //ArrangeDrawPile();
        // Show the cards moving to the drawPile
        //float t = Time.time;
        //foreach (CardLostCities tCB in drawPile)
        //{
        //    tCB.transform.localPosition = layout.discardPile.pos;
        //    tCB.callbackPlayer = null;
        //    tCB.MoveTo(layout.drawPile.pos);
        //    tCB.timeStart = t;
        //    t += 0.02f;
        //    tCB.state = CBState.toDrawpile;
        //    tCB.eventualSortLayer = "0";
        //}
        //}

        
        drawPile.RemoveAt(0); // Then remove it from List<> drawPile
        CheckGameOver();
        return (cd); // And return it

    }
    
    public CardLostCities RedDraw()
    {
        Debug.Log(redDiscardPile.Count - 1);
        int i = redDiscardPile.Count - 1;
        CardLostCities cd = redDiscardPile[i];
        redDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }
    public CardLostCities GreenDraw()
    {
        int i = greenDiscardPile.Count - 1;
        CardLostCities cd = greenDiscardPile[i];
        greenDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }
    public CardLostCities WhiteDraw()
    {
        int i = whiteDiscardPile.Count - 1;
        CardLostCities cd = whiteDiscardPile[i];
        whiteDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }
    public CardLostCities BlueDraw()
    {
        int i = blueDiscardPile.Count - 1;
        CardLostCities cd = blueDiscardPile[i];
        blueDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }
    public CardLostCities YellowDraw()
    {
        int i = yellowDiscardPile.Count - 1;
        CardLostCities cd = yellowDiscardPile[i];
        yellowDiscardPile.RemoveAt(i);
        playerTurn = !playerTurn;
        return (cd);
    }

    public void CardClicked(CardLostCities tCB)
    {
        //if (CURRENT_PLAYER.type != PlayerType.human) return;
        //if (phase == TurnPhase.waiting) return;

        ChangeTarget(tCB);

        switch (tCB.state)
        {
            case CBState.drawpile:
                // Draw the top card, not necessarily the one clicked.
                //CardLostCities cb = CURRENT_PLAYER.AddCard(Draw());
                //cb.callbackPlayer = CURRENT_PLAYER;
                //Utils.tr("LostCities:CardClicked()", "Draw", cb.name);
                //phase = TurnPhase.waiting;

                if (players[0].hand.Count < 8)
                {
                    players[0].AddCard(Draw());
                    ArrangeDrawPile();
                    playerTurn = !playerTurn;
                    played = false;
                    
                }
                break;

            case CBState.hand:
                // Check to see whether the card is valid
                if (!played)
                {
                    popUp = true;
                }
                
                //if (ValidPlay(tCB))
                //{
                //    MoveToPlayerDiscard(tCB);
                //    CURRENT_PLAYER.RemoveCard(tCB);
                //    MoveToTarget(tCB);
                //    tCB.callbackPlayer = CURRENT_PLAYER;
                //    Utils.tr("LostCities:CardClicked()", "Play", tCB.name, targetCard.name + " is target");
                //    phase = TurnPhase.waiting;
                //}

                {
                    // Just ignore it but report what the player tried
                    //    Utils.tr("LostCities:CardClicked()", "Attempted to Play", tCB.name, targetCard.name + " is target");

                }
                break;

            case CBState.redDiscard:
                if (players[0].hand.Count < 8)
                {
                    players[0].AddCard(RedDraw());
                }
                break;
            case CBState.greenDiscard:
                if (players[0].hand.Count < 8)
                {
                    players[0].AddCard(GreenDraw());
                }
                break;
            case CBState.whiteDiscard:
                if (players[0].hand.Count < 8)
                {
                    players[0].AddCard(WhiteDraw());
                }
                break;
            case CBState.blueDiscard:
                if (players[0].hand.Count < 8)
                {
                    players[0].AddCard(BlueDraw());
                }
                break;
            case CBState.yellowDiscard:
                if (players[0].hand.Count < 8)
                {
                    players[0].AddCard(YellowDraw());
                }
                break;
        }

    }
}
