using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizCards
{
    public class Deck
    {
        private String title;
        private String sideAName;
        private String sideBName;
        private List<Card> cards;
        private int index = -1;

        public Deck()
        {
            cards = new List<Card>();
        }

        public String getTitle()
        {
            return this.title;
        }
        public String getSideAName()
        {
            return this.sideAName;
        }
        public String getSideBName()
        {
            return this.sideBName;
        }
        public void setTitle(String t)
        {
            this.title = t;
        }
        public void setSideAName(String a)
        {
            this.sideAName = a;
        }
        public void setSideBName(String b)
        {
            this.sideBName = b;
        }

        public bool addCard(Card c)
        {
            this.cards.Add(c);
            return true;
        }
        public void shuffle()
        {
            Random randNum = new Random();
            int swapIndex = 0;
            Card tmp;
            for (int i = 0; i < this.cards.Count; i++)
            {
                swapIndex = randNum.Next(0, this.cards.Count);
                tmp = this.cards[swapIndex];
                this.cards[swapIndex] = this.cards[i];
                this.cards[i] = tmp;

            }
        }
        public Card getNextCard()
        {
            if (index < this.cards.Count - 1)
            {
                return this.cards.ElementAt(++this.index);
            }
            else
            {
                return null;
            }
        }
        public bool isNextCard()
        {
            return (this.index < this.cards.Count - 1);
        }
        public bool isPreviousCard()
        {
            return (this.index > 0);
        }
        public Card getPreviousCard()
        {
            if (index > 0)
            {
                return this.cards.ElementAt(--this.index);
            }
            else
            {
                return null;
            }
        }
        public int getLength()
        {
            return this.cards.Count;
        }
        public List<Card> getCards()
        {

            return this.cards;
        }
        public void disposeOfBitmaps()
        {
            foreach (Card c in this.cards)
            {
                c.emptyBitmap();
            }
        }
    }
}
