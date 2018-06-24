using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_CSharp
{
    class Blackjack
    {
        static void Main(string[] args)
        {
            handOfCards* player = new handOfCards();
            handOfCards* dealer = new handOfCards();
            bool gameOver = false;
            bool gotBlackjack = false;
            char playAgain;
            char hitOrStay;

            cout << "Casino Blackjack" << endl;
            cout << "Dealer hits on soft 17" << endl << endl;

            do
            {
                //deal first two cards to player and dealer
                for (int i = 0; i < 2; i++)
                {
                    player->draw();
                    dealer->draw();
                }

                //check for blackjack
                if (player->isBlackjack())
                {
                    cout << "You got blackjack! You won!" << endl;
                    gameOver = true;
                    gotBlackjack = true;
                }
                else if (dealer->isBlackjack())
                {
                    cout << "Dealer got blackjack. You lost." << endl;
                    gameOver = true;
                    gotBlackjack = true;
                }

                while (!gameOver)
                {
                    cout << "Dealer:\t";
                    dealer->print(true);
                    cout << "Player:\t";
                    player->print(false);

                    do
                    {
                        cout << "Do you want to hit or stay? [H / S]:";
                        cin >> hitOrStay;
                        if (tolower(hitOrStay) != 'h' && tolower(hitOrStay) != 's')
                            cout << "Please enter H or S." << endl;
                    } while (tolower(hitOrStay) != 'h' && tolower(hitOrStay) != 's');

                    if (tolower(hitOrStay) == 'h')
                    {
                        player->draw();
                        dealer->dealerPlay();
                    }
                    else
                    {
                        dealer->dealerPlay();
                        gameOver = true;
                    }
                }

                //if player or dealer got a blackjack, game was already decided
                if (!gotBlackjack)
                {
                    //report final card totals
                    cout << endl << "Final results:" << endl << endl;
                    cout << "Dealer: " << dealer->value() << endl;
                    dealer->print(false);
                    cout << endl;
                    cout << "Player: " << player->value() << endl; ;
                    player->print(false);
                    cout << endl;

                    //declare winner
                    if (player->isBlackjack())
                        cout << "You got blackjack! You won!" << endl;
                    else if (dealer->isBlackjack())
                        cout << "Dealer got blackjack. You lost." << endl;
                    else if (player->value() > 21)
                        cout << "You bust. You lose." << endl;
                    else if (dealer->value() > 21)
                        cout << "Dealer bust. You won!" << endl;
                    else if (player->value() == dealer->value())
                        cout << "Draw." << endl;
                    else if (player->value() > dealer->value())
                        cout << "You won!" << endl;
                    else
                        cout << "You lost." << endl;
                }

                //ask if they want to play again
                do
                {
                    cout << endl << "Do you want to play again? [Y/N]";
                    cin >> playAgain;
                    if (tolower(playAgain) != 'y' && tolower(playAgain) != 'n')
                        cout << "Please enter H or S." << endl;
                } while (tolower(playAgain) != 'y' && tolower(playAgain) != 'n');

                //if playing again, reset hands and gameover
                if (tolower(playAgain) == 'y')
                {
                    player->clear();
                    dealer->clear();
                    gameOver = false;
                }
            } while (tolower(playAgain) == 'y');
        }
    }

    //enum for face cards
    enum FaceCards
    {
        ACE = 1,
        JACK = 11,
        QUEEN = 12,
        KING = 13
    };

    ostream &operator <<(ostream &outs, FaceCards card)
    {
        switch (card)
        {
            case ACE:
                outs << "Ace";
                break;
            case JACK:
                outs << "Jack";
                break;
            case QUEEN:
                outs << "Queen";
                break;
            case KING:
                outs << "King";
                break;
            default:
                outs << static_cast<int>(card);
                break;
        }

        return outs;
    }

    class card
    {
        public:
	int type;
        int value;
        card* link;
    };

    class handOfCards
    {
        private:
	card* head;
        card* end;

        card* newCard()
        {
            card* temp;
            temp = new card();
            temp->link = NULL;
            temp->type = (rand() % 13) + 1;
            temp->value = assignValue(temp);
            return temp;
        }

        //assigns the values to cards base on the card type
        int assignValue(card* temp)
        {
            int val;
            FaceCards tempType;
            tempType = static_cast<FaceCards>(temp->type);

            //assign value of 10 to jack, queen, king
            //everyone else just gets their card type (1-10)
            if (tempType == JACK || tempType == QUEEN || tempType == KING)
                val = 10;
            else
                val = temp->type;

            return val;
        }

        public:
	handOfCards()
        {
            clear();
            srand(time(NULL));
        }

        //clears the hand to play again
        void clear()
        {
            head = NULL;
            end = NULL;
        }

        //deals a new card on the hand
        void draw()
        {
            card* temp;
            temp = newCard();

            if (head == NULL)
            {
                head = temp;
                end = temp;
            }
            else
            {
                end->link = temp;
                end = temp;
            }
        }

        //adds up the hand
        int value()
        {
            int value = 0;
            int aceCount = 0;
            card* cur = head;

            while (cur)
            {
                if (cur->type == 1)
                {
                    value += 11;
                    aceCount++;
                }
                else
                    value += cur->value;

                cur = cur->link;
            }

            //if we have aces, and counting them all as 11 has us bust,
            //try setting to 1
            while (aceCount > 0 && value > 21)
            {
                value -= 10; //swap an ace from 11 to 1
                aceCount--;
            }

            return value;
        }

        //print function with flag on whether or not to print dealer first card
        void print(bool hideDealerCard)
        {
            card* cur = head;
            int cardNum = 1;
            while (cur)
            {
                if (cardNum == 1 && hideDealerCard)
                    cout << "*\t";
                else
                    cout << static_cast<FaceCards>(cur->type) << '\t';

                cur = cur->link;
                cardNum++;
            }

            cout << endl;
        }

        //determines if a hand has been dealt blackjack (only run with two cards dealt)
        bool isBlackjack()
        {
            bool blackjack = false;
            int handValue = head->value + end->value;

            //if the hand is a face card or ten and an ace, the value will be 11
            if (handValue == 11)
                blackjack = true;

            return blackjack;
        }

        //determines if a hand has aces in it
        bool hasAces()
        {
            bool aces = false;
            card* cur = head;

            while (cur)
            {
                if (cur->type == 1)
                    aces = true;

                cur = cur->link;
            }

            return aces;
        }

        //plays the dealers hand
        void dealerPlay()
        {
            int handValue = value();
            bool aces = hasAces();

            if (handValue < 17)
                this->draw();
            else if (handValue == 17 && aces)
                this->draw();
        }
    };
}
