using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace DiskPartition

{
    class Program

    {
        public class Byte28
        {
            private UInt32 size;

            public Byte28(byte[] row_HeaderSize)
            {
                this.size = ToInt32(Fill(row_HeaderSize));
            }

            private byte[] Fill(byte[] row_HeaderSize)
            {
                var nArr = new byte[7];
                var prc_HeaderSize = new byte[28];
                for (int i = 0; i < 4; i++)
                {
                    if (row_HeaderSize[i] != 0)
                    {
                        nArr = this.ToBin(row_HeaderSize[i]);
                        for (int j = 0; j < 7; j++)
                            prc_HeaderSize[i * 7 + j] = nArr[j];
                    }
                }
                return prc_HeaderSize;
            }

            private byte[] ToBin(byte value)//Возвращает заполненный массив с битами
            {
                void Conversion(byte value_t, byte[] tArr, byte current)
                {
                    tArr[current] = (byte)(value_t % 2);
                    current--;
                    value_t /= 2;
                    if (value_t > 0)
                        Conversion(value_t, tArr, current);
                }
                byte[] nArr = new byte[7];
                Conversion(value, nArr, 6);
                return nArr;
            }
            public UInt32 ToInt32(byte[] prc_HeaderSize)
            {
                UInt32 pow(byte power, byte val)
                {
                    UInt32 answer = val;
                    if (power == 0 && val == 0)
                        answer = 0;
                    else if (power == 0)
                        answer = 1;
                    else if (power == 1)
                        return val;
                    else
                        for (byte i = 0; i < power; i++)
                            answer *= val;
                    return answer;
                }
                UInt32 value = 0;
                for (int i = 0; i < 28; i++)
                {
                    value <<= 1;
                    value += pow((byte)(27 - i), prc_HeaderSize[i]);
                }
                return value;
            }
            public UInt32 ReturnHeaderSize()
            {
                return size;
            }
        }

        static UInt32 ReturnHeaderSize(byte[] ID3Header)
        {
            var nArr = new byte[4];
            for (byte i = 0; i < 4; i++)
                nArr[i] = ID3Header[i + 6];
            var Obj = new Byte28(nArr);
            return Obj.ReturnHeaderSize();
        }

        public class Tag_Data
        {
            private string name;
            private byte[] flags = new byte[2];
            private UInt32 size;

            public Tag_Data(string name)
            {
                this.name =name + " doesn't found";
                size = 0;
            }
            public Tag_Data(string name, byte[] flags, byte[] size)
            {
                this.name = name;
                this.flags = flags;
                this.size = ToInt32(Fill(size));
            }

            private byte[] Fill(byte[] row_HeaderSize)
            {
                var nArr = new byte[8];
                var prc_HeaderSize = new byte[32];
                for (int i = 0; i < 4; i++)
                {
                    if (row_HeaderSize[i] != 0)
                    {
                        nArr = this.ToBin(row_HeaderSize[i]);
                        for (int j = 0; j < 8; j++)
                            prc_HeaderSize[i * 8 + j] = nArr[j];
                    }
                }
                return prc_HeaderSize;
            }

            private byte[] ToBin(byte value)//Возвращает заполненный массив с битами
            {
                void Conversion(byte value_t, byte[] tArr, byte current)
                {
                    tArr[current] = (byte)(value_t % 2);
                    current--;
                    value_t /= 2;
                    if (value_t > 0)
                        Conversion(value_t, tArr, current);
                }
                byte[] nArr = new byte[8];
                Conversion(value, nArr, 7);
                return nArr;
            }

            private UInt32 ToInt32(byte[] prc_HeaderSize)
            {
                UInt32 pow(byte power, byte val)
                {
                    UInt32 answer = val;
                    if (power == 0 && val == 0)
                        answer = 0;
                    else if (power == 0)
                        answer = 1;
                    else if (power == 1)
                        return val;
                    else
                        for (byte i = 0; i < power; i++)
                            answer *= val;
                    return answer;
                }
                UInt32 value = 0;
                for (int i = 0; i < 32; i++)
                {
                    value <<= 1;
                    value += pow((byte)(31 - i), prc_HeaderSize[i]);
                }
                return value;
            }
            public UInt32 ReturnHeaderSize()
            {
                return size;
            }
        }

        public class ID3TAG
        {
            private byte[] cur_header = new byte[10];
            private int cur_num_str = 0;
            List<Tag_Data> list_name = new List<Tag_Data>();
            public ID3TAG()
            {
                string[] Tag_name = {
                "TALB","TBPM","TCOM","TDAT","TEXT","TIME","TIT1","TIT2","TIT3","TLEN","TOAL","TOLY","TOPE",
                "TORY","TPE1","TPE2","TPE3","TPE4","TPOS","TPUB","TRCK","TRDA","TSIZ","TSSE","TYER","TCON" };
                AddtoList();
                for (int i = 0; i < this.list_name.Count; i++)
                    this.list_name[i] = new Tag_Data(Tag_name[i]);
            }

            private void Init(int id_str, byte[] header)
            {
                string[] Tag_name = {
                "TALB","TBPM","TCOM","TDAT","TEXT","TIME","TIT1","TIT2","TIT3","TLEN","TOAL","TOLY","TOPE",
                "TORY","TPE1","TPE2","TPE3","TPE4","TPOS","TPUB","TRCK","TRDA","TSIZ","TSSE","TYER","TCON" };

                var flag = new byte[2] { header[8], header[9] };
                var size = new byte[4] { header[4], header[5], header[6], header[7] };
                this.list_name[id_str] = new Tag_Data(Tag_name[id_str], flag, size);
            }
            private void AddtoList()
            {
                Tag_Data TALB = null, TBPM = null, TCOM = null, TDAT = null, TEXT = null, TIME = null, TIT1 = null, TIT2 = null, TIT3 = null, TLEN = null, TOAL = null, TOLY = null, TOPE = null,
                TORY = null, TPE1 = null, TPE2= null, TPE3 = null, TPE4 = null, TPOS = null, TPUB = null, TRCK = null, TRDA = null, TSIZ = null, TSSE=null, TYER =null, TCON = null;
                this.list_name.Add(TALB); this.list_name.Add(TBPM); this.list_name.Add(TCOM); this.list_name.Add(TDAT);
                this.list_name.Add(TEXT); this.list_name.Add(TIME); this.list_name.Add(TIT1); this.list_name.Add(TIT2);
                this.list_name.Add(TIT3); this.list_name.Add(TLEN); this.list_name.Add(TOAL); this.list_name.Add(TOLY);
                this.list_name.Add(TOPE); this.list_name.Add(TORY); this.list_name.Add(TPE1); this.list_name.Add(TPE2);
                this.list_name.Add(TPE3); this.list_name.Add(TPE4); this.list_name.Add(TPOS); this.list_name.Add(TPUB);
                this.list_name.Add(TRCK); this.list_name.Add(TRDA); this.list_name.Add(TSIZ); this.list_name.Add(TSSE);
                this.list_name.Add(TYER); this.list_name.Add(TCON);
            }
            public void Unpack(byte[] Header)
            {
                for (int i = 0; i < 10; i++)
                    this.cur_header[i] = Header[i];
                this.cur_num_str = (int)Header[10];
                this.Init(this.cur_num_str, this.cur_header);
            } 
            public Int32 ReturnLength (Int32 str_id)
            {
                return (Int32)this.list_name[str_id].ReturnHeaderSize();
            }

        }

        public class Parser
        {
            Int64 ice = 0;
            Int32 size;
            Int32 curPos;
            Int32 curStr;
            byte[] ID3Data;
            private string[] Tag_name = {
            "TALB","TBPM","TCOM","TDAT","TEXT","TIME","TIT1","TIT2","TIT3","TLEN","TOAL","TOLY","TOPE",
            "TORY","TPE1","TPE2","TPE3","TPE4","TPOS","TPUB","TRCK","TRDA","TSIZ","TSSE","TYER","TCON" };

            public Parser(ref byte[] ID3Data, Int32 curPos)
            {
                this.ID3Data = ID3Data;
                this.size = ID3Data.Length;
                this.curPos = curPos;
            }

            private byte[] Template() //Шаблон, возвращает заголовок в 10 байтах и номер строки в 11-м

            {
                byte step = 0;
                byte[] pars_res = new byte[11];
                byte status = 0;//Для отслеживания 3х букв
                string str="";
                for(Int32 i=curPos; i<size;i++)
                {
                    ice++;
                    if (step == 4)
                    {
                        int j;
                        for (j = 0; j < this.Tag_name.Length-1 && str.Contains(this.Tag_name[j]) == false;)
                            j++;
                        if (str.Contains(this.Tag_name[j]) == true)
                        {
                            byte[] temp = new byte[10];
                            temp = Header(i - 4); //Нужен он и j (в ID3)
                            for (byte z = 0; z < 10; z++)
                                pars_res[z] = temp[z];
                            pars_res[10] = (byte)j;
                            this.curStr = j;
                            return pars_res;
                        }
                        str = "";
                        status = 0;
                        step = 0;
                    }
                    else if ((this.ID3Data[i] > 64 && this.ID3Data[i] < 91) || (this.ID3Data[i] > 48 && this.ID3Data[i] < 58) )
                    {
                        if ((this.ID3Data[i] > 48 && this.ID3Data[i] < 58 && status == 0) && (status == 1 || status == 2))
                        {
                            status = 0;
                            step = 0;
                            str = "";
                        }
                        else
                        {
                            str += (char)this.ID3Data[i];
                            step++;
                            status++;
                        }
                    }
                    else if(step>0)
                    {
                        step = 0;
                        str = "";
                    }
                }
                return null;
            }
            private byte[] Header(Int32 index) //Возвращает массив байтов заголовка тега
            {
                byte[] nArr = new byte[10];
                for (int i = 0; i < size && i < 10; i++)
                    nArr[i] = ID3Data[index+i];
                return nArr;
            }
            public byte[] Result()
            {
                return Template();
            }
            public Int32 ReturnCurPos()
            {
                return this.curPos;
            }
            public void ChangeCurPos(Int32 pos)
            {
                this.curPos += pos+10;
            }
            public Int32 ReturnCurStr()
            {
                return this.curStr;
            }
        }

        static byte RerurnMajorVersion(byte[] bArr)
        {
            return bArr[3];
        }

        static byte RerurnMinorVersion(byte[] bArr)
        {
            return bArr[4];
        }

        static byte RerurnFlag(byte[] bArr)
        {
            return bArr[5];
        }

        static void Main(string[] args)

        {
            using (FileStream audio = new FileStream(@"S:\Bastille.mp3", FileMode.Open))
            {
                var ID3HeaderArr = new byte[10];
                audio.Read(ID3HeaderArr, 0, 10);

                Int32 HeaderSize = (int)ReturnHeaderSize(ID3HeaderArr);
                Console.WriteLine("The major version is {0}", RerurnMajorVersion(ID3HeaderArr));
                Console.WriteLine("The minoe version is {0}", RerurnMinorVersion(ID3HeaderArr));
                Console.WriteLine("The flag unsynchronisation is {0} ", (RerurnFlag(ID3HeaderArr) & 128) == 128);
                Console.WriteLine("The flag extended header is {0} ", (RerurnFlag(ID3HeaderArr) & 64) == 64);
                Console.WriteLine("The flag experimental indicator is {0} ", (RerurnFlag(ID3HeaderArr) & 32) == 32);
                Console.WriteLine("The length of tags is {0}", HeaderSize);

                var Data = new byte[HeaderSize];
                var D_char = new char[HeaderSize];
                audio.Read(Data, 0, HeaderSize); //Читает всю инфу

                Decoder Dec = Encoding.UTF8.GetDecoder();
                Dec.GetChars(Data, 0, HeaderSize, D_char, 0);//Декодирует в символьное

                //Хрень, содержащая все теги (почти)
                var ID3tag = new ID3TAG();
                var parser = new Parser(ref Data, (Int32)0);
                while(parser.ReturnCurPos() < (Int32)HeaderSize)
                {
                    ID3tag.Unpack(parser.Result());
                    parser.ChangeCurPos(ID3tag.ReturnLength(parser.ReturnCurStr()));
                }

                //----Парсинг----//

                byte status = 0;
                byte status2 = 0;
                for (UInt32 i = 0; i < HeaderSize; i++)
                    if (D_char[i] > 31 && D_char[i] < 127)
                    {
                        if (status2 == 0)
                        {
                            Console.Write("[{0}] {1}", i, D_char[i]);
                            status2 = 1;
                        }
                        else
                            Console.Write(D_char[i]);
                        status = 0;
                    }
                    else if (status == 0)
                    {
                        Console.WriteLine();
                        status = 1;
                        status2 = 0;
                    }
                //------------//
            }
        }

    }

}
/*"TALB",    //TALB Album/Movie/Show title]    
            "TBPM",    //TBPM BPM (beats per minute)]
            "TCOM",    //TCOM Composer]
            "TDAT",    //TDAT Date] 
            "TEXT",    //TEXT Lyricist/Text writer]
            "TIME",    //TIME Time]
            "TIT1",    //TIT1 Content group description]
            "TIT2",    //TIT2 Title/songname/content description]
            "TIT3",    //TIT3 Subtitle/Description refinement]
            "TLEN",    //TLEN Length]
            "TOAL",    //TOAL Original album/movie/show title]
            "TOLY",    //TOLY Original lyricist(s)/text writer(s)]
            "TOPE",    //TOPE Original artist(s)/performer(s)]
            "TORY",    //TORY Original release year]
            "TPE1",    //TPE1 Lead performer(s)/Soloist(s)]
            "TPE2",    //TPE2 Band/orchestra/accompaniment]
            "TPE3",    //TPE3 Conductor/performer refinement]
            "TPE4",    //TPE4 Interpreted, remixed, or otherwise modified by]
            "TPOS",    //TPOS Part of a set]
            "TPUB",    //TPUB Publisher]
            "TRCK",    //TRCK Track number/Position in set]
            "TRDA",    //TRDA Recording dates]
            "TSIZ",    //TSIZ Size]
            "TSSE",    //TSEE Software/Hardware and settings used for encoding]
            "TYER",    //TYER Year]  */