using System;
using System.IO;
using System.Text;

namespace mitttransdeploycli
{
  public class CDatFile
  {
    private static char[] hexDigits = new char[16]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F'
    };
    private string _workDirectory;
    private string _workFile;
    private byte _vers_dati_sw;

    public CDatFile(string workDirectory, string workFile, byte vers_dati_sw)
    {
      this._workDirectory = workDirectory;
      this._workFile = workFile;
      this._vers_dati_sw = vers_dati_sw;
      if (Directory.Exists(this._workDirectory))
        return;
      Directory.CreateDirectory(this._workDirectory);
    }

    public string ToHexString(byte[] bytes)
    {
      char[] chArray = new char[bytes.Length * 2];
      for (int index = 0; index < bytes.Length; ++index)
      {
        int num = (int) bytes[index];
        chArray[index * 2] = CDatFile.hexDigits[num >> 4];
        chArray[index * 2 + 1] = CDatFile.hexDigits[num & 15];
      }
      return new string(chArray);
    }

    public byte[] getByteToFileDat(string fileDat, int position, int numByte)
    {
      byte[] byteToFileDat;
      try
      {
        FileStream input = new FileStream(fileDat, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader((Stream) input);
        input.Seek(0L, SeekOrigin.Begin);
        binaryReader.BaseStream.Position = (long) position;
        byteToFileDat = binaryReader.ReadBytes(numByte);
        binaryReader.Close();
        input.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return byteToFileDat;
    }

    public string ConvertByteArrayToString(byte[] bytes)
    {
      char[] chArray = new char[bytes.Length * 2];
      for (int index = 0; index < bytes.Length; ++index)
      {
        int num = (int) bytes[index];
        chArray[index * 2] = CDatFile.hexDigits[num >> 4];
        chArray[index * 2 + 1] = CDatFile.hexDigits[num & 15];
      }
      return new string(chArray);
    }

    public byte[] ConvertStringToByteArray(string stringToConvert) => Encoding.ASCII.GetBytes(stringToConvert);

    public byte[] ConvertUintToByteArray(uint uintToConvert)
    {
      byte[] bytes1 = BitConverter.GetBytes(uintToConvert);
      byte[] bytes2 = BitConverter.GetBytes(uintToConvert);
      try
      {
        bytes2[0] = bytes1[3];
        bytes2[1] = bytes1[2];
        bytes2[2] = bytes1[1];
        bytes2[3] = bytes1[0];
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return bytes2;
    }

    public void writeFileChkSum()
    {
      uint num1 = 0;
      FileStream input = new FileStream(this._workDirectory + this._workFile + ".dat", FileMode.Open, FileAccess.ReadWrite);
      BinaryReader binaryReader = new BinaryReader((Stream) input);
      try
      {
        input.Seek(0L, SeekOrigin.Begin);
        binaryReader.BaseStream.Position = 0L;
        for (int index = 0; (long) index < input.Length; ++index)
        {
          long position = binaryReader.BaseStream.Position;
          string str = this.ConvertByteArrayToString(binaryReader.ReadBytes(1));
          if (position != 63L)
            num1 += Convert.ToUInt32(str, 16);
        }
        binaryReader.Close();
        input.Close();
        uint num2 = 256U - num1 % 256U;
        if (num2 == 256U)
          num2 = 0U;
        FileStream output = new FileStream(this._workDirectory + this._workFile + ".dat", FileMode.Open, FileAccess.ReadWrite);
        BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
        binaryWriter.BaseStream.Position = 63L;
        binaryWriter.Write(Convert.ToByte(num2));
        binaryWriter.Flush();
        output.Flush();
        binaryWriter.Close();
        output.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public int writeFileLung()
    {
      uint length;
      try
      {
        FileStream fileStream = new FileStream(this._workDirectory + this._workFile + ".dat", FileMode.Open, FileAccess.ReadWrite);
        length = (uint) fileStream.Length;
        fileStream.Close();
        FileStream output = new FileStream(this._workDirectory + this._workFile + ".dat", FileMode.Open, FileAccess.ReadWrite);
        BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
        binaryWriter.BaseStream.Position = 38L;
        binaryWriter.Write(this.ConvertUintToByteArray(length));
        binaryWriter.Flush();
        output.Flush();
        binaryWriter.Close();
        output.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      return (int) length;
    }

    public void createNewHeader(int typeOfHeader)
    {
      uint num1 = 0;
      byte num2 = 0;
      byte num3 = 0;
      uint uintToConvert1 = 0;
      uint uintToConvert2 = 0;
      ushort num4 = 0;
      uint num5 = 0;
      byte num6 = 0;
      try
      {
        if (typeOfHeader == 1)
        {
          num1 = 0U;
          num2 = (byte) 1;
          num3 = (byte) 60;
          uintToConvert1 = (uint) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
          uintToConvert2 = 1U;
          num4 = (ushort) 0;
          num5 = 0U;
          num6 = (byte) 0;
        }
        FileStream output = new FileStream(this._workDirectory + "header.dat", FileMode.Create, FileAccess.ReadWrite);
        BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
        binaryWriter.BaseStream.Position = 0L;
        binaryWriter.Write(this.ConvertStringToByteArray("MITT"));
        binaryWriter.Write(this.ConvertStringToByteArray(this._workFile.PadRight(30, ' ')));
        binaryWriter.Write(this.ConvertStringToByteArray(".dat"));
        binaryWriter.Write(num1);
        binaryWriter.Write(this._vers_dati_sw);
        binaryWriter.Write(num2);
        binaryWriter.Write(num3);
        binaryWriter.Write(this.ConvertUintToByteArray(uintToConvert1));
        binaryWriter.Write(this.ConvertUintToByteArray(uintToConvert1));
        binaryWriter.Write(this.ConvertUintToByteArray(uintToConvert2));
        binaryWriter.Write(num4);
        binaryWriter.Write(num5);
        binaryWriter.Write(num6);
        binaryWriter.Flush();
        output.Flush();
        binaryWriter.Close();
        output.Close();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public void createTrans(string path_trans, string nom_fil_trans)
    {
      try
      {
        this.createNewHeader(1);
        byte[] buffer1 = (byte[]) null;
        FileStream input1 = new FileStream(this._workDirectory + "header.dat", FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader1 = new BinaryReader((Stream) input1);
        input1.Seek(0L, SeekOrigin.Begin);
        binaryReader1.BaseStream.Position = 0L;
        while (binaryReader1.BaseStream.Position < input1.Length)
          buffer1 = binaryReader1.ReadBytes((int) input1.Length);
        byte[] buffer2 = (byte[]) null;
        FileStream input2 = new FileStream(path_trans + nom_fil_trans, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader2 = new BinaryReader((Stream) input2);
        input2.Seek(0L, SeekOrigin.Begin);
        binaryReader2.BaseStream.Position = 0L;
        while (binaryReader2.BaseStream.Position < input2.Length)
          buffer2 = binaryReader2.ReadBytes((int) input2.Length);
        FileStream output = new FileStream(this._workDirectory + this._workFile + ".dat", FileMode.Create, FileAccess.Write);
        BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
        output.Seek(0L, SeekOrigin.Begin);
        binaryWriter.BaseStream.Position = 0L;
        binaryWriter.Write(buffer1);
        binaryWriter.Write(buffer2);
        binaryWriter.Flush();
        output.Flush();
        binaryWriter.Close();
        output.Close();
        input1.Close();
        binaryReader1.Close();
        input2.Close();
        binaryReader2.Close();
        this.writeFileLung();
        this.writeFileChkSum();
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    private static string GetBit(byte b, byte position)
    {
      string bit = "0";
      if (((int) b & (int) (byte) (1U << (int) position)) != 0)
        bit = "1";
      return bit;
    }

  }
}
