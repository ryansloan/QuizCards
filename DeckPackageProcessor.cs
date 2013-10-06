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
        private bool deckDescriptionFound = false;
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
            this.deckDescriptionFound = false;
            this.deck.filename = file.Name;
            bool result = false;
            //Locate Archive based on this.packagePath
            var folder = ApplicationData.Current.TemporaryFolder;
            //Open Archive
            Stream stream = await file.OpenStreamForReadAsync();
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry en in archive.Entries)
                {             //Iterate through Entries
                    if (en.FullName.Contains("deckdescription.xml"))
                    {
                        char[] output = new char[en.Length];
                        this.deckDescriptionFound = true;
                        //Open deckdescription.xml and save it into deckXml
                        using (StreamReader sr = new StreamReader(en.Open()))
                        {
                            await sr.ReadAsync(output, 0, (int)en.Length);
                            //There was some weirdness here, which I'm documenting for the sake of my Memento-esque memory.
                            //We were hitting all kinds of weird XMLExceptions at the end of certain files (XMLExceptions around the Null character 0x00).
                            //The UTF-8 files effected always involved some characters that required two bytes. We'd hit this exception after the closing tag ("</deck>")
                            //When I inspected these files in a Hex editor, there were never any null characters at the end of the file, which was weird.
                            //My hypothesis was that when we built the output char array using en.Length, that length was returning the length in Bytes, which 
                            //was not the same as the number of characters (since some required 2 bytes). The result was that the output array was longer
                            //than the string that it was representing, and those extra slots were NULL (excess length was a function of the number of characters 
                            //requiring 2 bytes). Thus, I am trimming the null chars off the end.
                            //tl;dr: ZipArchiveEntry.Length doesn't actually return the number of characters in the file. This seems obvious in retrospect. 
                            this.deckXml = new String(output).TrimEnd('\0');
                        }
                    }
                    if (en.FullName.EndsWith(".jpg") || en.FullName.EndsWith(".jpeg") || en.FullName.EndsWith(".png"))
                    {
                        //Copy Images to LocalStorage - Tmp Folder
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
            if (this.deckDescriptionFound)
            {
                try
                {
                    result = processXml();
                }
                catch (XmlException e)
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
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
                                    return false;
                                }
                            }
                            else
                            {
                                tabDepth++;
                                if (reader.Name.Equals("title"))
                                {
                                    this.deck.setTitle(System.Net.WebUtility.HtmlDecode(reader.ReadElementContentAsString()));
                                    Debug.WriteLine("Set deck title to " + this.deck.getTitle());
                                }
                                else if (reader.Name.Equals("description"))
                                {
                                    //SET DESCRIPTION!
                                    Debug.WriteLine("Deck Description: " + System.Net.WebUtility.HtmlDecode(reader.ReadElementContentAsString()));
                                }
                                else if (reader.Name.Equals("sideaname"))
                                {
                                    this.deck.setSideAName(System.Net.WebUtility.HtmlDecode(reader.ReadElementContentAsString()));
                                }
                                else if (reader.Name.Equals("sidebname"))
                                {
                                    this.deck.setSideBName(System.Net.WebUtility.HtmlDecode(reader.ReadElementContentAsString()));
                                }
                                else if (reader.Name.Equals("cards"))
                                {
                                    tabDepth++;
                                }
                                else if (reader.Name.Equals("card"))
                                {
                                    tabDepth++;
                                    currentCard = new Card();
                                }
                                else if (reader.Name.Equals("sidealabel"))
                                {
                                    currentCard.sideALabel = System.Net.WebUtility.HtmlDecode(reader.ReadElementContentAsString());
                                }
                                else if (reader.Name.Equals("sideblabel"))
                                {
                                    currentCard.sideBLabel = System.Net.WebUtility.HtmlDecode(reader.ReadElementContentAsString());
                                }
                                else if (reader.Name.Equals("sideaimage"))
                                {
                                    currentCard.setSideAImage("ms-appdata:///temp/" + System.Net.WebUtility.HtmlDecode(reader.ReadElementContentAsString()));
                                }
                                else if (reader.Name.Equals("sidebimage"))
                                {
                                    currentCard.setSideBImage("ms-appdata:///temp/" + System.Net.WebUtility.HtmlDecode(reader.ReadElementContentAsString()));
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
                            if (reader.Name.Equals("card") || reader.Name.Equals("cards") || reader.Name.Equals("deck"))
                            {
                                tabDepth--;
                            }
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
            return true;
        }

        public async Task<bool> writePackageAsync(StorageFile file, Deck outDeck)
        {
            this.deck = outDeck;
            StorageFolder tmpFolder = ApplicationData.Current.TemporaryFolder;
            //create a MemoryStream to build ZipArchive in Memory
            using (MemoryStream zipStream = new MemoryStream())
            {

                //Create ZipArchive using MemoryStream
                using (ZipArchive archive = new ZipArchive(zipStream,ZipArchiveMode.Create,true)) { //leave zipstream open so we can copy from it later.
                    //Write File(s) to ZipArchive as entries
                    //for every image referenced by a card, we'll need to copy them over
                    String[] segments;
                    ZipArchiveEntry en;
                    foreach (Card c in this.deck.cards)
                    {
                        if (c.hasSideAImage())
                        {
                            segments = c.sideAImage.UriSource.Segments;
                            StorageFile inFile = await tmpFolder.GetFileAsync(segments[segments.Count() - 1]); //to be copied

                            using (Stream s = await inFile.OpenStreamForReadAsync()) //incoming stream
                            {
                                en = archive.CreateEntry(segments[segments.Count() - 1]);
                                using (Stream sout = en.Open()) //outgoing stream. Can't use en.Open() directly because it won't get properly disposed.
                                {
                                    await s.CopyToAsync(sout);
                                }
                            }
                        }
                        if (c.hasSideBImage())
                        {
                            segments = c.sideBImage.UriSource.Segments;
                            StorageFile inFile = await tmpFolder.GetFileAsync(segments[segments.Count() - 1]); //to be copied

                            using (Stream s = await inFile.OpenStreamForReadAsync()) //incoming stream
                            {
                                en = archive.CreateEntry(segments[segments.Count() - 1]);
                                using (Stream sout = en.Open()) //outgoing stream. Can't use en.Open() directly because it won't get properly disposed.
                                {
                                    await s.CopyToAsync(sout);
                                }
                            }
                        }

                    }
                    //Then we write the deck description file
                    deckToXml();
                    en = archive.CreateEntry("deckdescription.xml");
                    using (StreamWriter sw = new StreamWriter(en.Open()))
                    {
                        sw.Write(this.deckXml);
                    }

                }
            //Copy MemoryStream (entire Zip Archive) to file specified
                zipStream.Position = 0;
                using (Stream s = await file.OpenStreamForWriteAsync())
                {
                    zipStream.CopyTo(s);
                }
            }

            return true;
        }

        public bool deckToXml()
        {
            String[] segments;
            if (this.deck != null)
            {
                StringBuilder s = new StringBuilder();
                s.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                s.AppendLine("<deck>");
                s.Append("<title>");
                s.Append(System.Net.WebUtility.HtmlEncode(this.deck.title));
                s.AppendLine("</title>");
                s.Append("<description>");
                s.Append("");
                s.AppendLine("</description>");
                s.Append("<sideaname>");
                s.Append(System.Net.WebUtility.HtmlEncode(this.deck.sideAName));
                s.AppendLine("</sideaname>");
                s.Append("<sidebname>");
                s.Append(System.Net.WebUtility.HtmlEncode(this.deck.sideBName));
                s.AppendLine("</sidebname>");
                s.AppendLine("<cards>");
                foreach (Card c in this.deck.cards)
                {
                    s.AppendLine("<card>");
                    s.AppendLine("<sidealabel>" + System.Net.WebUtility.HtmlEncode(c.sideALabel) + "</sidealabel>");
                    s.AppendLine("<sideblabel>" + System.Net.WebUtility.HtmlEncode(c.sideBLabel) + "</sideblabel>");
                    if (c.hasSideAImage())
                    {
                        segments = c.sideAImage.UriSource.Segments;
                        s.AppendLine("<sideaimage>"+ System.Net.WebUtility.HtmlEncode(segments[segments.Count()-1]) +"</sideaimage>");
                    }
                    if (c.hasSideBImage())
                    {
                        segments = c.sideBImage.UriSource.Segments;
                        s.AppendLine("<sidebimage>" + System.Net.WebUtility.HtmlEncode(segments[segments.Count() - 1]) + "</sidebimage>");
                    }
                    s.AppendLine("</card>");
                }
                s.AppendLine("</cards>");
                s.AppendLine("</deck>");
                this.deckXml = s.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
