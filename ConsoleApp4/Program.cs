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
                for(sbyte i=(sbyte)count;i<4;i++)
                {
                    nArr[i] = (byte)(this.size >> 7*(i-1) & mask);
                }
                return nArr;
            }
       
            public byte[] PacktoArr()
            {
                byte[] nArr=null ;
                if (this.size < 1 << 21)
                {
                    if (this.size < 1 << 14)
                    {
                        if (this.size < 1 << 7)
                        {
                            nArr = new byte[4]{ 0,0,0,(byte)this.size};
                        }
                        else
                        {
                            nArr = ConverttoArr(2); ;
                        }
                    }
                    else
                    {
                        nArr =  ConverttoArr(3); ;
                    }
                }
                else
                    nArr = nArr = ConverttoArr(4);
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
            public Tag_Data(string name, byte[] flags, Int32 size, Int32 data_index, string value)
            {
                this.name = name;
                this.flags = flags;
                this.size = (UInt32)size;
                this.data = new byte[this.size];
                this.data_index = data_index;
                for (int i = 0; i < this.size; i++)
                    this.data[i] = (byte)value[i];
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
            private byte[] ConverttoArr(byte count)
            {
                byte[] nArr = new byte[4];
                byte mask = 255;
                for (sbyte i = (sbyte)count; i < 4; i++)
                {
                    nArr[i] = (byte)(this.size >> 8 * (i - 1) & mask);
                }
                return nArr;
            }
            public byte[] PacktoArr(Int32 size)
            {
                this.size = (UInt32) size;
                byte[] nArr = null;
                if (this.size < 1 << 24)
                {
                    if (this.size < 1 << 16)
                    {
                        if (this.size < 1 << 8)
                        {
                            nArr = new byte[4] { 0, 0, 0, (byte)this.size };
                        }
                        else
                        {
                            nArr = ConverttoArr(2); ;
                        }
                    }
                    else
                    {
                        nArr = ConverttoArr(3); ;
                    }
                }
                else
                    nArr = nArr = ConverttoArr(4);
                return nArr;
            }
        }

        public class ID3TAG
        {
            private byte[] cur_header = new byte[10];
            private int cur_num_str = 0;
            private byte[] data;
            public Int32 index_free_bytes=10;
            private List<Tag_Data> list_name = new List<Tag_Data>();
            string[] Tag_name = {
                "TALB","TBPM","TCOM","TDAT","TEXT","TIME","TIT1","TIT2","TIT3","TLEN","TOAL","TOLY","TOPE",
                "TORY","TPE1","TPE2","TPE3","TPE4","TPOS","TPUB","TRCK","TRDA","TSIZ","TSSE","TYER","TCON",
                "TCOM","COMM","AENC","APIC","COMR","ENCR","EQUA","ETCO","GEOB","GRID","IPLS","LINK","MCDI",
                "MLLT","OWNE","PRIV","PCNT","POPM","POSS","RBUF","RVAD","RVRB","SYLT","SYTC","TCOP",
                "TENC","TFLT","TKEY","TLAN","TMED","TOFN","TOWN","TRSN","TRSO","TSRC","TXXX","UFID",
                "USER","USLT","WCOM","WCOP","WOAF","WOAR","WOAS","WORS","WPAY","WPUB","WXXX"};

            public ID3TAG(FileStream audio,Int32 size)
            {
                AddtoList();
                for (int i = 0; i < this.list_name.Count; i++)
                    this.list_name[i] = new Tag_Data(this.Tag_name[i]);
                this.data = new byte[size];
                audio.Seek(10, 0);
                audio.Read(this.data, 0, size);
            }

            private void Init(int id_str, byte[] header, Int32 data_index,ref byte[] data)
            {
                var flag = new byte[2] { header[8], header[9] };
                var size = new byte[4] { header[4], header[5], header[6], header[7] };
                this.list_name[id_str] = new Tag_Data(this.Tag_name[id_str], flag, size, data_index,ref data);
            }
            private void AddtoList()
            {
                Tag_Data TALB = null, TBPM = null, TCOM = null, TDAT = null, TEXT = null, TIME = null, TIT1 = null, TIT2 = null, TIT3 = null, TLEN = null, TOAL = null, TOLY = null, TOPE = null,
                TORY = null, TPE1 = null, TPE2 = null, TPE3 = null, TPE4 = null, TPOS = null, TPUB = null, TRCK = null, TRDA = null, TSIZ = null, TSSE = null, TYER = null, TCON = null, COMM = null,
                AENC = null, APIC = null, COMR = null, ENCR = null, EQUA = null, ETCO = null, GEOB = null, GRID = null, IPLS = null, LINK = null, MCDI = null, MLLT = null, OWNE = null, PRIV = null,
                PCNT = null, POPM = null, POSS = null, RBUF = null, RVAD = null, RVRB = null, SYLT = null, SYTC = null, TCOP = null, TENC = null, TFLT = null, TKEY = null, TLAN = null, TMED = null,
                TOFN = null, TOWN = null, TRSN = null, TRSO = null, TSRC = null, TXXX = null, UFID = null, USER = null, USLT = null, WCOM = null, WCOP = null, WOAF = null, WOAR = null, WOAS = null,
                WORS = null, WPAY = null, WPUB = null, WXXX = null;

                this.list_name.Add(TALB); this.list_name.Add(TBPM); this.list_name.Add(TCOM); this.list_name.Add(TDAT);
                this.list_name.Add(TEXT); this.list_name.Add(TIME); this.list_name.Add(TIT1); this.list_name.Add(TIT2);
                this.list_name.Add(TIT3); this.list_name.Add(TLEN); this.list_name.Add(TOAL); this.list_name.Add(TOLY);
                this.list_name.Add(TOPE); this.list_name.Add(TORY); this.list_name.Add(TPE1); this.list_name.Add(TPE2);
                this.list_name.Add(TPE3); this.list_name.Add(TPE4); this.list_name.Add(TPOS); this.list_name.Add(TPUB);
                this.list_name.Add(TRCK); this.list_name.Add(TRDA); this.list_name.Add(TSIZ); this.list_name.Add(TSSE);
                this.list_name.Add(TYER); this.list_name.Add(TCON); this.list_name.Add(TCOM); this.list_name.Add(COMM);

                this.list_name.Add(AENC); this.list_name.Add(APIC); this.list_name.Add(COMR); this.list_name.Add(ENCR);
                this.list_name.Add(EQUA); this.list_name.Add(ETCO); this.list_name.Add(GEOB); this.list_name.Add(GRID);
                this.list_name.Add(IPLS); this.list_name.Add(LINK); this.list_name.Add(MCDI); this.list_name.Add(MLLT);
                this.list_name.Add(OWNE); this.list_name.Add(PRIV); this.list_name.Add(PCNT); this.list_name.Add(POPM);
                this.list_name.Add(POSS); this.list_name.Add(RBUF); this.list_name.Add(RVAD); this.list_name.Add(RVRB);
                this.list_name.Add(SYLT); this.list_name.Add(SYTC); this.list_name.Add(TCOP); this.list_name.Add(TENC);
                this.list_name.Add(TFLT); this.list_name.Add(TKEY); this.list_name.Add(TLAN); this.list_name.Add(TMED);

                this.list_name.Add(TOFN); this.list_name.Add(TOWN); this.list_name.Add(TRSN); this.list_name.Add(TRSO);
                this.list_name.Add(TSRC); this.list_name.Add(TXXX); this.list_name.Add(UFID); this.list_name.Add(USER);
                this.list_name.Add(USLT); this.list_name.Add(WCOM); this.list_name.Add(WCOP); this.list_name.Add(WOAF);
                this.list_name.Add(WOAR); this.list_name.Add(WOAS); this.list_name.Add(WORS); this.list_name.Add(WPAY);
                this.list_name.Add(WPUB); this.list_name.Add(WXXX); 
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
            public void OutputTags()
            {
               for(Int32 i = 0;i<list_name.Count;i++)
                {
                    if (this.list_name[i].ReturnHeaderSize() > 0)
                    {
                        Console.WriteLine("-----------------");
                        Console.WriteLine("Tag name: {0}", this.list_name[i].ReturnName());
                        Console.WriteLine("Tag contains: {0}", this.list_name[i].Decrypt());
                        Console.WriteLine("Tag begin position: {0}", this.list_name[i].ReturnIndex());
                        Console.WriteLine("Tag size: {0}", this.list_name[i].ReturnHeaderSize());
                        Console.WriteLine("-----------------");
                    }
                }
            }
            private void IncreaseTagSize(FileStream audio, Int32 shift_begin, Int32 shift_end, Int32 df_size)
            {
                var src_data = new byte[df_size];
                var dest_data = new byte[df_size];
                var null_data = new byte[df_size];

                Int64 pos = shift_begin;
                audio.Seek(pos, 0);
                audio.Read(src_data, 0, df_size);
                audio.Seek(pos, 0);
                audio.Write(null_data, 0, df_size);
                while (audio.Position < shift_end - df_size)
                {
                    pos = audio.Position;
                    audio.Read(dest_data, 0, df_size);
                    audio.Seek(pos, 0);
                    audio.Write(src_data, 0, df_size);
                    dest_data.CopyTo(src_data, 0);
                }
                Int32 size_remain = (Int32)(shift_end - audio.Position);
                pos = audio.Position;
                audio.Read(dest_data, 0, size_remain);
                audio.Seek(pos, 0);
                audio.Write(src_data, 0, df_size);
                audio.Write(dest_data, 0, size_remain);
            }
            private void CreateNewTag(FileStream audio, Int32 tag_begin, string tag_name, string value)
            {
                audio.Seek(tag_begin, 0);
                Int32 index=0;
                for(index=0;index<Tag_name.Length;index++)

                    if (Tag_name[index] == tag_name)
                        break;
                byte[] tag_size = list_name[index].PacktoArr(value.Length+1);
                for (Int32 i = 0; i < 4; i++)
                    audio.WriteByte((byte)tag_name[i]);
                audio.Write(tag_size, 0, 4);
                audio.WriteByte(0); //2 флага
                audio.WriteByte(0);
                audio.WriteByte(0);//на проверке
                for (Int32 i = 0; i < value.Length; i++)
                    audio.WriteByte((byte)value[i]);
            }
            public void SetNewTagValue(FileStream audio,string tag_name,string new_value)
            {
                void ShiftRW(Int32 id, ref byte[] arr_val)
                {
                    audio.Seek(list_name[id].ReturnIndex() + 10, 0);
                    audio.WriteByte(0);
                    audio.Write(arr_val, 0, arr_val.Length);
                    byte[] new_size = list_name[id].PacktoArr(arr_val.Length+1);
                    audio.Seek(list_name[id].ReturnIndex() + 4, 0);
                    audio.Write(new_size, 0, new_size.Length);
                }
                byte[] ToByteArr(string str)
                {
                    byte[] nArr = new byte[str.Length];
                    for (Int32 i = 0; i < str.Length; i++)
                        nArr[i] = (byte)str[i];
                    return nArr;
                }

                Int32 index = 0;
                Int32 tag_size = (Int32)list_name[index].ReturnHeaderSize();

                foreach (string str in this.Tag_name)
                {
                    if (str == tag_name)
                        break;
                    index++;
                }
                if (list_name[index].ReturnHeaderSize() == 0)
                {
                    CreateNewTag(audio, index_free_bytes, tag_name, new_value);
                    byte[] flags = new byte[2];
                    list_name[index] = new Tag_Data(tag_name, flags, new_value.Length, this.index_free_bytes + 10, new_value);
                    this.index_free_bytes += 10 + new_value.Length;
                }
                else                       
                {
                    byte[] arr_value = ToByteArr(new_value);
                    var df_size = tag_size - new_value.Length-1;
                    if (tag_size+1 < new_value.Length) //Если больше запланированного
                    {
                        Int32 max_index = 0;
                        Int32 i;
                        for (i = 0; i < list_name.Count; i++)
                        {
                            if (list_name[i].ReturnIndex() > max_index)
                                max_index = list_name[i].ReturnIndex();
                        }
                        Int32 end_index_max = (Int32)(max_index + 10 + this.list_name[i].ReturnHeaderSize());//В идеале сравнить с размером под заголовок
                        if (max_index != index)//Если тег не последний,расширить,сместить м перезаписать
                        {
                            Int32 shift_begin = tag_size + 10 + list_name[i].ReturnIndex();
                            this.IncreaseTagSize(audio, shift_begin, end_index_max, df_size);
                            ShiftRW(index, ref arr_value);
                        }
                        else//сместить м перезаписать
                            ShiftRW(index, ref arr_value);

                    }
                    else
                    {
                        Int32 tag_index = list_name[index].ReturnIndex();
                        byte[] null_arr = new byte[df_size];
                        audio.Seek(tag_index + 10, 0);
                        audio.WriteByte(0);//на проверке
                        audio.Write(arr_value, 0, new_value.Length);
                        audio.Write(null_arr, 0, df_size);//Могут быть 2 нуля перед
                    }
                }
            }
        }

        public class ID3Header
        {
            private byte[] Header = new byte[10];
            private Int32 tagSize;

            public ID3Header(FileStream audio)
            {
                audio.Seek(0, 0);
                audio.Read(this.Header, 0, 10);
                if (this.Header[0] != 73 && this.Header[1] != 68 && this.Header[2] != 51)
                {
                    CreateID3Header(audio);
                    audio.Seek(0, 0);
                    audio.Read(this.Header, 0, 10);
                }
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

            private void CreateID3Header(FileStream audio)
            {
                void IncreaseTagsSize(Int64 tagSize)
                {
                    var size = audio.Length;
                    var new_size = tagSize + size;
                    Int32 df_size = (Int32)(new_size - size);
                    var src_data = new byte[df_size];
                    var dest_data = new byte[df_size];
                    var null_data = new byte[df_size];

                    Int64 pos = 0;
                    audio.SetLength(audio.Length + tagSize);
                    audio.Seek(pos, 0);
                    audio.Read(src_data, 0, df_size);
                    audio.Seek(pos, 0);
                    audio.Write(null_data, 0, df_size);
                    while (audio.Position < size- df_size)
                    {
                        pos = audio.Position;
                        audio.Read(dest_data, 0, df_size);
                        audio.Seek(pos, 0);
                        audio.Write(src_data, 0, df_size);
                        dest_data.CopyTo(src_data, 0);
                    }
                    Int32 size_remain = (Int32)(size - audio.Position);
                    pos = audio.Position;
                    audio.Read(dest_data, 0, size_remain);
                    audio.Seek(pos, 0);
                    audio.Write(src_data, 0, df_size);
                    audio.Write(dest_data, 0, size_remain);
                }
                IncreaseTagsSize(4096);
                byte[] head = new byte[10] { 73, 68, 51, 3, 0, 0, 0, 0, 32, 0 };
                audio.Seek(0, 0);
                audio.Write(head, 0, 10);
            }

            public void SetHeaderSize(FileStream audio, Int32 size)
            {
                this.tagSize = size;
                Byte28 new_size = new Byte28(size);
                byte[] new_size_arr = new_size.PacktoArr();
                audio.Seek(6, 0);
                audio.Write(new_size_arr, 0, 4);
            }
        }

        public class Parser
        {
            Int32 size;
            Int32 curPos;
            Int32 curStr;
            bool endTags = false;
            byte[] ID3Data;
            string[] Tag_name = {
                "TALB","TBPM","TCOM","TDAT","TEXT","TIME","TIT1","TIT2","TIT3","TLEN","TOAL","TOLY","TOPE",
                "TORY","TPE1","TPE2","TPE3","TPE4","TPOS","TPUB","TRCK","TRDA","TSIZ","TSSE","TYER","TCON",
                "TCOM","COMM","AENC","APIC","COMR","ENCR","EQUA","ETCO","GEOB","GRID","IPLS","LINK","MCDI",
                "MLLT","OWNE","PRIV","PCNT","POPM","POSS","RBUF","RVAD","RVRB","SYLT","SYTC","TCOP",
                "TENC","TFLT","TKEY","TLAN","TMED","TOFN","TOWN","TRSN","TRSO","TSRC","TXXX","UFID",
                "USER","USLT","WCOM","WCOP","WOAF","WOAR","WOAS","WORS","WPAY","WPUB","WXXX"};

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
                for(Int32 i=curPos-10; i<size-10;i++)
                {
                    if (step == 4)
                    {
                        int j;
                        for (j = 0; j < this.Tag_name.Length && str.Contains(this.Tag_name[j]) == false;)
                            j++;
                        if (str.Contains(this.Tag_name[j]) == true)
                        {
                            byte[] temp = new byte[10];
                            temp = Header(i - 4); 
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
            void IncreaseTagsSize(FileStream audio, Int64 tagSize) //Закинуть в Header
            {
                var size = audio.Length;
                var size_data = size - 10 - tagSize;
                var new_size = 4096 - tagSize + size;
                Int32 df_size = (Int32)(new_size - size);
                var src_data = new byte[df_size];
                var dest_data = new byte[df_size];
                var null_data = new byte[df_size];

                Int64 pos = 10 + tagSize;
                audio.Seek(pos, 0);
                audio.Read(src_data, 0, df_size);
                audio.Seek(pos, 0);
                audio.Write(null_data, 0, df_size);
                while (audio.Position < size-df_size  )
                {    
                    pos = audio.Position;
                    audio.Read(dest_data, 0, df_size);
                    audio.Seek(pos, 0);
                    audio.Write(src_data, 0, df_size);
                    dest_data.CopyTo(src_data, 0);
                }
                Int32 size_remain = (Int32)(audio.Length - audio.Position);
                pos = audio.Position;
                audio.Read(dest_data, 0, size_remain);
                audio.Seek(pos, 0);
                audio.Write(src_data, 0, df_size);
                audio.Write(dest_data, 0, size_remain);
            } //Надо будет перекинуть в другое место
            
            using (FileStream audio = new FileStream(@"S:\Callejon - Utopia.mp3", FileMode.Open))
            {
                var ID3header = new ID3Header(audio);
                var ID3tag = new ID3TAG(audio, ID3header.ReturnTagSize());
                var parser = new Parser(ref ID3tag.ReturnData(), (Int32)10);

                while (parser.ReturnCurPos() < ID3header.ReturnTagSize() && parser.ReturnEndTags() == false)
                {
                    try
                    {
                        ID3tag.Unpack(parser.Result(), parser.ReturnCurPos());
                    }
                    catch (NullReferenceException)
                    {
                        ID3tag.index_free_bytes=parser.ReturnCurPos();
                    }
                    if (parser.ReturnEndTags() == false)
                        parser.ChangeCurPos(ID3tag.ReturnLength(parser.ReturnCurStr()));
                }

                ID3header.OutputID3HeaderInfo();
                ID3tag.OutputTags();
                if (ID3header.ReturnTagSize() < 4096) //4096 - предпочитаемый размер тегов
                {
                    IncreaseTagsSize(audio, ID3header.ReturnTagSize());
                    ID3header.SetHeaderSize(audio, 4096);
                    ID3header = new ID3Header(audio);
                    ID3header.OutputID3HeaderInfo();
                }
                ID3tag.SetNewTagValue(audio, "TPE2", "Eyes Without a Face");
            }
        }

    }

}