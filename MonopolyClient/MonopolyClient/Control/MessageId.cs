using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public enum MessageId // Make the enum public
    {
        Connect,
        RegisterPlayer,
        RegisterSuccess,
        RegisterFailure,
        PlayerJoined,
        ReadyIsPossible,
        ReadyToStartGame,
        ReadySuccess,
        ReadyFailure,
        PlayerIsReady,
        EnableRegisterInId,
        DisableReady,
        GameStarted,
        GameHadAlreadyStarted,
        WhoseTurn,
        RollDiceAvailable,
        RollDiceClicked,
        RollDiceSuccess,
        RollDiceFailure,
        SendDicesValue,
        RollDiceEnded,
        MovePlayer,
        PlayerNeedsToGoToSquare,
        PlayerMovingEnded,
        UpdateMessageLabelForEveryone,
        OkClicked,
        BuySquareOffer,
        Buy,
        DontBuy,
        UpdateSquareWithOwner,
        BuyHouseOffer,
        HouseAdded,
        UpdateMoneyLabel,
        AmountOfOutOfJailCards,
        UseOutOfJailCardOffer,
        UseCard,
        DontUseCard,
        RemoveSquareFromOwner,
        SendMessage,
        UpdateMessageInChat,
        AskedMoneyOfferedSquares,
        AskedMoneySquaresOfferedSquares,
        AskedSquaresOfferedMoney,
        AskedSquaresOfferedSquaresMoney,
        AskedSquaresOfferedSquares,
        TradingOffer,
        Sell,
        DontSell,
        EnableSendingOffersToPlayer,
        PlayerIsNotAbleToReceiveOffer,
        TradeWasMade,
        UpdatePrivateMessageLabel,
        RemovePlayerDisconnectedFromClients,
        MakePlayerSpectateInClients
    }
}

