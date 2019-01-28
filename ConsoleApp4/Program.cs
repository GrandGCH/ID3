using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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

            public Byte28(Int32 size)
            {
                this.size = (UInt32)size;
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

            private byte[] ConverttoArr(byte count)
            {
                byte[] nArr = new byte[4];
                byte mask = 127;
                UInt32 size = this.size;
                for(sbyte i=3;i>=0;i--)
                {
                    nArr[i] = (byte)(size >> 7*i & mask);
                }
                return nArr;
            }
       

            public byte[] PacktoArr()
            {
                byte[] nArr=null ;
                long max = 268435455;
                if (this.size >= max >> 21)
                {
                    if (this.size >= max >> 14)
                    {
                        if (this.size >= max >> 7)
                        {
                            nArr = ConverttoArr(4);
                        }
                        else
                        {
                            nArr = ConverttoArr(3);
                        }
                    }
                    else
                    {
                        nArr = nArr = ConverttoArr(2); ;
                    }
                }
                else
                    nArr = new byte[4] { 0, 0, 0, (byte)this.size };
                return nArr;
            }

        }

        public class Tag_Data
        {
            private string name;
            private byte[] flags = new byte[2];
            private byte[] data;
            private Int32 data_index;
            private UInt32 size;

            public Tag_Data(string name)
            {
                this.name =name + " doesn't found";
                size = 0;
            }
            public Tag_Data(string name, byte[] flags, byte[] size, Int32 data_index, ref byte[] data)
            {
                this.name = name;
                this.flags = flags;
                this.size = ToInt32(Fill(size));
                this.data = new byte[this.size];
                this.data_index = data_index;
                for (int i=0;i<this.size;i++)
                    this.data[i] = data[i + data_index];
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
            public string Decrypt()
            {
                string str = "";
                for(int i=0;i<this.size;i++)
                {
                    str += (char)this.data[i];
                }
                return str;
            }
            public string ReturnName()
            {
                return name;
            }
            public UInt32 ReturnHeaderSize()
            {
                return size;
            }
            public Int32 ReturnIndex()
            {
                return this.data_index;
            }
        }

        public class ID3TAG
        {
            private byte[] cur_header = new byte[10];
            private int cur_num_str = 0;
            private byte[] data;
            private Int32 index_free_bytes;
            private List<Tag_Data> list_name = new List<Tag_Data>();

            public ID3TAG(FileStream audio,Int32 size)
            {
                string[] Tag_name = {
                "TALB","TBPM","TCOM","TDAT","TEXT","TIME","TIT1","TIT2","TIT3","TLEN","TOAL","TOLY","TOPE",
                "TORY","TPE1","TPE2","TPE3","TPE4","TPOS","TPUB","TRCK","TRDA","TSIZ","TSSE","TYER","TCON","TCOM","COMM" };
                AddtoList();
                for (int i = 0; i < this.list_name.Count; i++)
                    this.list_name[i] = new Tag_Data(Tag_name[i]);
                this.data = new byte[size];
                audio.Read(this.data, 0, size);
            }

            private void Init(int id_str, byte[] header, Int32 data_index,ref byte[] data)
            {
                string[] Tag_name = {
                "TALB","TBPM","TCOM","TDAT","TEXT","TIME","TIT1","TIT2","TIT3","TLEN","TOAL","TOLY","TOPE",
                "TORY","TPE1","TPE2","TPE3","TPE4","TPOS","TPUB","TRCK","TRDA","TSIZ","TSSE","TYER","TCON","TCOM","COMM" };

                var flag = new byte[2] { header[8], header[9] };
                var size = new byte[4] { header[4], header[5], header[6], header[7] };
                this.list_name[id_str] = new Tag_Data(Tag_name[id_str], flag, size, data_index,ref data);
            }
            private void AddtoList()
            {
                Tag_Data TALB = null, TBPM = null, TCOM = null, TDAT = null, TEXT = null, TIME = null, TIT1 = null, TIT2 = null, TIT3 = null, TLEN = null, TOAL = null, TOLY = null, TOPE = null,
                TORY = null, TPE1 = null, TPE2= null, TPE3 = null, TPE4 = null, TPOS = null, TPUB = null, TRCK = null, TRDA = null, TSIZ = null, TSSE=null, TYER =null, TCON = null, COMM = null;
                this.list_name.Add(TALB); this.list_name.Add(TBPM); this.list_name.Add(TCOM); this.list_name.Add(TDAT);
                this.list_name.Add(TEXT); this.list_name.Add(TIME); this.list_name.Add(TIT1); this.list_name.Add(TIT2);
                this.list_name.Add(TIT3); this.list_name.Add(TLEN); this.list_name.Add(TOAL); this.list_name.Add(TOLY);
                this.list_name.Add(TOPE); this.list_name.Add(TORY); this.list_name.Add(TPE1); this.list_name.Add(TPE2);
                this.list_name.Add(TPE3); this.list_name.Add(TPE4); this.list_name.Add(TPOS); this.list_name.Add(TPUB);
                this.list_name.Add(TRCK); this.list_name.Add(TRDA); this.list_name.Add(TSIZ); this.list_name.Add(TSSE);
                this.list_name.Add(TYER); this.list_name.Add(TCON); this.list_name.Add(TCOM); this.list_name.Add(COMM); 
            }
            public void Unpack(byte[] Header,Int32 data_index)
            {
                for (int i = 0; i < 10; i++)
                    this.cur_header[i] = Header[i];
                this.cur_num_str = (int)Header[10];
                this.Init(this.cur_num_str, this.cur_header, data_index,ref this.data);
            } 
            public Int32 ReturnLength (Int32 str_id)
            {
                return (Int32)this.list_name[str_id].ReturnHeaderSize();
            }
            public ref byte[] ReturnData()
            {
                return ref this.data;
            }
            public Int32 ReturnCountFreeBytes()
            {
                return this.data.Length-this.index_free_bytes;
            }
            public void SetIndexFreeBytes(Int32 last_index)
            {
                this.index_free_bytes = last_index;
            }
            /*public void OutputTags()
            {
               for(Int32 i = 0;i<list_name.Count;i++)
                {
                    Console.WriteLine("-----------------");
                    Console.WriteLine("-----------------");
                    Console.WriteLine("-----------------");
                }
            }*/
        }

        public class ID3Header
        {
            private byte[] Header = new byte[10];
            private Int32 tagSize; 

            public ID3Header(FileStream audio)
            {
                audio.Read(this.Header, 0, 10);
                this.tagSize = (Int32)UnpackTagSize();
            }

            private UInt32 UnpackTagSize()
            {
                var nArr = new byte[4];
                for (byte i = 0; i < 4; i++)
                    nArr[i] = this.Header[i + 6];
                var Obj = new Byte28(nArr);
                return Obj.ReturnHeaderSize();
            }

            private byte RerurnMajorVersion()
            {
                return this.Header[3];
            }

            private byte RerurnMinorVersion()
            {
                return this.Header[4];
            }

            private byte RerurnFlag()
            {
                return this.Header[5];
            }

            public Int32 ReturnTagSize()
            {
                return this.tagSize;
            }

            public void OutputID3HeaderInfo()
            {
                Console.WriteLine("-------------ID3Header Info--------------");
                Console.WriteLine("The major version is {0}", this.RerurnMajorVersion());
                Console.WriteLine("The minor version is {0}", this.RerurnMinorVersion());
                Console.WriteLine("The flag unsynchronisation is {0} ", (this.RerurnFlag() & 128) == 128);
                Console.WriteLine("The flag extended header is {0} ", (this.RerurnFlag() & 64) == 64);
                Console.WriteLine("The flag experimental indicator is {0} ", (this.RerurnFlag() & 32) == 32);
                Console.WriteLine("The length of tags is {0}", this.tagSize);
                Console.WriteLine("-----------------------------------------");
            }

            public void SetHeaderSize(FileStream audio, Int32 size)
            {
                this.tagSize = size;
                Byte28 new_size = new Byte28(size);
                byte[] new_size_arr=new_size.PacktoArr();
                audio.Seek(6, 0);
                audio.Write(new_size_arr, 0, 4);
            }
        }

        public class Parser
        {
            Int64 ice = 0;
            Int32 size;
            Int32 curPos;
            Int32 curStr;
            bool endTags = false;
            byte[] ID3Data;
            private string[] Tag_name = {
            "TALB","TBPM","TCOM","TDAT","TEXT","TIME","TIT1","TIT2","TIT3","TLEN","TOAL","TOLY","TOPE",
            "TORY","TPE1","TPE2","TPE3","TPE4","TPOS","TPUB","TRCK","TRDA","TSIZ","TSSE","TYER","TCON","TCOM","COMM" };

            public Parser(ref byte[] ID3Data, Int32 curPos)
            {
                this.ID3Data = ID3Data;
                this.size = ID3Data.Length;
                this.curPos = curPos;
            }

            private byte[] Filter() //Шаблон, возвращает заголовок в 10 байтах и номер строки в 11-м

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
                        for (j = 0; j < this.Tag_name.Length - 1 && str.Contains(this.Tag_name[j]) == false;)
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
                    else if ((this.ID3Data[i] > 64 && this.ID3Data[i] < 91) || (this.ID3Data[i] > 48 && this.ID3Data[i] < 58))
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
                    else
                        endTags = true;
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
                return Filter();
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
            public bool ReturnEndTags()
            {
                return this.endTags;
            }
        }

        

        static void Main(string[] args)

        {
            void IncreaseFileSize(FileStream audio, Int64 tagSize)
            {
                var size = audio.Length;
                var size_data = size - 10 - tagSize;
                var new_size = 4096 - tagSize + size;
                var new_old_size_diff = new_size - size;
                var mb_size = 1024 * 1024;
                var src_data = new byte[mb_size];
                var dest_data = new byte[mb_size];

                audio.SetLength(new_size);
                audio.Seek(10 + tagSize, 0);

                Int64 foll_size = size_data;
                Int64 pos;

                while (foll_size < mb_size * 2)
                    mb_size /= 2;

                audio.Read(src_data, 0, mb_size);
                while (foll_size > 0)
                {
                    if (foll_size >= mb_size * 2)
                    {
                        pos = audio.Position;
                        audio.Read(dest_data, 0, mb_size);
                        audio.Seek(pos, 0);
                        audio.Write(src_data, 0, mb_size);
                        src_data = dest_data;
                        foll_size -= mb_size;
                    }
                    else if (foll_size == 1)
                    {
                        audio.Write(src_data, 0, mb_size);
                        foll_size--;
                    }
                    else
                        mb_size /= 2;
                }

                audio.Seek(10+tagSize, 0);
                for (Int64 i = 0; i < new_old_size_diff; i++)
                {
                    audio.WriteByte(0);
                }
            }

            using (FileStream audio = new FileStream(@"S:\FULL.mp3", FileMode.Open))
            {
                var ID3header = new ID3Header(audio);
                var ID3tag = new ID3TAG(audio,ID3header.ReturnTagSize());
                var parser = new Parser(ref ID3tag.ReturnData(), (Int32)0);

                while(parser.ReturnCurPos() < ID3header.ReturnTagSize() && parser.ReturnEndTags()==false)
                {
                    try
                    {
                        ID3tag.Unpack(parser.Result(),parser.ReturnCurPos()+10);
                    }
                    catch(NullReferenceException)
                    {
                        ID3tag.SetIndexFreeBytes(parser.ReturnCurPos());
                    }
                    if(parser.ReturnEndTags() == false)
                        parser.ChangeCurPos(ID3tag.ReturnLength(parser.ReturnCurStr()));
                }
                ID3header.OutputID3HeaderInfo();
                //ID3tag.OutputTags();
                Console.WriteLine("Count of free bytes is {0}", ID3tag.ReturnCountFreeBytes());

                if (ID3header.ReturnTagSize() < 4096)
                {
                    IncreaseFileSize(audio, ID3header.ReturnTagSize());
                    ID3header.SetHeaderSize(audio, 4096);
                }
            }
        }

    }

}
/*            TALB Album/Movie/Show title]    
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
            "TYER",    //TYER Year]  
*/