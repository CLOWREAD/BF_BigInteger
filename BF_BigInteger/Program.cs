using System;

public class BF_BInteger
{
    public const int BYTE_COUNT=64;
    public uint[] numbers = new uint[BYTE_COUNT];
    public bool sign = true;
    public override String ToString()
    {
        String res = "";
        for(int i=0;i< numbers.Length;i++)
        {

            res += String.Format("{0:X2}", numbers[i]%256);
        }

        return res;
    }
    public void FromHex(String Hex)
    {
        String res = "";
        char[] char_array =Hex.ToCharArray();
        byte[] byte_array = new byte[char_array.Length];
        if(byte_array.Length>BYTE_COUNT*2)
        {
            return;
        }

        for(int i=0;i<byte_array.Length;i++)
        {
            byte_array[i] = (byte)char_array[i];
        }


        byte temp_byte=0;
        int number_index=0;
        for (int i = byte_array.Length-1; i >=0; i--)
        {
            if(byte_array[i]>='A' && byte_array[i] <= 'F')
            {
                temp_byte =(byte)( byte_array[i] - 'A');
                temp_byte += 10;
            }
            if (byte_array[i] >= '0' && byte_array[i] <= '9')
            {
                temp_byte =(byte)(byte_array[i] - '0');
            }
            if ((byte_array.Length - 1 - i)%2==1)
            {
                temp_byte <<= 4;
            }
            number_index = byte_array.Length - 1 - i;
            number_index = number_index / 2;
            number_index = BYTE_COUNT-1 - number_index;
            numbers[number_index] += temp_byte;
            numbers[number_index] %= 256;

        }




    }

    public BF_BInteger  Add(BF_BInteger BI1)
    {
        BF_BInteger res = new BF_BInteger();
        uint temp_byte = 0;
        uint lower_byte_overflow = 0;
        int number_index = 0;
        for (int i = 0; i < BYTE_COUNT; i++)
        {
            number_index = BYTE_COUNT - 1 - i;
            temp_byte = this.numbers[number_index] + BI1.numbers[number_index]+ lower_byte_overflow;
            res.numbers[number_index] = (uint)temp_byte % 256;
            lower_byte_overflow= temp_byte / 256;
        }

        return res;
    }
    public int Compare(BF_BInteger BI1)
    {
        
        for (int i = 0; i < BYTE_COUNT; i++)
        {
            if(this.numbers[i] > BI1.numbers[i])
            {
                return 1;
            }
            if (this.numbers[i] < BI1.numbers[i])
            {
                return -1;
            }
        }

        return 0;
    }
    public BF_BInteger Duplicate()
    {
        BF_BInteger res = new BF_BInteger();
     
        for (int i = 0; i < BYTE_COUNT; i++)
        {
           
            res.numbers[i] = numbers[i];
            
        }

        return res;
    }
    public BF_BInteger Zero()
    {
        BF_BInteger res = new BF_BInteger();

        for (int i = 0; i < BYTE_COUNT; i++)
        {

            res.numbers[i] = 0;

        }

        return res;
    }

    public BF_BInteger Mul(BF_BInteger BI1)
    {
        BF_BInteger res = Zero();
        BF_BInteger[] mul_cache = new BF_BInteger[BYTE_COUNT * 8];
        mul_cache[0] = this.Duplicate();
        for (int i = 1; i < BYTE_COUNT*8/2; i++)
        {
            mul_cache[i] = mul_cache[i-1].Add(mul_cache[i-1]);
        }

        uint temp_byte = 0;
        uint temp_bit = 0;
        int number_index = 0;
        for (int i = 0; i < BYTE_COUNT * 8/2; i++)
        {
            number_index = BYTE_COUNT - 1 - (i/8);
            temp_byte = (uint)(BI1.numbers[number_index]);
            temp_bit = (uint)( temp_byte & (1 << (i % 8) ) );
            if(temp_bit!=0)
            {
                res = res.Add(mul_cache[i]);
            }

        }
       

        return res;
    }

    public BF_BInteger Div(BF_BInteger BI1)
    {
        BF_BInteger res = Zero();
        
        BF_BInteger[] mul_cache = new BF_BInteger[BYTE_COUNT * 8];
        mul_cache[0] = BI1.Duplicate();
        for (int i = 1; i < BYTE_COUNT * 8/2; i++)
        {
            mul_cache[i] = mul_cache[i - 1].Add(mul_cache[i - 1]);
        }
        BF_BInteger temp_div = Zero();
        BF_BInteger temptest= Zero();
        uint temp_byte = 0;
        uint temp_bit = 0;
        int number_index = 0;
        int cache_bit_index = 0;
        int bit_index = 0;

        for (int i = 0; i < BYTE_COUNT * 8/2; i++)
        {
            cache_bit_index = BYTE_COUNT * 8 / 2 -1 - i;
            temptest = temp_div.Add(mul_cache[cache_bit_index]);
            if(temptest.Compare(this)<=0)
            {
                temp_div = temptest.Duplicate();
                bit_index = BYTE_COUNT * 8 / 2 + i;
                number_index = bit_index / 8;
                temp_byte = (uint) res.numbers[number_index];
                temp_byte = (uint)(temp_byte | (0x80 >> (i % 8)));
                res.numbers[number_index] = (uint)temp_byte%256;

            }


        }


        return res;
    }


    public BF_BInteger Neg()
    {
        BF_BInteger res = new BF_BInteger();
        byte temp_byte = 0;
        
        int number_index = 0;
        for (int i = 0; i < BYTE_COUNT; i++)
        {
            number_index = BYTE_COUNT - 1 - i;
            temp_byte = (byte)this.numbers[number_index];
            temp_byte =(byte) ~temp_byte;
            res.numbers[number_index] = temp_byte;

        }

        return res;
    }
}



namespace BF_BigInteger
{
    class Program
    {
        static void Main(string[] args)
        {
            BF_BInteger bi1 = new BF_BInteger();
            bi1.FromHex("ABCD12345");
            BF_BInteger bi2 = new BF_BInteger();
            bi2.FromHex("ABCD12345");
            BF_BInteger bi3 = bi1.Mul(bi2);
            BF_BInteger bi4 = bi3.Div(bi2);

            Console.WriteLine(bi4.ToString());
        }
    }
}
