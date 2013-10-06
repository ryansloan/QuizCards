using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizCards
{
    public class Deck : INotifyPropertyChanged
    {
        public String filename;
        private String _title;
        public String title
        {
            get
            {
                return this._title;
            }
            set
            {
                if (value == this._title)
                {
                    return;
                }
                else
                {
                    this._title = value;
                    OnPropertyChanged("title");
                }

            }
        }
        private String _sideAName;
        public String sideAName
        {
            get
            {
                return this._sideAName;
            }
            set
            {
                if (value == this._sideAName)
                {
                    return;
                }
                else
                {
                    this._sideAName = value;
                    OnPropertyChanged("sideAName");
                }
            }
        }
        private String _sideBName;
        public String sideBName
        {
            get
            {
                return this._sideBName;
            }
            set
            {
                if (value == this._sideBName)
                {
                    return;
                }
                else
                {
                    this._sideBName = value;
                    OnPropertyChanged("sideBName");
                }
            }
        }
        public ObservableCollection<Card> cards { get; set; }
        private int index = -1;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public Deck()
        {
            cards = new ObservableCollection<Card>();
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
        public int getPosition()
        {
            return this.index + 1;
        }
        public ObservableCollection<Card> getCards()
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
