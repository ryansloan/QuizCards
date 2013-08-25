using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace QuizCards
{
    class DeckPackageProcessor
    {
        public String packagePath;
        public Deck deck;
        private String deckXml;
        public DeckPackageProcessor()
        {
            this.deck = new Deck();
            this.packagePath = null;
        }
        public DeckPackageProcessor(String p)
        {
            this.deck = new Deck();
            this.packagePath = p;
        }
        public async Task<bool> readPackageAsync(StorageFile file)
        {
            //Locate Archive based on this.packagePath
            var folder = ApplicationData.Current.LocalFolder;
            /*StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/test.zip"));*/
            //Open Archive
            Stream stream = await file.OpenStreamForReadAsync();
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry en in archive.Entries)
                {             //Iterate through Entries
                    if (en.FullName.Contains("deckdescription.xml"))
                    {
                        char[] output = new char[en.Length];

                        using (StreamReader sr = new StreamReader(en.Open()))
                        {
                            //Open deckdescription.xml and save it into deckXml
                            await sr.ReadAsync(output, 0, (int)en.Length);
                            this.deckXml = new String(output);
                        }
                    }
                    else if (en.FullName.EndsWith(".jpg") || en.FullName.EndsWith(".jpeg") || en.FullName.EndsWith(".png"))
                    {
                        //Copy Images to LocalStorage
                        using (Stream picdata = en.Open())
                        {
                            StorageFile outfile = await folder.CreateFileAsync(en.Name, CreationCollisionOption.ReplaceExisting);
                            using (Stream outputfilestream = await outfile.OpenStreamForWriteAsync())
                            {
                                await picdata.CopyToAsync(outputfilestream);
                                await outputfilestream.FlushAsync(); //flush makes sure all the bits are written
                            }

                        }
                    }
                }
            }

            //kick off processXml, return result of that.
            return processXml();
        }

        public bool processXml()
        {
            //Process this.deckXml and:
            //Populate Deck [title, sideA and sideB names, description]
            //Populate Cards, linking to bitmaps that are loaded in localstorage.
            //return true if succeeded
            using (XmlReader reader = XmlReader.Create(new StringReader(this.deckXml)))
            {
                Card currentCard = null;
                int tabDepth = 0;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if ((tabDepth == 0))
                            {
                                if (reader.Name.Equals("deck"))
                                {
                                    tabDepth++;
                                }
                                else
                                {
                                    Debug.WriteLine("ERROR: improperly structured deck description.");
                                }
                            }
                            else
                            {
                                tabDepth++;
                                if (reader.Name.Equals("title"))
                                {
                                    this.deck.setTitle(reader.ReadElementContentAsString());
                                    Debug.WriteLine("Set deck title to " + this.deck.getTitle());
                                }
                                else if (reader.Name.Equals("description"))
                                {
                                    //SET DESCRIPTION!
                                    Debug.WriteLine("Deck Description: " + reader.ReadElementContentAsString());
                                }
                                else if (reader.Name.Equals("sideaname"))
                                {
                                    this.deck.setSideAName(reader.ReadElementContentAsString());
                                }
                                else if (reader.Name.Equals("sidebname"))
                                {
                                    this.deck.setSideBName(reader.ReadElementContentAsString());
                                }
                                else if (reader.Name.Equals("cards"))
                                {
                                    Debug.WriteLine("entering cards...");
                                }
                                else if (reader.Name.Equals("card"))
                                {
                                    currentCard = new Card();
                                }
                                else if (reader.Name.Equals("sidealabel"))
                                {
                                    currentCard.sideALabel = reader.ReadElementContentAsString();
                                    Debug.WriteLine("Reading card labeled " + currentCard.sideALabel);
                                }
                                else if (reader.Name.Equals("sideblabel"))
                                {
                                    currentCard.sideBLabel = reader.ReadElementContentAsString();
                                }
                                else if (reader.Name.Equals("sideaimage"))
                                {
                                    currentCard.setImage("ms-appdata:///local/" + reader.ReadElementContentAsString());
                                }

                            }
                            break;
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.Text:
                            break;
                        case XmlNodeType.XmlDeclaration:
                            break;
                        case XmlNodeType.EndElement:
                            tabDepth--;
                            if (reader.Name.Equals("card"))
                            {
                                //Card info is ready, add to deck!
                                this.deck.addCard(currentCard);
                            }
                            break;
                    }
                }

                Debug.WriteLine("Deck '" + this.deck.getTitle() + "' has " + this.deck.getLength() + " cards.");
            }
            return false;
        }


    }
}
